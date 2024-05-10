using Confluent.Kafka;
using Confluent.Kafka.Admin;

public class KafkaAdminClient
{
    private readonly EnvHelpers _envHelpers;
    private readonly ILogger<KafkaAdminClient> _logger;

    public KafkaAdminClient(EnvHelpers envHelpers, ILogger<KafkaAdminClient> logger)
    {
        _envHelpers = envHelpers;
        _logger = logger;
        logger.LogInformation($"{nameof(KafkaAdminClient)} initialized");
    }

    public async Task<bool> TryCreateTopics()
    {

        var bootstrapServers = _envHelpers.GetEnvironmentVariableContent(KAFKA_BOOTSTRAP_SERVERS);
        var consumerGroup = _envHelpers.GetEnvironmentVariableContent(KAFKA_CONSUMER_GROUP);

        var sslCaPem = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText(KAFKA_TRUST_STORE_PEM_LOCATION);
        var sslCertificatePem = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText(KAFKA_CLIENT_CERTIFICATE_PEM_LOCATION);
        var sslKeyPem = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText(KAFKA_CLIENT_KEY_PEM_LOCATION);
        var sslKeyPassword = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText(KAFKA_CLIENT_KEY_PASSWORD_LOCATION);

        var topic = _envHelpers.GetEnvironmentVariableContent(KAFKA_KEY_VALUE_TOPIC);

        var adminClientConfig = new AdminClientConfig
        {
            // Connect to the SSL listener of the local platform
            BootstrapServers = bootstrapServers,

            // Set the security protocol to use SSL and certificate based authentication
            SecurityProtocol = SecurityProtocol.Ssl,

            SslCaPem = sslCaPem,
            SslCertificatePem = sslCertificatePem,
            SslKeyPem = sslKeyPem,
            SslKeyPassword = sslKeyPassword
        };

        using (var adminClient = new AdminClientBuilder(adminClientConfig).Build())
        {
            try
            {
                // List<DescribeConfigsResult> preExistingConfigs = await adminClient.DescribeConfigsAsync(
                //     [
                //         new ConfigResource
                //         {
                //             Name = topic,
                //             Type = ResourceType.Topic,
                //         }
                //     ]
                // );
                // if(preExistingConfigs.Any())
                // {
                //     return false;
                // }
                _logger.LogInformation($"Asking admin client to create topic {topic}");
                await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                    new TopicSpecification
                    {
                         Name = topic,
                         ReplicationFactor = -1,
                         NumPartitions = 1,
                         Configs = new Dictionary<string, string>
                         {
                            { "cleanup.policy", "compact" },
                            { "retention.bytes", "-1" },
                            { "retention.ms", "-1" },
                         }
                    }
                });
                _logger.LogInformation($"Admin client done creating topic {topic}");
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An error occurred creating topic");
            }
        }

        return false;
    }

}
