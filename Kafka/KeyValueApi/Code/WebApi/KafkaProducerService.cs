using System;
using System.Collections.Generic;

using Confluent.Kafka;

public class KafkaProducerService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly EnvHelpers _envHelpers;
    private IProducer<byte[], byte[]> Producer;
    private readonly KafkaTopic _topic;

    public KafkaProducerService(ILogger<KafkaConsumerService> logger, EnvHelpers envHelpers)
    {
        _logger = logger;
        _envHelpers = envHelpers;
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        var config = GetProducerConfig();
        Producer = new ProducerBuilder<byte[], byte[]>(config).Build();
        _topic = new KafkaTopic { Value = _envHelpers.GetEnvironmentVariableContent("KAFKA_KEY_VALUE_TOPIC") };
    }

    private ProducerConfig GetProducerConfig()
    {
        var bootstrapServers = _envHelpers.GetEnvironmentVariableContent("KAFKA_BOOTSTRAP_SERVERS");

        var sslCaPem = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_TRUST_STORE_PEM_LOCATION");
        var sslCertificatePem = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_CLIENT_CERTIFICATE_PEM_LOCATION");
        var sslKeyPem = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_CLIENT_KEY_PEM_LOCATION");
        var sslKeyPassword = _envHelpers.GetContentOfFileReferencedByEnvironmentVariableAsText("KAFKA_CLIENT_KEY_PASSWORD_LOCATION");

        var producerConfig = new ProducerConfig()
        {
            // Connect to the SSL listener of the local platform
            BootstrapServers = bootstrapServers,

            // Set the security protocol to use SSL and certificate based authentication
            SecurityProtocol = SecurityProtocol.Ssl,

            // Ssl settings
            SslCaPem = sslCaPem,
            SslCertificatePem = sslCertificatePem,
            SslKeyPem = sslKeyPem,
            SslKeyPassword = sslKeyPassword,

            // Specify the Kafka delivery settings
            Acks = Acks.All,
            LingerMs = 10,

            //Debug = "all"
        };

        return producerConfig;
    }

    public bool Produce(byte[] key, byte[]? value, Dictionary<string, byte[]> headers, CorrelationId correlationId)
    {
        _logger.LogInformation($"Producing message with correlation ID {correlationId.Value}");
        var message = new Message<byte[], byte[]>
        {
            Key = key,
            Value = value
        };
        foreach(var header in headers)
        {
            message.Headers.Add(header.Key, header.Value);
        }
        try
        {
            Producer.Produce(_topic.Value, message);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Got exception when producing message with correlation ID {correlationId.Value} to topic {_topic}");
            return false;
        }
        return true;
    }

    private void OnProcessExit(object sender, EventArgs e)
    {
        // Because finalizers are not necessarily called on program exit in newer dotnet:
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/finalizers
        // Could maybe be handled by making this a BackgroundService and using the provided shutdown handling there,
        // but then again this is not really for doing long running background work.
        _logger.LogInformation("Kafka producer process exit event triggered.");
        try
        {
            Producer.Flush();
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
            Producer.Flush();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Kafka producer got exception while flushing during finalization");
        }
    }
}
