using System.Collections.Generic;
using System.Linq;

public class KeyValueStateService
{
    private readonly ILogger<KeyValueStateService> _logger;
    private readonly Dictionary<byte[], List<KeyValue>> keyValueState;
    // private readonly ConsumerConfig _consumerConfig;
    // private readonly ProducerConfig _producerConfig;
    // private System.IO.Hashing.Crc32 crc32; // https://learn.microsoft.com/en-us/dotnet/api/system.io.hashing.crc32?view=dotnet-plat-ext-8.0

    public KeyValueStateService(ILogger<KeyValueStateService> logger)
    {
        _logger = logger;
        keyValueState = new Dictionary<byte[], List<KeyValue>>();
        // _consumerConfig = GetConsumerConfig();
        // crc32 = new();

        // resources todo
        // https://docs.axual.io/axual/2022.1/getting_started/consumer/dotnet/dotnet-kafka-client-consumer.html
        // https://docs.axual.io/axual/2022.1/getting_started/producer/dotnet/dotnet-kafka-client-producer.html
        // https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.IConsumer-2.html
    }

    public byte[]? GetValue(byte[] key)
    {
        var keyHash = System.IO.Hashing.Crc32.Hash(key);
        if(keyValueState.ContainsKey(keyHash))
        {
            for (int i = 0; i < keyValueState[keyHash].Count; i++)
            {
                if(keyValueState[keyHash][i].Key.SequenceEqual(key))
                {
                    return keyValueState[keyHash][i].Value;
                }
            }
        }
        return null;
    }

    public bool AddKeyValuePair(byte[] key, byte[] value)
    {
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

    public bool RemoveKey(byte[] key)
    {
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

    // public bool SetValue(string key, string value)
    // {
    //     // todo produce event
    //     // if successfull return success
    //     // No, this should be done using separate producer service
    // }

    // private async Task FeedKeyValueState()
    // {
    //     var config = new ConsumerConfig
    //     {
    //         BootstrapServers = "host1:9092,host2:9092",
    //         GroupId = "foo",
    //         AutoOffsetReset = AutoOffsetReset.Earliest
    //     };
    //     using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
    //     {
    //         consumer.Subscribe(topics);

    //         while (!cancelled)
    //         {
    //             var consumeResult = consumer.Consume(cancellationToken);

    //             // handle consumed message.
    //             // ...
    //         }

    //         consumer.Close();
    //     }

    // }

    // private ConsumerConfig GetConsumerConfig()
    // {
    //     //   KAFKA_BOOTSTRAP_SERVERS: "172.80.80.11:9092,172.80.80.12:9092,172.80.80.13:9092"
    //     //   KAFKA_TOPIC: "t.key-value-api"
    //     //   KAFKA_CONSUMER_GROUP: "cg.key-value-api.0"
    //     //   KAFKA_TRUST_STORE_PEM_LOCATION: "/kafka/secrets/CA_lokalmaskin.crt"
    //     //   KAFKA_CLIENT_CERTIFICATE_PEM_LOCATION: "/kafka/secrets/key-value-api.lokalmaskin.crt"
    //     //   KAFKA_CLIENT_KEY_PEM_LOCATION: "/kafka/secrets/key-value-api.lokalmaskin.key"
    //     //   KAFKA_CLIENT_KEY_PASSWORD_LOCATION: "/kafka/secrets/key-value-api.lokalmaskin.password.txt"
    //     var bootstrapServers = GetEnvironmentVariableContent("KAFKA_BOOTSTRAP_SERVERS");
    //     var consumerGroup = GetEnvironmentVariableContent("KAFKA_CONSUMER_GROUP");

    //     // var sslKeystoreLocation = Environment.GetEnvironmentVariable("TODO");
    //     // var sslKeystorePasswordLocation = Environment.GetEnvironmentVariable("TODO");

    //     var sslCaPem = GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_TRUST_STORE_PEM_LOCATION");
    //     var sslCertificatePem = GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_CLIENT_CERTIFICATE_PEM_LOCATION");
    //     var sslKeyPem = GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_CLIENT_KEY_PEM_LOCATION");
    //     var sslKeyPassword = GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_CLIENT_KEY_PASSWORD_LOCATION");

    //     var consumerConfig = new ConsumerConfig
    //     {
    //         // Connect to the SSL listener of the local platform
    //         BootstrapServers = bootstrapServers,

    //         // Set the security protocol to use SSL and certificate based authentication
    //         SecurityProtocol = SecurityProtocol.Ssl,

    //         // Ssl settings
    //         // Keystore files are used because Schema Registry client does not support PEM files
    //         // SslKeystoreLocation = $"{RootFolder}/certificates/example-consumer.p12",
    //         // SslKeystorePassword = "notsecret",
    //         // EnableSslCertificateVerification = false,
    //         // SslCaLocation = $"{RootFolder}/certificates/trustedCa.pem",
    //         SslCaPem = SslCaPem,
    //         SslCertificatePem = sslCertificatePem,
    //         SslKeyPem = sslKeyPem,
    //         SslKeyPassword = sslKeyPassword,

    //         // Specify the Kafka delivery settings
    //         GroupId = consumerGroup,
    //         EnableAutoCommit = true,
    //         AutoCommitIntervalMs = 200,
    //         AutoOffsetReset = AutoOffsetReset.Earliest
    //     };

    //     return consumerConfig;
    // }

    // private ProducerConfig GetProducerConfig()
    // {
    //     var bootstrapServers = GetEnvironmentVariableContent("KAFKA_BOOTSTRAP_SERVERS");
    //     // var consumerGroup = GetEnvironmentVariableContent("KAFKA_CONSUMER_GROUP");

    //     // var sslKeystoreLocation = Environment.GetEnvironmentVariable("TODO");
    //     // var sslKeystorePasswordLocation = Environment.GetEnvironmentVariable("TODO");

    //     var sslCaPem = GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_TRUST_STORE_PEM_LOCATION");
    //     var sslCertificatePem = GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_CLIENT_CERTIFICATE_PEM_LOCATION");
    //     var sslKeyPem = GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_CLIENT_KEY_PEM_LOCATION");
    //     var sslKeyPassword = GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_CLIENT_KEY_PASSWORD_LOCATION");

    //     var producerConfig = new ProducerConfig()
    //     {
    //         // Connect to the SSL listener of the local platform
    //         BootstrapServers = bootstrapServers,

    //         // Set the security protocol to use SSL and certificate based authentication
    //         SecurityProtocol = SecurityProtocol.Ssl,

    //         // Ssl settings
    //         SslCaPem = SslCaPem,
    //         SslCertificatePem = sslCertificatePem,
    //         SslKeyPem = sslKeyPem,
    //         SslKeyPassword = sslKeyPassword,

    //         // Specify the Kafka delivery settings
    //         Acks = Acks.All,
    //         LingerMs = 10,

    //         //Debug = "all"
    //     };

    //     return producerConfig;
    // }
}
