using Confluent.Kafka;

public class KafkaProducerService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly EnvHelpers _envHelpers;
    private readonly IProducer<byte[], byte[]?> _producer;
    private readonly KafkaTopic _topic;
    private readonly Func<byte[], byte[]> _encrypt;
    private readonly Func<string, string> _encryptHeaderKey;

    public KafkaProducerService(ILogger<KafkaConsumerService> logger, EnvHelpers envHelpers)
    {
        _logger = logger;
        _envHelpers = envHelpers;
        if (_envHelpers.GetEnvironmentVariableContent(KV_API_ENCRYPT_DATA_ON_KAFKA) == "true")
        {
            var cryptoService = new CryptoService();
            _encrypt = cryptoService.Encrypt;
            _encryptHeaderKey = delegate(string input) { return Convert.ToBase64String(cryptoService.Encrypt(System.Text.Encoding.UTF8.GetBytes(input))); };
        }
        else
        {
            _encrypt = delegate(byte[] input) { return input; };
            _encryptHeaderKey = delegate(string input) { return input; };
        }
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        var config = KafkaProducerConfigCreator.GetProducerConfig();
        _producer = new ProducerBuilder<byte[], byte[]?>(config).Build();
        _topic = new KafkaTopic { Value = _envHelpers.GetEnvironmentVariableContent(KAFKA_KEY_VALUE_TOPIC) };
        _logger.LogInformation($"{nameof(KafkaProducerService)} initialized");
    }

    // private ProducerConfig GetProducerConfig()
    // {
    //     var bootstrapServers = _envHelpers.GetEnvironmentVariableContent(KAFKA_BOOTSTRAP_SERVERS);

    //     var sslCaPem = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText(KAFKA_TRUST_STORE_PEM_LOCATION);
    //     var sslCertificatePem = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText(KAFKA_CLIENT_CERTIFICATE_PEM_LOCATION);
    //     var sslKeyPem = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText(KAFKA_CLIENT_KEY_PEM_LOCATION);
    //     var sslKeyPassword = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText(KAFKA_CLIENT_KEY_PASSWORD_LOCATION);

    //     var producerConfig = new ProducerConfig()
    //     {
    //         // Connect to the SSL listener of the local platform
    //         BootstrapServers = bootstrapServers,

    //         // Set the security protocol to use SSL and certificate based authentication
    //         SecurityProtocol = SecurityProtocol.Ssl,

    //         // Ssl settings
    //         SslCaPem = sslCaPem,
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

    public bool Produce(byte[] key, byte[]? value, Dictionary<string, byte[]> headers, CorrelationId correlationId)
    {
        _logger.LogInformation($"Producing message with correlation ID {correlationId.Value}");

        var message = new Message<byte[], byte[]?>
        {
            Key = _encrypt(key),
            Value = (value == null) ? null : _encrypt(value)
        };

        if(headers.Count > 0)
        {
            message.Headers = new Headers();
            foreach(var header in headers)
            {
                message.Headers.Add(_encryptHeaderKey(header.Key), _encrypt(header.Value));
            }
        }
        try
        {
            _producer.Produce(_topic.Value, message);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Got exception when producing message with correlation ID {correlationId.Value} to topic {_topic}");
            return false;
        }
        return true;
    }

    private void OnProcessExit(object? sender, EventArgs e)
    {
        // Because finalizers are not necessarily called on program exit in newer dotnet:
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/finalizers
        // Could maybe be handled by making this a BackgroundService and using the provided shutdown handling there,
        // but then again this is not really for doing long running background work.
        _logger.LogInformation("Kafka producer process exit event triggered.");
        try
        {
            _producer.Flush();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Kafka producer got exception while flushing during process termination");
        }
    }

    ~KafkaProducerService()
    {
        _logger.LogInformation("Kafka producer finalizer called.");
        try
        {
            _producer.Flush();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Kafka producer got exception while flushing during finalization");
        }
    }
}
