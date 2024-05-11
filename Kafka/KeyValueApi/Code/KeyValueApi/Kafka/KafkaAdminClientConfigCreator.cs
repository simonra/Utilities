public static class KafkaAdminClientConfigCreator
{
    public static Confluent.Kafka.AdminClientConfig GetAdminClientConfig()
    {
        var clientConfig = KafkaClientConfigCreator.GetClientConfig();
        var adminClientConfig = new Confluent.Kafka.AdminClientConfig(clientConfig);
        return adminClientConfig;
    }
}
