public static class KafkaConsumerConfigGenerator
{
    public static Confluent.Kafka.ConsumerConfig GetConsumerConfig()
    {
        var clientConfig = KafkaClientConfigGenerator.GetClientConfig();
        var consumerConfig = new Confluent.Kafka.ConsumerConfig(clientConfig);

        var IsolationLevel = Environment.GetEnvironmentVariable(KAFKA_ISOLATION_LEVEL);
        switch (IsolationLevel?.ToLowerInvariant())
        {
            case "readcommitted":
                consumerConfig.IsolationLevel = Confluent.Kafka.IsolationLevel.ReadCommitted;
                break;
            case "readuncommitted":
                consumerConfig.IsolationLevel = Confluent.Kafka.IsolationLevel.ReadUncommitted;
                break;
            default:
                break;
        }

        var FetchErrorBackoffMs = Environment.GetEnvironmentVariable(KAFKA_FETCH_ERROR_BACKOFF_MS);
        if(!string.IsNullOrEmpty(FetchErrorBackoffMs)) consumerConfig.FetchErrorBackoffMs = int.Parse(FetchErrorBackoffMs);

        var FetchMinBytes = Environment.GetEnvironmentVariable(KAFKA_FETCH_MIN_BYTES);
        if(!string.IsNullOrEmpty(FetchMinBytes)) consumerConfig.FetchMinBytes = int.Parse(FetchMinBytes);

        var FetchMaxBytes = Environment.GetEnvironmentVariable(KAFKA_FETCH_MAX_BYTES);
        if(!string.IsNullOrEmpty(FetchMaxBytes)) consumerConfig.FetchMaxBytes = int.Parse(FetchMaxBytes);

        var MaxPartitionFetchBytes = Environment.GetEnvironmentVariable(KAFKA_MAX_PARTITION_FETCH_BYTES);
        if(!string.IsNullOrEmpty(MaxPartitionFetchBytes)) consumerConfig.MaxPartitionFetchBytes = int.Parse(MaxPartitionFetchBytes);

        var FetchQueueBackoffMs = Environment.GetEnvironmentVariable(KAFKA_FETCH_QUEUE_BACKOFF_MS);
        if(!string.IsNullOrEmpty(FetchQueueBackoffMs)) consumerConfig.FetchQueueBackoffMs = int.Parse(FetchQueueBackoffMs);

        var FetchWaitMaxMs = Environment.GetEnvironmentVariable(KAFKA_FETCH_WAIT_MAX_MS);
        if(!string.IsNullOrEmpty(FetchWaitMaxMs)) consumerConfig.FetchWaitMaxMs = int.Parse(FetchWaitMaxMs);

        var QueuedMaxMessagesKbytes = Environment.GetEnvironmentVariable(KAFKA_QUEUED_MAX_MESSAGES_KBYTES);
        if(!string.IsNullOrEmpty(QueuedMaxMessagesKbytes)) consumerConfig.QueuedMaxMessagesKbytes = int.Parse(QueuedMaxMessagesKbytes);

        var QueuedMinMessages = Environment.GetEnvironmentVariable(KAFKA_QUEUED_MIN_MESSAGES);
        if(!string.IsNullOrEmpty(QueuedMinMessages)) consumerConfig.QueuedMinMessages = int.Parse(QueuedMinMessages);

        var EnableAutoOffsetStore = Environment.GetEnvironmentVariable(KAFKA_ENABLE_AUTO_OFFSET_STORE);
        switch (EnableAutoOffsetStore?.ToLowerInvariant())
        {
            case "true":
                consumerConfig.EnableAutoOffsetStore = true;
                break;
            case "false":
                consumerConfig.EnableAutoOffsetStore = false;
                break;
            default:
                break;
        }

        var AutoCommitIntervalMs = Environment.GetEnvironmentVariable(KAFKA_AUTO_COMMIT_INTERVAL_MS);
        if(!string.IsNullOrEmpty(AutoCommitIntervalMs)) consumerConfig.AutoCommitIntervalMs = int.Parse(AutoCommitIntervalMs);

        var MaxPollIntervalMs = Environment.GetEnvironmentVariable(KAFKA_MAX_POLL_INTERVAL_MS);
        if(!string.IsNullOrEmpty(MaxPollIntervalMs)) consumerConfig.MaxPollIntervalMs = int.Parse(MaxPollIntervalMs);

        var EnablePartitionEof = Environment.GetEnvironmentVariable(KAFKA_ENABLE_PARTITION_EOF);
        switch (EnablePartitionEof?.ToLowerInvariant())
        {
            case "true":
                consumerConfig.EnablePartitionEof = true;
                break;
            case "false":
                consumerConfig.EnablePartitionEof = false;
                break;
            default:
                break;
        }

        var CoordinatorQueryIntervalMs = Environment.GetEnvironmentVariable(KAFKA_COORDINATOR_QUERY_INTERVAL_MS);
        if(!string.IsNullOrEmpty(CoordinatorQueryIntervalMs)) consumerConfig.CoordinatorQueryIntervalMs = int.Parse(CoordinatorQueryIntervalMs);

        var GroupProtocolType = Environment.GetEnvironmentVariable(KAFKA_GROUP_PROTOCOL_TYPE);
        if(!string.IsNullOrEmpty(GroupProtocolType)) consumerConfig.GroupProtocolType = GroupProtocolType;

        var HeartbeatIntervalMs = Environment.GetEnvironmentVariable(KAFKA_HEARTBEAT_INTERVAL_MS);
        if(!string.IsNullOrEmpty(HeartbeatIntervalMs)) consumerConfig.HeartbeatIntervalMs = int.Parse(HeartbeatIntervalMs);

        var SessionTimeoutMs = Environment.GetEnvironmentVariable(KAFKA_SESSION_TIMEOUT_MS);
        if(!string.IsNullOrEmpty(SessionTimeoutMs)) consumerConfig.SessionTimeoutMs = int.Parse(SessionTimeoutMs);

        var PartitionAssignmentStrategy = Environment.GetEnvironmentVariable(KAFKA_PARTITION_ASSIGNMENT_STRATEGY);
        switch (PartitionAssignmentStrategy?.ToLowerInvariant())
        {
            case "cooperativesticky":
                consumerConfig.PartitionAssignmentStrategy = Confluent.Kafka.PartitionAssignmentStrategy.CooperativeSticky;
                break;
            case "range":
                consumerConfig.PartitionAssignmentStrategy = Confluent.Kafka.PartitionAssignmentStrategy.Range;
                break;
            case "roundrobin":
                consumerConfig.PartitionAssignmentStrategy = Confluent.Kafka.PartitionAssignmentStrategy.RoundRobin;
                break;
            default:
                break;
        }

        var GroupInstanceId = Environment.GetEnvironmentVariable(KAFKA_GROUP_INSTANCE_ID);
        if(!string.IsNullOrEmpty(GroupInstanceId)) consumerConfig.GroupInstanceId = GroupInstanceId;

        var GroupId = Environment.GetEnvironmentVariable(KAFKA_GROUP_ID);
        if(!string.IsNullOrEmpty(GroupId)) consumerConfig.GroupId = GroupId;

        var AutoOffsetReset = Environment.GetEnvironmentVariable(KAFKA_AUTO_OFFSET_RESET);
        switch (AutoOffsetReset?.ToLowerInvariant())
        {
            case "earliest":
                consumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                break;
            case "error":
                consumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Error;
                break;
            case "latest":
                consumerConfig.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Latest;
                break;
            default:
                break;
        }

        var ConsumeResultFields = Environment.GetEnvironmentVariable(KAFKA_CONSUME_RESULT_FIELDS);
        if(!string.IsNullOrEmpty(ConsumeResultFields)) consumerConfig.ConsumeResultFields = ConsumeResultFields;

        var EnableAutoCommit = Environment.GetEnvironmentVariable(KAFKA_ENABLE_AUTO_COMMIT);
        switch (EnableAutoCommit?.ToLowerInvariant())
        {
            case "true":
                consumerConfig.EnableAutoCommit = true;
                break;
            case "false":
                consumerConfig.EnableAutoCommit = false;
                break;
            default:
                break;
        }

        var CheckCrcs = Environment.GetEnvironmentVariable(KAFKA_CHECK_CRCS);
        switch (CheckCrcs?.ToLowerInvariant())
        {
            case "true":
                consumerConfig.CheckCrcs = true;
                break;
            case "false":
                consumerConfig.CheckCrcs = false;
                break;
            default:
                break;
        }

        return consumerConfig;
    }

