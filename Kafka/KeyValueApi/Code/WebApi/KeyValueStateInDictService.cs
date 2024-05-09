using static EnvVarNames;

public class KeyValueStateInDictService : IKeyValueStateService
{
    private readonly ILogger<KeyValueStateInDictService> _logger;
    private readonly Dictionary<byte[], List<KeyValue>> keyValueState;
    private readonly Func<byte[], byte[]> _encrypt;
    private readonly Func<byte[], byte[]> _decrypt;
    // private readonly ConsumerConfig _consumerConfig;
    // private readonly ProducerConfig _producerConfig;
    // private System.IO.Hashing.Crc32 crc32; // https://learn.microsoft.com/en-us/dotnet/api/system.io.hashing.crc32?view=dotnet-plat-ext-8.0

    public KeyValueStateInDictService(ILogger<KeyValueStateInDictService> logger)
    {
        _logger = logger;
        keyValueState = new Dictionary<byte[], List<KeyValue>>();
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
    }

    public bool Store(byte[] inputKey, byte[] inputValue)
    {
        var key = _encrypt(inputKey);
        var value = _encrypt(inputValue);
        var keyHash = System.IO.Hashing.Crc32.Hash(key);
        if(keyValueState.ContainsKey(keyHash))
        {
            for (int i = 0; i < keyValueState[keyHash].Count; i++)
            {
                if(keyValueState[keyHash][i].Key.SequenceEqual(key))
                {
                    keyValueState[keyHash][i] =  new KeyValue { Key = key, Value = value };
                    return true;
                }
            }
            // This is an actual hash collision
            keyValueState[keyHash].Add(new KeyValue { Key = key, Value = value });
            return true;
        }
        else
        {
            keyValueState.Add(keyHash, new List<KeyValue> { new KeyValue { Key = key, Value = value } });
            return true;
        }
    }

    public bool TryRetrieve(byte[] inputKey, out byte[] value)
    {
        var key = _encrypt(inputKey);
        var keyHash = System.IO.Hashing.Crc32.Hash(key);
        if(keyValueState.ContainsKey(keyHash))
        {
            for (int i = 0; i < keyValueState[keyHash].Count; i++)
            {
                if(keyValueState[keyHash][i].Key.SequenceEqual(key))
                {
                    value = _decrypt(keyValueState[keyHash][i].Value);
                    return true;
                }
            }
        }
        value = [];
        return false;
    }

    public bool Remove(byte[] inputKey)
    {
        var key = _encrypt(inputKey);
        var keyHash = System.IO.Hashing.Crc32.Hash(key);
        if(keyValueState.ContainsKey(keyHash))
        {
            for (int i = 0; i < keyValueState[keyHash].Count; i++)
            {
                if(keyValueState[keyHash][i].Key.SequenceEqual(key))
                {
                    if(keyValueState[keyHash].Count == 1)
                    {
                        keyValueState.Remove(keyHash);
                    }
                    else
                    {
                        keyValueState[keyHash].RemoveAt(i);
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
}
