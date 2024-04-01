using System;

using Confluent.Kafka;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly KeyValueStateService _keyValueStateService;
    private readonly EnvHelpers _envHelpers;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, KeyValueStateService keyValueStateService, EnvHelpers envHelpers)
    {
        _logger = logger;
        _keyValueStateService = keyValueStateService;
        _envHelpers = envHelpers;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer service is doing pre startup blocking work.");

        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer service background task started.");

        var consumerConfig = GetConsumerConfig();
        var consumer = new ConsumerBuilder<byte[], byte[]>(consumerConfig)
            .SetPartitionsAssignedHandler((c, partitions) =>
            {
                var offsets = partitions.Select(tp => new TopicPartitionOffset(tp, Offset.Beginning));
                return offsets;
            })
            .Build();
        var topic = new KafkaTopic { Value = _envHelpers.GetEnvironmentVariableContent("KAFKA_KEY_VALUE_TOPIC") };
        consumer.Subscribe(topic.Value);

        // Don't check if topic has schemas defined for every invocation, jut do it once and use the appropriate handling method.
        // Note for future me: array[x..] is new more efficient dotnet syntax for skip first x and take rest of array
        Func<byte[], byte[]> handleSchemaMagicBytesInKey =
            TopicKeyHasSchema(topic)
                ? delegate(byte[] input) { return input[5..]; }
                : delegate(byte[] input) { return input; };
        // Func<byte[], byte[]> handleSchemaMagicBytesInKey =
        //     TopicKeyHasSchema(topic)
        //         ? (byte[] input) => input[5..]
        //         : (byte[] input) => input;
        Func<byte[], byte[]> handleSchemaMagicBytesInValue =
            TopicKeyHasSchema(topic)
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
                        var key = handleSchemaMagicBytesInKey(result.Message.Key);
                        _keyValueStateService.RemoveKey(key);
                    }
                    else
                    {
                        var key = handleSchemaMagicBytesInKey(result.Message.Key);
                        var value = handleSchemaMagicBytesInValue(result.Message.Value);
                        _keyValueStateService.AddKeyValuePair(key, value);
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

    private ConsumerConfig GetConsumerConfig()
    {
        //   KAFKA_BOOTSTRAP_SERVERS: "172.80.80.11:9092,172.80.80.12:9092,172.80.80.13:9092"
        //   KAFKA_TOPIC: "t.key-value-api"
        //   KAFKA_CONSUMER_GROUP: "cg.key-value-api.0"
        //   KAFKA_TRUST_STORE_PEM_LOCATION: "/kafka/secrets/CA_lokalmaskin.crt"
        //   KAFKA_CLIENT_CERTIFICATE_PEM_LOCATION: "/kafka/secrets/key-value-api.lokalmaskin.crt"
        //   KAFKA_CLIENT_KEY_PEM_LOCATION: "/kafka/secrets/key-value-api.lokalmaskin.key"
        //   KAFKA_CLIENT_KEY_PASSWORD_LOCATION: "/kafka/secrets/key-value-api.lokalmaskin.password.txt"
        var bootstrapServers = _envHelpers.GetEnvironmentVariableContent("KAFKA_BOOTSTRAP_SERVERS");
        var consumerGroup = _envHelpers.GetEnvironmentVariableContent("KAFKA_CONSUMER_GROUP");

        // var sslKeystoreLocation = Environment.GetEnvironmentVariable("TODO");
        // var sslKeystorePasswordLocation = Environment.GetEnvironmentVariable("TODO");

        var sslCaPem = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_TRUST_STORE_PEM_LOCATION");
        var sslCertificatePem = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_CLIENT_CERTIFICATE_PEM_LOCATION");
        var sslKeyPem = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_CLIENT_KEY_PEM_LOCATION");
        var sslKeyPassword = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_CLIENT_KEY_PASSWORD_LOCATION");

        var consumerConfig = new ConsumerConfig
        {
            // Connect to the SSL listener of the local platform
            BootstrapServers = bootstrapServers,

            // Set the security protocol to use SSL and certificate based authentication
            SecurityProtocol = SecurityProtocol.Ssl,

            // Ssl settings
            // Keystore files are used because Schema Registry client does not support PEM files
            // SslKeystoreLocation = $"{RootFolder}/certificates/example-consumer.p12",
            // SslKeystorePassword = "notsecret",
            // EnableSslCertificateVerification = false,
            // SslCaLocation = $"{RootFolder}/certificates/trustedCa.pem",
            SslCaPem = sslCaPem,
            SslCertificatePem = sslCertificatePem,
            SslKeyPem = sslKeyPem,
            SslKeyPassword = sslKeyPassword,

            // Specify the Kafka delivery settings
            GroupId = consumerGroup,
            EnableAutoCommit = true,
            AutoCommitIntervalMs = 200,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        return consumerConfig;
    }

    private bool TopicKeyHasSchema(KafkaTopic topic)
    {
        throw new NotImplementedException();
    }

    private bool TopicValueHasSchema(KafkaTopic topic)
    {
        throw new NotImplementedException();
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer received request for graceful shutdown.");

        await base.StopAsync(stoppingToken);
    }
}
