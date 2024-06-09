using Confluent.Kafka;
using Confluent.SchemaRegistry;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IKeyValueStateService _keyValueStateService;
    private readonly HttpClient _httpClient;
    private readonly KafkaAdminClient _kafkaAdminClient;
    private readonly CachedSchemaRegistryClient _schemaRegistryClient;
    private readonly KafkaTopic _topic;
    private readonly Func<byte[], byte[]> _decrypt;
    private readonly Func<string, string> _decryptHeaderKey;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IKeyValueStateService keyValueStateService, HttpClient httpClient, KafkaAdminClient kafkaAdminClient)
    {
        _logger = logger;
        _keyValueStateService = keyValueStateService;
        _httpClient = httpClient;
        _kafkaAdminClient = kafkaAdminClient;
        if (Environment.GetEnvironmentVariable(KV_API_ENCRYPT_DATA_ON_KAFKA) == "true")
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

        var topicName = Environment.GetEnvironmentVariable(KV_API_KAFKA_KEY_VALUE_TOPIC);
        if(string.IsNullOrWhiteSpace(topicName))
        {
            _logger.LogError($"Cannot consume if topic is not specified. Environment variable {nameof(KV_API_KAFKA_KEY_VALUE_TOPIC)} was not set/is empty.");
            throw new InvalidOperationException($"Environment variable {nameof(KV_API_KAFKA_KEY_VALUE_TOPIC)} has to have value.");
        }
        _topic = new KafkaTopic { Value = topicName };

        var schemaRegistryClientConfig = KafkaSchemaRegistryConfigGenerator.GetSchemaRegistryConfig();
        var schemaRegistryClient = new Confluent.SchemaRegistry.CachedSchemaRegistryClient(schemaRegistryClientConfig);
        _schemaRegistryClient = schemaRegistryClient;

        _logger.LogInformation($"{nameof(KafkaConsumerService)} initialized");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = await _kafkaAdminClient.TryCreateTopics();
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

        consumer.Subscribe(_topic.Value);

        // Don't check if topic has schemas defined for every invocation, jut do it once and use the appropriate handling method.
        // Note for future me: array[x..] is new more efficient dotnet syntax for skip first x and take rest of array
        Func<byte[], byte[]> handleSchemaMagicBytesInKey =
            await TopicKeyHasSchema(_topic, stoppingToken)
                ? delegate(byte[] input) { return input[5..]; }
                : delegate(byte[] input) { return input; };
        // Func<byte[], byte[]> handleSchemaMagicBytesInKey =
        //     TopicKeyHasSchema(topic)
        //         ? (byte[] input) => input[5..]
        //         : (byte[] input) => input;
        Func<byte[], byte[]> handleSchemaMagicBytesInValue =
            await TopicValueHasSchema(_topic, stoppingToken)
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

        var schemaName = $"{topic.Value}-key";
        try
        {
            var registeredSchemas = await _schemaRegistryClient.GetAllSubjectsAsync();
            return registeredSchemas?.Any(rs => rs == schemaName) == true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Got exception while checking if topic {topic.Value} key has schema by retrieving {schemaName}");
        }
        return false;
    }

    private async Task<bool> TopicValueHasSchema(KafkaTopic topic, CancellationToken stoppingToken)
    {
        var schemaName = $"{topic.Value}-value";
        try
        {
            var registeredSchemas = await _schemaRegistryClient.GetAllSubjectsAsync();
            return registeredSchemas?.Any(rs => rs == schemaName) == true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Got exception while checking if topic {topic} value has schema by retrieving {schemaName}");
        }
        return false;
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer received request for graceful shutdown.");

        await base.StopAsync(stoppingToken);
    }
}
