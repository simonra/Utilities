public static class KafkaAdminClientConfigGenerator
{
    public static Confluent.Kafka.AdminClientConfig GetAdminClientConfig()
    {
        var clientConfig = KafkaClientConfigGenerator.GetClientConfig();
        var adminClientConfig = new Confluent.Kafka.AdminClientConfig(clientConfig);
        return adminClientConfig;
    }
}
