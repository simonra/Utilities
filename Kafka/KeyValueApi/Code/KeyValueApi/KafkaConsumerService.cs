using Confluent.Kafka;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IKeyValueStateService _keyValueStateService;
    private readonly EnvHelpers _envHelpers;
    private readonly HttpClient _httpClient;
    private readonly Func<byte[], byte[]> _decrypt;
    private readonly Func<string, string> _decryptHeaderKey;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IKeyValueStateService keyValueStateService, EnvHelpers envHelpers, HttpClient httpClient, KafkaAdminClient kafkaAdminClient)
    {
        _logger = logger;
        _keyValueStateService = keyValueStateService;
        _envHelpers = envHelpers;
        _httpClient = httpClient;
        var topicsCreated = kafkaAdminClient.TryCreateTopics().GetAwaiter().GetResult();
        if (_envHelpers.GetEnvironmentVariableContent(KV_API_ENCRYPT_DATA_ON_KAFKA) == "true")
        {
            var cryptoService = new CryptoService();
            _decrypt = cryptoService.Decrypt;
            _decryptHeaderKey = delegate(string input) { return System.Text.Encoding.UTF8.GetString(cryptoService.Decrypt(Convert.FromBase64String(input))); };
        }
        else
        {
            _decrypt = delegate(byte[] input) { return input; };
            _decryptHeaderKey = delegate(string input) { return input; };
        }
        _logger.LogInformation($"{nameof(KafkaConsumerService)} initialized");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer service is doing pre startup blocking work.");

        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer service background task started.");

        var consumerConfig = KafkaConsumerConfigGenerator.GetConsumerConfig();
        var consumer = new ConsumerBuilder<byte[], byte[]>(consumerConfig)
            .SetPartitionsAssignedHandler((c, partitions) =>
            {
                var offsets = partitions.Select(tp => new TopicPartitionOffset(tp, Offset.Beginning));
                return offsets;
            })
            .Build();
        var topic = new KafkaTopic { Value = _envHelpers.GetEnvironmentVariableContent(KAFKA_KEY_VALUE_TOPIC) };
        consumer.Subscribe(topic.Value);

        // Don't check if topic has schemas defined for every invocation, jut do it once and use the appropriate handling method.
        // Note for future me: array[x..] is new more efficient dotnet syntax for skip first x and take rest of array
        Func<byte[], byte[]> handleSchemaMagicBytesInKey =
            await TopicKeyHasSchema(topic, stoppingToken)
                ? delegate(byte[] input) { return input[5..]; }
                : delegate(byte[] input) { return input; };
        // Func<byte[], byte[]> handleSchemaMagicBytesInKey =
        //     TopicKeyHasSchema(topic)
        //         ? (byte[] input) => input[5..]
        //         : (byte[] input) => input;
        Func<byte[], byte[]> handleSchemaMagicBytesInValue =
            await TopicValueHasSchema(topic, stoppingToken)
                ? delegate(byte[] input) { return input[5..]; }
                : delegate(byte[] input) { return input; };
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);
                if (result?.Message == null)
                {
                    Console.WriteLine("We've reached the end of the topic.");
                    await Task.Delay(TimeSpan.FromSeconds(8), stoppingToken);
                }
                else
                {
                    // Console.WriteLine($"The next event on topic {result.TopicPartitionOffset.Topic} partition {result.TopicPartitionOffset.Partition.Value} offset {result.TopicPartitionOffset.Offset.Value} received to the topic at the time {result.Message.Timestamp.UtcDateTime:u} has the value {result.Message.Value}");
                    if(result.Message.Value == null)
                    {
                        // ToDo: handle tombstone, delete from dict
                        var key = _decrypt(handleSchemaMagicBytesInKey(result.Message.Key));
                        _keyValueStateService.Remove(key);
                    }
                    else
                    {
                        var key = _decrypt(handleSchemaMagicBytesInKey(result.Message.Key));
                        var value = _decrypt(handleSchemaMagicBytesInValue(result.Message.Value));
                        _keyValueStateService.Store(key, value);
                    }
                }
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Kafka consumer received exception while consuming, exiting");
        }
        finally
        {
            // Close consumer
            _logger.LogInformation("Disconnecting consumer from Kafka cluster, leaving consumer group and all that");
            consumer.Close();
        }
    }

    private async Task<bool> TopicKeyHasSchema(KafkaTopic topic, CancellationToken stoppingToken)
    {
        // curl -X GET http://localhost:8083/subjects/topicname-key/versions
        // If the above doesn't work, screw it, just go with
        // curl -X GET http://localhost:8083/subjects
        // And grep for the expected name in the resulting array (should look like `["subject-1","subject-2",...,"last-subject"]`).
        // The proper
        // curl -X POST -H "Content-Type: application/vnd.schemaregistry.v1+json" --data '{"schema": "{\"type\": \"string\"}"}' http://localhost:8083/subjects/topicname-key
        // is too much work to stuff into the dotnet http client, and the answer is also more involved to parse than what I can be bothered with right now.

        var schemaRegistryAddress = GetSchemaRegistryAddress();
        var keySchemaAddress = $"{schemaRegistryAddress}/subjects/{topic}-key/versions";
        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(keySchemaAddress,stoppingToken);
            if(response.IsSuccessStatusCode && response.Content.ReadAsStringAsync(stoppingToken).Result.Length > 0)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Got exception while checking if topic {topic} has schema by retrieving {keySchemaAddress}");
        }
        return false;
    }

    private async Task<bool> TopicValueHasSchema(KafkaTopic topic, CancellationToken stoppingToken)
    {
        var schemaRegistryAddress = GetSchemaRegistryAddress();
        var valueSchemaAddress = $"{schemaRegistryAddress}/subjects/{topic}-value/versions";
        try
        {
            using HttpResponseMessage response = await _httpClient.GetAsync(valueSchemaAddress,stoppingToken);
            if(response.IsSuccessStatusCode && response.Content.ReadAsStringAsync(stoppingToken).Result.Length > 0)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Got exception while checking if topic {topic} has schema by retrieving {valueSchemaAddress}");
        }
        return false;
    }

    private string GetSchemaRegistryAddress()
    {
        var schemaRegistryAddress = _envHelpers.GetEnvironmentVariableContent(KAFKA_SCHEMA_REGISTRY_ADDRESS);
        if(!schemaRegistryAddress.StartsWith("http"))
        {
            _logger.LogWarning($"Schema registry address found in env variable \"{KAFKA_SCHEMA_REGISTRY_ADDRESS}\" does not start with \"http\"/specify the protocol. It's value is \"{schemaRegistryAddress}\".");
        }
        return schemaRegistryAddress;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer received request for graceful shutdown.");

        await base.StopAsync(stoppingToken);
    }
}
