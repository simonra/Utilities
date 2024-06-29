public class KeyValueStateInDictService : IKeyValueStateService
{
    private readonly ILogger<KeyValueStateInDictService> _logger;
    private readonly Dictionary<string, List<KeyValue>> _keyValueState;
    // private readonly ConsumerConfig _consumerConfig;
    // private readonly ProducerConfig _producerConfig;
    // private System.IO.Hashing.Crc32 crc32; // https://learn.microsoft.com/en-us/dotnet/api/system.io.hashing.crc32?view=dotnet-plat-ext-8.0

    public KeyValueStateInDictService(ILogger<KeyValueStateInDictService> logger)
    {
        _logger = logger;
        _keyValueState = new Dictionary<string, List<KeyValue>>();
        // _consumerConfig = GetConsumerConfig();
        // crc32 = new();

        // resources todo
        // https://docs.axual.io/axual/2022.1/getting_started/consumer/dotnet/dotnet-kafka-client-consumer.html
        // https://docs.axual.io/axual/2022.1/getting_started/producer/dotnet/dotnet-kafka-client-producer.html
        // https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.IConsumer-2.html

        _logger.LogInformation($"{nameof(KeyValueStateInDictService)} initialized");
    }

    public bool Store(byte[] key, byte[] value)
    {
        _logger.LogDebug($"Storing key {key}");
        var keyHash = key.GetHashString();
        if(_keyValueState.TryGetValue(keyHash, out List<KeyValue>? pairsSharingHash))
        {
            for (int i = 0; i < pairsSharingHash.Count; i++)
            {
                if(pairsSharingHash[i].Key.SequenceEqual(key))
                {
                    pairsSharingHash[i] =  new KeyValue { Key = key, Value = value };
                    _keyValueState[keyHash] = pairsSharingHash;
                    return true;
                }
            }
            pairsSharingHash.Add(new KeyValue { Key = key, Value = value });
            _keyValueState[keyHash] = pairsSharingHash;
            return true;
        }
        else
        {
            _keyValueState.Add(keyHash, [new() { Key = key, Value = value }]);
            return true;
        }
    }

    public bool TryRetrieve(byte[] keyRaw, out byte[] value)
    {
        var keyHash = keyRaw.GetHashString();
        if(_keyValueState.TryGetValue(keyHash, out List<KeyValue>? pairsSharingHash))
        {
            for (int i = 0; i < pairsSharingHash.Count; i++)
            {
                if(pairsSharingHash[i].Key.SequenceEqual(keyRaw))
                {
                    value = pairsSharingHash[i].Value;
                    return true;
                }
            }
        }
        value = [];
        return false;
    }

    public bool Remove(byte[] keyRaw)
    {
        var keyHash = keyRaw.GetHashString();
        if(_keyValueState.TryGetValue(keyHash, out List<KeyValue>? pairsSharingHash))
        {
            for (int i = 0; i < pairsSharingHash.Count; i++)
            {
                if(pairsSharingHash[i].Key.SequenceEqual(keyRaw))
                {
                    if(pairsSharingHash.Count == 1)
                    {
                        _keyValueState.Remove(keyHash);
                    }
                    else
                    {
                        _keyValueState[keyHash].RemoveAt(i);
                    }
                    return true;
                }
            }
            // This is an actual hash collision, but the key is not present, so there is nothing to remove, noop
            return true;
        }
        else
        {
            return true;
        }
    }

    public List<KafkaTopicPartitionOffset> GetLastConsumedTopicPartitionOffsets()
    {
        return [];
    }

    public bool UpdateLastConsumedTopicPartitionOffsets(KafkaTopicPartitionOffset topicPartitionOffsets)
    {
        return false;
    }
}
