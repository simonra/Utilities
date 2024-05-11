public static class KafkaProducerConfigGenerator
{
    public static Confluent.Kafka.ProducerConfig GetProducerConfig()
    {
        var clientConfig = KafkaClientConfigGenerator.GetClientConfig();
        var producerConfig = new Confluent.Kafka.ProducerConfig(clientConfig);

        var BatchNumMessages = Environment.GetEnvironmentVariable(KAFKA_BATCH_NUM_MESSAGES);
        if(!string.IsNullOrEmpty(BatchNumMessages)) producerConfig.BatchNumMessages = int.Parse(BatchNumMessages);

        var CompressionType = Environment.GetEnvironmentVariable(KAFKA_COMPRESSION_TYPE);
        switch (CompressionType?.ToLowerInvariant())
        {
            case "gzip":
                producerConfig.CompressionType = Confluent.Kafka.CompressionType.Gzip;
                break;
            case "lz4":
                producerConfig.CompressionType = Confluent.Kafka.CompressionType.Lz4;
                break;
            case "none":
                producerConfig.CompressionType = Confluent.Kafka.CompressionType.None;
                break;
            case "snappy":
                producerConfig.CompressionType = Confluent.Kafka.CompressionType.Snappy;
                break;
            case "zstd":
                producerConfig.CompressionType = Confluent.Kafka.CompressionType.Zstd;
                break;
            default:
                break;
        }

        var QueueBufferingBackpressureThreshold = Environment.GetEnvironmentVariable(KAFKA_QUEUE_BUFFERING_BACKPRESSURE_THRESHOLD);
        if(!string.IsNullOrEmpty(QueueBufferingBackpressureThreshold)) producerConfig.QueueBufferingBackpressureThreshold = int.Parse(QueueBufferingBackpressureThreshold);

        var RetryBackoffMs = Environment.GetEnvironmentVariable(KAFKA_RETRY_BACKOFF_MS);
        if(!string.IsNullOrEmpty(RetryBackoffMs)) producerConfig.RetryBackoffMs = int.Parse(RetryBackoffMs);

        var MessageSendMaxRetries = Environment.GetEnvironmentVariable(KAFKA_MESSAGE_SEND_MAX_RETRIES);
        if(!string.IsNullOrEmpty(MessageSendMaxRetries)) producerConfig.MessageSendMaxRetries = int.Parse(MessageSendMaxRetries);

        var LingerMs = Environment.GetEnvironmentVariable(KAFKA_LINGER_MS);
        if(!string.IsNullOrEmpty(LingerMs)) producerConfig.LingerMs = double.Parse(LingerMs);

        var QueueBufferingMaxKbytes = Environment.GetEnvironmentVariable(KAFKA_QUEUE_BUFFERING_MAX_KBYTES);
        if(!string.IsNullOrEmpty(QueueBufferingMaxKbytes)) producerConfig.QueueBufferingMaxKbytes = int.Parse(QueueBufferingMaxKbytes);

        var QueueBufferingMaxMessages = Environment.GetEnvironmentVariable(KAFKA_QUEUE_BUFFERING_MAX_MESSAGES);
        if(!string.IsNullOrEmpty(QueueBufferingMaxMessages)) producerConfig.QueueBufferingMaxMessages = int.Parse(QueueBufferingMaxMessages);

        var EnableGaplessGuarantee = Environment.GetEnvironmentVariable(KAFKA_ENABLE_GAPLESS_GUARANTEE);
        switch (EnableGaplessGuarantee?.ToLowerInvariant())
        {
            case "true":
                producerConfig.EnableGaplessGuarantee = true;
                break;
            case "false":
                producerConfig.EnableGaplessGuarantee = false;
                break;
            default:
                break;
        }

        var EnableIdempotence = Environment.GetEnvironmentVariable(KAFKA_ENABLE_IDEMPOTENCE);
        switch (EnableIdempotence?.ToLowerInvariant())
        {
            case "true":
                producerConfig.EnableIdempotence = true;
                break;
            case "false":
                producerConfig.EnableIdempotence = false;
                break;
            default:
                break;
        }

        var TransactionTimeoutMs = Environment.GetEnvironmentVariable(KAFKA_TRANSACTION_TIMEOUT_MS);
        if(!string.IsNullOrEmpty(TransactionTimeoutMs)) producerConfig.TransactionTimeoutMs = int.Parse(TransactionTimeoutMs);

        var TransactionalId = Environment.GetEnvironmentVariable(KAFKA_TRANSACTIONAL_ID);
        if(!string.IsNullOrEmpty(TransactionalId)) producerConfig.TransactionalId = TransactionalId;

        var CompressionLevel = Environment.GetEnvironmentVariable(KAFKA_COMPRESSION_LEVEL);
        if(!string.IsNullOrEmpty(CompressionLevel)) producerConfig.CompressionLevel = int.Parse(CompressionLevel);

        var Partitioner = Environment.GetEnvironmentVariable(KAFKA_PARTITIONER);
        switch (Partitioner?.ToLowerInvariant())
        {
            case "consistent":
                producerConfig.Partitioner = Confluent.Kafka.Partitioner.Consistent;
                break;
            case "consistentrandom":
                producerConfig.Partitioner = Confluent.Kafka.Partitioner.ConsistentRandom;
                break;
            case "murmur2":
                producerConfig.Partitioner = Confluent.Kafka.Partitioner.Murmur2;
                break;
            case "murmur2random":
                producerConfig.Partitioner = Confluent.Kafka.Partitioner.Murmur2Random;
                break;
            case "random":
                producerConfig.Partitioner = Confluent.Kafka.Partitioner.Random;
                break;
            default:
                break;
        }

        var MessageTimeoutMs = Environment.GetEnvironmentVariable(KAFKA_MESSAGE_TIMEOUT_MS);
        if(!string.IsNullOrEmpty(MessageTimeoutMs)) producerConfig.MessageTimeoutMs = int.Parse(MessageTimeoutMs);

        var RequestTimeoutMs = Environment.GetEnvironmentVariable(KAFKA_REQUEST_TIMEOUT_MS);
        if(!string.IsNullOrEmpty(RequestTimeoutMs)) producerConfig.RequestTimeoutMs = int.Parse(RequestTimeoutMs);

        var DeliveryReportFields = Environment.GetEnvironmentVariable(KAFKA_DELIVERY_REPORT_FIELDS);
        if(!string.IsNullOrEmpty(DeliveryReportFields)) producerConfig.DeliveryReportFields = DeliveryReportFields;

        var EnableDeliveryReports = Environment.GetEnvironmentVariable(KAFKA_ENABLE_DELIVERY_REPORTS);
        switch (EnableDeliveryReports?.ToLowerInvariant())
        {
            case "true":
                producerConfig.EnableDeliveryReports = true;
                break;
            case "false":
                producerConfig.EnableDeliveryReports = false;
                break;
            default:
                break;
        }

        var EnableBackgroundPoll = Environment.GetEnvironmentVariable(KAFKA_ENABLE_BACKGROUND_POLL);
        switch (EnableBackgroundPoll?.ToLowerInvariant())
        {
            case "true":
                producerConfig.EnableBackgroundPoll = true;
                break;
            case "false":
                producerConfig.EnableBackgroundPoll = false;
                break;
            default:
                break;
        }

        var BatchSize = Environment.GetEnvironmentVariable(KAFKA_BATCH_SIZE);
        if(!string.IsNullOrEmpty(BatchSize)) producerConfig.BatchSize = int.Parse(BatchSize);

        var StickyPartitioningLingerMs = Environment.GetEnvironmentVariable(KAFKA_STICKY_PARTITIONING_LINGER_MS);
        if(!string.IsNullOrEmpty(StickyPartitioningLingerMs)) producerConfig.StickyPartitioningLingerMs = int.Parse(StickyPartitioningLingerMs);

        return producerConfig;
    }

    // Env var names
    public const string KAFKA_BATCH_NUM_MESSAGES = nameof(KAFKA_BATCH_NUM_MESSAGES);
    public const string KAFKA_COMPRESSION_TYPE = nameof(KAFKA_COMPRESSION_TYPE);
    public const string KAFKA_QUEUE_BUFFERING_BACKPRESSURE_THRESHOLD = nameof(KAFKA_QUEUE_BUFFERING_BACKPRESSURE_THRESHOLD);
    public const string KAFKA_RETRY_BACKOFF_MS = nameof(KAFKA_RETRY_BACKOFF_MS);
    public const string KAFKA_MESSAGE_SEND_MAX_RETRIES = nameof(KAFKA_MESSAGE_SEND_MAX_RETRIES);
    public const string KAFKA_LINGER_MS = nameof(KAFKA_LINGER_MS);
    public const string KAFKA_QUEUE_BUFFERING_MAX_KBYTES = nameof(KAFKA_QUEUE_BUFFERING_MAX_KBYTES);
    public const string KAFKA_QUEUE_BUFFERING_MAX_MESSAGES = nameof(KAFKA_QUEUE_BUFFERING_MAX_MESSAGES);
    public const string KAFKA_ENABLE_GAPLESS_GUARANTEE = nameof(KAFKA_ENABLE_GAPLESS_GUARANTEE);
    public const string KAFKA_ENABLE_IDEMPOTENCE = nameof(KAFKA_ENABLE_IDEMPOTENCE);
    public const string KAFKA_TRANSACTION_TIMEOUT_MS = nameof(KAFKA_TRANSACTION_TIMEOUT_MS);
    public const string KAFKA_TRANSACTIONAL_ID = nameof(KAFKA_TRANSACTIONAL_ID);
    public const string KAFKA_COMPRESSION_LEVEL = nameof(KAFKA_COMPRESSION_LEVEL);
    public const string KAFKA_PARTITIONER = nameof(KAFKA_PARTITIONER);
    public const string KAFKA_MESSAGE_TIMEOUT_MS = nameof(KAFKA_MESSAGE_TIMEOUT_MS);
    public const string KAFKA_REQUEST_TIMEOUT_MS = nameof(KAFKA_REQUEST_TIMEOUT_MS);
    public const string KAFKA_DELIVERY_REPORT_FIELDS = nameof(KAFKA_DELIVERY_REPORT_FIELDS);
    public const string KAFKA_ENABLE_DELIVERY_REPORTS = nameof(KAFKA_ENABLE_DELIVERY_REPORTS);
    public const string KAFKA_ENABLE_BACKGROUND_POLL = nameof(KAFKA_ENABLE_BACKGROUND_POLL);
    public const string KAFKA_BATCH_SIZE = nameof(KAFKA_BATCH_SIZE);
    public const string KAFKA_STICKY_PARTITIONING_LINGER_MS = nameof(KAFKA_STICKY_PARTITIONING_LINGER_MS);
}

