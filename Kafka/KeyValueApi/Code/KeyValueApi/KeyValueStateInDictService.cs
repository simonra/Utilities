public class KeyValueStateInDictService : IKeyValueStateService
{
    private readonly ILogger<KeyValueStateInDictService> _logger;
    private readonly Dictionary<string, List<KeyValue>> _keyValueState;
    private readonly Func<byte[], byte[]> _encrypt;
    private readonly Func<byte[], byte[]> _decrypt;
    // private readonly ConsumerConfig _consumerConfig;
    // private readonly ProducerConfig _producerConfig;
    // private System.IO.Hashing.Crc32 crc32; // https://learn.microsoft.com/en-us/dotnet/api/system.io.hashing.crc32?view=dotnet-plat-ext-8.0

    public KeyValueStateInDictService(ILogger<KeyValueStateInDictService> logger)
    {
        _logger = logger;
        _keyValueState = new Dictionary<string, List<KeyValue>>();
        if (Environment.GetEnvironmentVariable(KV_API_ENCRYPT_DATA_IN_STATE_STORAGE) == "true")
        {
            // Why would you encrypt memory while the key has to reside plaintext in memory anyways?
            // Who knows, gives the good feeling of being able to say "it's encrypted while it's here" I guess.
            // I think I'm adding it for the feeling of completeness. But for practical use I'd probably remove
            // it from the in memory/dict implementation for the performance improvement.
            var cryptoService = new CryptoService();
            _encrypt = cryptoService.Encrypt;
            _decrypt = cryptoService.Decrypt;
        }
        else
        {
            _encrypt = delegate(byte[] input) { return input; };
            _decrypt = delegate(byte[] input) { return input; };
        }
        // _consumerConfig = GetConsumerConfig();
        // crc32 = new();

        // resources todo
        // https://docs.axual.io/axual/2022.1/getting_started/consumer/dotnet/dotnet-kafka-client-consumer.html
        // https://docs.axual.io/axual/2022.1/getting_started/producer/dotnet/dotnet-kafka-client-producer.html
        // https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.IConsumer-2.html

        _logger.LogInformation($"{nameof(KeyValueStateInDictService)} initialized");
    }

    public bool Store(byte[] keyRaw, byte[] valueRaw)
    {
        _logger.LogInformation($"Storing key {keyRaw}");
        var keyEncrypted = _encrypt(keyRaw);
        var valueEncrypted = _encrypt(valueRaw);
        var keyHash = keyRaw.GetHashString();
        if(_keyValueState.TryGetValue(keyHash, out List<KeyValue>? pairsSharingHash))
        {
            for (int i = 0; i < pairsSharingHash.Count; i++)
            {
                if(_decrypt(pairsSharingHash[i].Key).SequenceEqual(keyRaw))
                {
                    pairsSharingHash[i] =  new KeyValue { Key = keyEncrypted, Value = valueEncrypted };
                    _keyValueState[keyHash] = pairsSharingHash;
                    return true;
                }
            }
            pairsSharingHash.Add(new KeyValue { Key = keyEncrypted, Value = valueEncrypted });
            _keyValueState[keyHash] = pairsSharingHash;
            return true;
        }
        else
        {
            _keyValueState.Add(keyHash, [new() { Key = keyEncrypted, Value = valueEncrypted }]);
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
                if(_decrypt(pairsSharingHash[i].Key).SequenceEqual(keyRaw))
                {
                    value = _decrypt(pairsSharingHash[i].Value);
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
                if(_decrypt(pairsSharingHash[i].Key).SequenceEqual(keyRaw))
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
