using Confluent.Kafka;
using Confluent.Kafka.Admin;

public class KafkaAdminClient
{
    private readonly ILogger<KafkaAdminClient> _logger;

    public KafkaAdminClient(ILogger<KafkaAdminClient> logger)
    {
        _logger = logger;
        logger.LogInformation($"{nameof(KafkaAdminClient)} initialized");
    }

    public async Task<bool> TryCreateTopics()
    {
        var adminClientConfig = KafkaAdminClientConfigGenerator.GetAdminClientConfig();
        var topic = Environment.GetEnvironmentVariable(KAFKA_KEY_VALUE_TOPIC);

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