    public const string KAFKA_ISOLATION_LEVEL = nameof(KAFKA_ISOLATION_LEVEL);
    public const string KAFKA_FETCH_ERROR_BACKOFF_MS = nameof(KAFKA_FETCH_ERROR_BACKOFF_MS);
    public const string KAFKA_FETCH_MIN_BYTES = nameof(KAFKA_FETCH_MIN_BYTES);
    public const string KAFKA_FETCH_MAX_BYTES = nameof(KAFKA_FETCH_MAX_BYTES);
    public const string KAFKA_MAX_PARTITION_FETCH_BYTES = nameof(KAFKA_MAX_PARTITION_FETCH_BYTES);
    public const string KAFKA_FETCH_QUEUE_BACKOFF_MS = nameof(KAFKA_FETCH_QUEUE_BACKOFF_MS);
    public const string KAFKA_FETCH_WAIT_MAX_MS = nameof(KAFKA_FETCH_WAIT_MAX_MS);
    public const string KAFKA_QUEUED_MAX_MESSAGES_KBYTES = nameof(KAFKA_QUEUED_MAX_MESSAGES_KBYTES);
    public const string KAFKA_QUEUED_MIN_MESSAGES = nameof(KAFKA_QUEUED_MIN_MESSAGES);
    public const string KAFKA_ENABLE_AUTO_OFFSET_STORE = nameof(KAFKA_ENABLE_AUTO_OFFSET_STORE);
    public const string KAFKA_AUTO_COMMIT_INTERVAL_MS = nameof(KAFKA_AUTO_COMMIT_INTERVAL_MS);
    public const string KAFKA_MAX_POLL_INTERVAL_MS = nameof(KAFKA_MAX_POLL_INTERVAL_MS);
    public const string KAFKA_ENABLE_PARTITION_EOF = nameof(KAFKA_ENABLE_PARTITION_EOF);
    public const string KAFKA_COORDINATOR_QUERY_INTERVAL_MS = nameof(KAFKA_COORDINATOR_QUERY_INTERVAL_MS);
    public const string KAFKA_GROUP_PROTOCOL_TYPE = nameof(KAFKA_GROUP_PROTOCOL_TYPE);
    public const string KAFKA_HEARTBEAT_INTERVAL_MS = nameof(KAFKA_HEARTBEAT_INTERVAL_MS);
    public const string KAFKA_SESSION_TIMEOUT_MS = nameof(KAFKA_SESSION_TIMEOUT_MS);
    public const string KAFKA_PARTITION_ASSIGNMENT_STRATEGY = nameof(KAFKA_PARTITION_ASSIGNMENT_STRATEGY);
    public const string KAFKA_GROUP_INSTANCE_ID = nameof(KAFKA_GROUP_INSTANCE_ID);
    public const string KAFKA_GROUP_ID = nameof(KAFKA_GROUP_ID);
    public const string KAFKA_AUTO_OFFSET_RESET = nameof(KAFKA_AUTO_OFFSET_RESET);
    public const string KAFKA_CONSUME_RESULT_FIELDS = nameof(KAFKA_CONSUME_RESULT_FIELDS);
    public const string KAFKA_ENABLE_AUTO_COMMIT = nameof(KAFKA_ENABLE_AUTO_COMMIT);
    public const string KAFKA_CHECK_CRCS = nameof(KAFKA_CHECK_CRCS);
}
