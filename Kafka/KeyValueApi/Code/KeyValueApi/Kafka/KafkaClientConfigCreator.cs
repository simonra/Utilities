using static KafkaClientConfigEnvVars;

public static class KafkaClientConfigCreator
{
    public static Confluent.Kafka.ClientConfig GetClientConfig()
    {
        var clientConfig = new Confluent.Kafka.ClientConfig();

        var sslProviders = Environment.GetEnvironmentVariable(KAFKA_SSL_PROVIDERS);
        if(!string.IsNullOrEmpty(sslProviders)) clientConfig.SslProviders = sslProviders;

        var sslKeystorePassword = Environment.GetEnvironmentVariable(KAFKA_SSL_KEYSTORE_PASSWORD);
        if(!string.IsNullOrEmpty(sslKeystorePassword)) clientConfig.SslKeystorePassword = sslKeystorePassword;

        var sslKeystoreLocation = Environment.GetEnvironmentVariable(KAFKA_SSL_KEYSTORE_LOCATION);
        if(!string.IsNullOrEmpty(sslKeystoreLocation)) clientConfig.SslKeystoreLocation = sslKeystoreLocation;

        var sslCrlLocation = Environment.GetEnvironmentVariable(KAFKA_SSL_CRL_LOCATION);
        if(!string.IsNullOrEmpty(sslCrlLocation)) clientConfig.SslCrlLocation = sslCrlLocation;

        var SslCaCertificateStores = Environment.GetEnvironmentVariable(KAFKA_SSL_CA_CERTIFICATE_STORES);
        if(!string.IsNullOrEmpty(SslCaCertificateStores)) clientConfig.SslCaCertificateStores = SslCaCertificateStores;

        // ToDo: Get from location?
        var SslCaPem = Environment.GetEnvironmentVariable(KAFKA_SSL_CA_PEM);
        if(!string.IsNullOrEmpty(SslCaPem)) clientConfig.SslCaPem = SslCaPem;

        var SslCaPemLocation = Environment.GetEnvironmentVariable(KAFKA_SSL_CA_PEM_LOCATION);
        if(!string.IsNullOrEmpty(SslCaPemLocation)) clientConfig.SslCaPem = File.ReadAllText(SslCaPemLocation);

        var SslCaLocation = Environment.GetEnvironmentVariable(KAFKA_SSL_CA_LOCATION);
        if(!string.IsNullOrEmpty(SslCaLocation)) clientConfig.SslCaLocation = SslCaLocation;

        var SslCertificateLocation = Environment.GetEnvironmentVariable(KAFKA_SSL_CERTIFICATE_LOCATION);
        if(!string.IsNullOrEmpty(SslCertificateLocation)) clientConfig.SslCertificateLocation = SslCertificateLocation;

        var SslEngineLocation = Environment.GetEnvironmentVariable(KAFKA_SSL_ENGINE_LOCATION);
        if(!string.IsNullOrEmpty(SslEngineLocation)) clientConfig.SslEngineLocation = SslEngineLocation;

        var SslKeyPem = Environment.GetEnvironmentVariable(KAFKA_SSL_KEY_PEM);
        if(!string.IsNullOrEmpty(SslKeyPem)) clientConfig.SslKeyPem = SslKeyPem;

        var SslKeyPassword = Environment.GetEnvironmentVariable(KAFKA_SSL_KEY_PASSWORD);
        if(!string.IsNullOrEmpty(SslKeyPassword)) clientConfig.SslKeyPassword = SslKeyPassword;

        var SslKeyPasswordLocation = Environment.GetEnvironmentVariable(KAFKA_SSL_KEY_PASSWORD_LOCATION);
        if(!string.IsNullOrEmpty(SslKeyPasswordLocation)) clientConfig.SslCaPem = File.ReadAllText(SslKeyPasswordLocation);

        var SslKeyLocation = Environment.GetEnvironmentVariable(KAFKA_SSL_KEY_LOCATION);
        if(!string.IsNullOrEmpty(SslKeyLocation)) clientConfig.SslKeyLocation = SslKeyLocation;

        var SslSigalgsList = Environment.GetEnvironmentVariable(KAFKA_SSL_SIGALGS_LIST);
        if(!string.IsNullOrEmpty(SslSigalgsList)) clientConfig.SslSigalgsList = SslSigalgsList;

        var SslCurvesList = Environment.GetEnvironmentVariable(KAFKA_SSL_CURVES_LIST);
        if(!string.IsNullOrEmpty(SslCurvesList)) clientConfig.SslCurvesList = SslCurvesList;

        var SslCipherSuites = Environment.GetEnvironmentVariable(KAFKA_SSL_CIPHER_SUITES);
        if(!string.IsNullOrEmpty(SslCipherSuites)) clientConfig.SslCipherSuites = SslCipherSuites;

        // ToDo: Use location?
        var SslCertificatePem = Environment.GetEnvironmentVariable(KAFKA_SSL_CERTIFICATE_PEM);
        if(!string.IsNullOrEmpty(SslCertificatePem)) clientConfig.SslCertificatePem = SslCertificatePem;

        var SslEngineId = Environment.GetEnvironmentVariable(KAFKA_SSL_ENGINE_ID);
        if(!string.IsNullOrEmpty(SslEngineId)) clientConfig.SslEngineId = SslEngineId;

        var SslEndpointIdentificationAlgorithm = Environment.GetEnvironmentVariable(KAFKA_SSL_ENDPOINT_IDENTIFICATION_ALGORITHM);
        switch (SslEndpointIdentificationAlgorithm?.ToLowerInvariant())
        {
            case "none":
                clientConfig.SslEndpointIdentificationAlgorithm = Confluent.Kafka.SslEndpointIdentificationAlgorithm.None;
                break;
            case "https":
                clientConfig.SslEndpointIdentificationAlgorithm = Confluent.Kafka.SslEndpointIdentificationAlgorithm.Https;
                break;
            default:
                break;
        }

        var SecurityProtocol = Environment.GetEnvironmentVariable(KAFKA_SECURITY_PROTOCOL);
        switch (SecurityProtocol?.ToLowerInvariant())
        {
            case "plaintext":
                clientConfig.SecurityProtocol = Confluent.Kafka.SecurityProtocol.Plaintext;
                break;
            case "saslplaintext":
                clientConfig.SecurityProtocol = Confluent.Kafka.SecurityProtocol.SaslPlaintext;
                break;
            case "saslssl":
                clientConfig.SecurityProtocol = Confluent.Kafka.SecurityProtocol.SaslSsl;
                break;
            case "ssl":
                clientConfig.SecurityProtocol = Confluent.Kafka.SecurityProtocol.Ssl;
                break;
            default:
                break;
        }

        var PluginLibraryPaths = Environment.GetEnvironmentVariable(KAFKA_PLUGIN_LIBRARY_PATHS);
        if(!string.IsNullOrEmpty(PluginLibraryPaths)) clientConfig.PluginLibraryPaths = PluginLibraryPaths;

        var SaslOauthbearerTokenEndpointUrl = Environment.GetEnvironmentVariable(KAFKA_SASL_OAUTHBEARER_TOKEN_ENDPOINT_URL);
        if(!string.IsNullOrEmpty(SaslOauthbearerTokenEndpointUrl)) clientConfig.SaslOauthbearerTokenEndpointUrl = SaslOauthbearerTokenEndpointUrl;

        var SaslOauthbearerExtensions = Environment.GetEnvironmentVariable(KAFKA_SASL_OAUTHBEARER_EXTENSIONS);
        if(!string.IsNullOrEmpty(SaslOauthbearerExtensions)) clientConfig.SaslOauthbearerExtensions = SaslOauthbearerExtensions;

        var SaslOauthbearerScope = Environment.GetEnvironmentVariable(KAFKA_SASL_OAUTHBEARER_SCOPE);
        if(!string.IsNullOrEmpty(SaslOauthbearerScope)) clientConfig.SaslOauthbearerScope = SaslOauthbearerScope;

        var SaslOauthbearerClientSecret = Environment.GetEnvironmentVariable(KAFKA_SASL_OAUTHBEARER_CLIENT_SECRET);
        if(!string.IsNullOrEmpty(SaslOauthbearerClientSecret)) clientConfig.SaslOauthbearerClientSecret = SaslOauthbearerClientSecret;

        var SaslOauthbearerClientId = Environment.GetEnvironmentVariable(KAFKA_SASL_OAUTHBEARER_CLIENT_ID);
        if(!string.IsNullOrEmpty(SaslOauthbearerClientId)) clientConfig.SaslOauthbearerClientId = SaslOauthbearerClientId;

        var SaslOauthbearerMethod = Environment.GetEnvironmentVariable(KAFKA_SASL_OAUTHBEARER_METHOD);
        switch (SaslOauthbearerMethod?.ToLowerInvariant())
        {
            case "default":
                clientConfig.SaslOauthbearerMethod = Confluent.Kafka.SaslOauthbearerMethod.Default;
                break;
            case "oidc":
                clientConfig.SaslOauthbearerMethod = Confluent.Kafka.SaslOauthbearerMethod.Oidc;
                break;
            default:
                break;
        }

        var EnableSslCertificateVerification = Environment.GetEnvironmentVariable(KAFKA_ENABLE_SSL_CERTIFICATE_VERIFICATION);
        switch (EnableSslCertificateVerification?.ToLowerInvariant())
        {
            case "true":
                clientConfig.EnableSslCertificateVerification = true;
                break;
            case "false":
                clientConfig.EnableSslCertificateVerification = false;
                break;
            default:
                break;
        }

        var EnableSaslOauthbearerUnsecureJwt = Environment.GetEnvironmentVariable(KAFKA_ENABLE_SASL_OAUTHBEARER_UNSECURE_JWT);
        switch (EnableSaslOauthbearerUnsecureJwt?.ToLowerInvariant())
        {
            case "true":
                clientConfig.EnableSaslOauthbearerUnsecureJwt = true;
                break;
            case "false":
                clientConfig.EnableSaslOauthbearerUnsecureJwt = false;
                break;
            default:
                break;
        }

        var SaslPassword = Environment.GetEnvironmentVariable(KAFKA_SASL_PASSWORD);
        if(!string.IsNullOrEmpty(SaslPassword)) clientConfig.SaslPassword = SaslPassword;

        var SaslUsername = Environment.GetEnvironmentVariable(KAFKA_SASL_USERNAME);
        if(!string.IsNullOrEmpty(SaslUsername)) clientConfig.SaslUsername = SaslUsername;

        var SaslKerberosMinTimeBeforeRelogin = Environment.GetEnvironmentVariable(KAFKA_SASL_KERBEROS_MIN_TIME_BEFORE_RELOGIN);
        if(!string.IsNullOrEmpty(SaslKerberosMinTimeBeforeRelogin)) clientConfig.SaslKerberosMinTimeBeforeRelogin = int.Parse(SaslKerberosMinTimeBeforeRelogin);

        var SaslKerberosKeytab = Environment.GetEnvironmentVariable(KAFKA_SASL_KERBEROS_KEYTAB);
        if(!string.IsNullOrEmpty(SaslKerberosKeytab)) clientConfig.SaslKerberosKeytab = SaslKerberosKeytab;

        var SaslKerberosKinitCmd = Environment.GetEnvironmentVariable(KAFKA_SASL_KERBEROS_KINIT_CMD);
        if(!string.IsNullOrEmpty(SaslKerberosKinitCmd)) clientConfig.SaslKerberosKinitCmd = SaslKerberosKinitCmd;

        var SaslKerberosPrincipal = Environment.GetEnvironmentVariable(KAFKA_SASL_KERBEROS_PRINCIPAL);
        if(!string.IsNullOrEmpty(SaslKerberosPrincipal)) clientConfig.SaslKerberosPrincipal = SaslKerberosPrincipal;

        var SaslKerberosServiceName = Environment.GetEnvironmentVariable(KAFKA_SASL_KERBEROS_SERVICE_NAME);
        if(!string.IsNullOrEmpty(SaslKerberosServiceName)) clientConfig.SaslKerberosServiceName = SaslKerberosServiceName;

        var SaslOauthbearerConfig = Environment.GetEnvironmentVariable(KAFKA_SASL_OAUTHBEARER_CONFIG);
        if(!string.IsNullOrEmpty(SaslOauthbearerConfig)) clientConfig.SaslOauthbearerConfig = SaslOauthbearerConfig;

        var AllowAutoCreateTopics = Environment.GetEnvironmentVariable(KAFKA_ALLOW_AUTO_CREATE_TOPICS_VALUE);
        switch (AllowAutoCreateTopics?.ToLowerInvariant())
        {
            case "true":
                clientConfig.AllowAutoCreateTopics = true;
                break;
            case "false":
                clientConfig.AllowAutoCreateTopics = false;
                break;
            default:
                break;
        }

        var BrokerVersionFallback = Environment.GetEnvironmentVariable(KAFKA_BROKER_VERSION_FALLBACK);
        if(!string.IsNullOrEmpty(BrokerVersionFallback)) clientConfig.BrokerVersionFallback = BrokerVersionFallback;

        var apiVersionFallbackMs = Environment.GetEnvironmentVariable(KAFKA_API_VERSION_FALLBACK_MS);
        if(!string.IsNullOrEmpty(apiVersionFallbackMs)) clientConfig.ApiVersionFallbackMs = int.Parse(apiVersionFallbackMs);

        var debug = Environment.GetEnvironmentVariable(KAFKA_DEBUG);
        if(!string.IsNullOrEmpty(debug)) clientConfig.Debug = debug;

        var topicBlacklist = Environment.GetEnvironmentVariable(KAFKA_TOPIC_BLACKLIST);
        if(!string.IsNullOrEmpty(topicBlacklist)) clientConfig.TopicBlacklist = topicBlacklist;

        var topicMetadataPropagationMaxMs = Environment.GetEnvironmentVariable(KAFKA_TOPIC_METADATA_PROPAGATION_MAX_MS);
        if(!string.IsNullOrEmpty(topicMetadataPropagationMaxMs)) clientConfig.TopicMetadataPropagationMaxMs = int.Parse(topicMetadataPropagationMaxMs);

        var topicMetadataRefreshSparse = Environment.GetEnvironmentVariable(KAFKA_TOPIC_METADATA_REFRESH_SPARSE);
        switch (topicMetadataRefreshSparse?.ToLowerInvariant())
        {
            case "true":
                clientConfig.TopicMetadataRefreshSparse = true;
                break;
            case "false":
                clientConfig.TopicMetadataRefreshSparse = false;
                break;
            default:
                break;
        }

        var topicMetadataRefreshFastIntervalMs = Environment.GetEnvironmentVariable(KAFKA_TOPIC_METADATA_REFRESH_FAST_INTERVAL_MS);
        if(!string.IsNullOrEmpty(topicMetadataRefreshFastIntervalMs)) clientConfig.TopicMetadataRefreshFastIntervalMs = int.Parse(topicMetadataRefreshFastIntervalMs);

        var metadataMaxAgeMs = Environment.GetEnvironmentVariable(KAFKA_METADATA_MAX_AGE_MS);
        if(!string.IsNullOrEmpty(metadataMaxAgeMs)) clientConfig.MetadataMaxAgeMs = int.Parse(metadataMaxAgeMs);

        var topicMetadataRefreshIntervalMs = Environment.GetEnvironmentVariable(KAFKA_TOPIC_METADATA_REFRESH_INTERVAL_MS);
        if(!string.IsNullOrEmpty(topicMetadataRefreshIntervalMs)) clientConfig.TopicMetadataRefreshIntervalMs = int.Parse(topicMetadataRefreshIntervalMs);

        var maxInFlight = Environment.GetEnvironmentVariable(KAFKA_MAX_IN_FLIGHT);
        if(!string.IsNullOrEmpty(maxInFlight)) clientConfig.MaxInFlight = int.Parse(maxInFlight);

        var receiveMessageMaxBytes = Environment.GetEnvironmentVariable(KAFKA_RECEIVE_MESSAGE_MAX_BYTES);
        if(!string.IsNullOrEmpty(receiveMessageMaxBytes)) clientConfig.ReceiveMessageMaxBytes = int.Parse(receiveMessageMaxBytes);

        var messageCopyMaxBytes = Environment.GetEnvironmentVariable(KAFKA_MESSAGE_COPY_MAX_BYTES);
        if(!string.IsNullOrEmpty(messageCopyMaxBytes)) clientConfig.MessageCopyMaxBytes = int.Parse(messageCopyMaxBytes);

        var messageMaxBytes = Environment.GetEnvironmentVariable(KAFKA_MESSAGE_MAX_BYTES);
        if(!string.IsNullOrEmpty(messageMaxBytes)) clientConfig.MessageMaxBytes = int.Parse(messageMaxBytes);

        var bootstrapServers = Environment.GetEnvironmentVariable(KAFKA_BOOTSTRAP_SERVERS);
        if(!string.IsNullOrEmpty(bootstrapServers)) clientConfig.BootstrapServers = bootstrapServers;

        var clientId = Environment.GetEnvironmentVariable(KAFKA_CLIENT_ID);
        if(!string.IsNullOrEmpty(clientId)) clientConfig.ClientId = clientId;

        var acks = Environment.GetEnvironmentVariable(KAFKA_ACKS);
        switch (acks?.ToLowerInvariant())
        {
            case "all":
                clientConfig.Acks = Confluent.Kafka.Acks.All;
                break;
            case "leader":
                clientConfig.Acks = Confluent.Kafka.Acks.Leader;
                break;
            case "none":
                clientConfig.Acks = Confluent.Kafka.Acks.None;
                break;
            default:
                break;
        }

        var saslMechanism = Environment.GetEnvironmentVariable(KAFKA_SASL_MECHANISM);
        switch (saslMechanism?.ToLowerInvariant())
        {
            case "gssapi":
                clientConfig.SaslMechanism = Confluent.Kafka.SaslMechanism.Gssapi;
                break;
            case "oauthbearer":
                clientConfig.SaslMechanism = Confluent.Kafka.SaslMechanism.OAuthBearer;
                break;
            case "plain":
                clientConfig.SaslMechanism = Confluent.Kafka.SaslMechanism.Plain;
                break;
            case "scramsha256":
                clientConfig.SaslMechanism = Confluent.Kafka.SaslMechanism.ScramSha256;
                break;
            case "scramsha512":
                clientConfig.SaslMechanism = Confluent.Kafka.SaslMechanism.ScramSha512;
                break;
            default:
                break;
        }

        var socketTimeoutMs = Environment.GetEnvironmentVariable(KAFKA_SOCKET_TIMEOUT_MS);
        if(!string.IsNullOrEmpty(socketTimeoutMs)) clientConfig.SocketTimeoutMs = int.Parse(socketTimeoutMs);

        var socketSendBufferBytes = Environment.GetEnvironmentVariable(KAFKA_SOCKET_SEND_BUFFER_BYTES);
        if(!string.IsNullOrEmpty(socketSendBufferBytes)) clientConfig.SocketSendBufferBytes = int.Parse(socketSendBufferBytes);

        var socketReceiveBufferBytes = Environment.GetEnvironmentVariable(KAFKA_SOCKET_RECEIVE_BUFFER_BYTES);
        if(!string.IsNullOrEmpty(socketReceiveBufferBytes)) clientConfig.SocketReceiveBufferBytes = int.Parse(socketReceiveBufferBytes);

        var socketKeepaliveEnable = Environment.GetEnvironmentVariable(KAFKA_SOCKET_KEEPALIVE_ENABLE);
        switch (socketKeepaliveEnable?.ToLowerInvariant())
        {
            case "true":
                clientConfig.SocketKeepaliveEnable = true;
                break;
            case "false":
                clientConfig.SocketKeepaliveEnable = false;
                break;
            default:
                break;
        }

        var apiVersionRequestTimeoutMs = Environment.GetEnvironmentVariable(KAFKA_API_VERSION_REQUEST_TIMEOUT_MS);
        if(!string.IsNullOrEmpty(apiVersionRequestTimeoutMs)) clientConfig.ApiVersionRequestTimeoutMs = int.Parse(apiVersionRequestTimeoutMs);

        var apiVersionRequest = Environment.GetEnvironmentVariable(KAFKA_API_VERSION_REQUEST);
        switch (apiVersionRequest?.ToLowerInvariant())
        {
            case "true":
                clientConfig.ApiVersionRequest = true;
                break;
            case "false":
                clientConfig.ApiVersionRequest = false;
                break;
            default:
                break;
        }

        var internalTerminationSignal = Environment.GetEnvironmentVariable(KAFKA_INTERNAL_TERMINATION_SIGNAL);
        if(!string.IsNullOrEmpty(internalTerminationSignal)) clientConfig.InternalTerminationSignal = int.Parse(internalTerminationSignal);

        var logConnectionClose = Environment.GetEnvironmentVariable(KAFKA_LOG_CONNECTION_CLOSE);
        switch (logConnectionClose?.ToLowerInvariant())
        {
            case "true":
                clientConfig.LogConnectionClose = true;
                break;
            case "false":
                clientConfig.LogConnectionClose = false;
                break;
            default:
                break;
        }

        var enableRandomSeed = Environment.GetEnvironmentVariable(KAFKA_ENABLE_RANDOM_SEED);
        switch (enableRandomSeed?.ToLowerInvariant())
        {
            case "true":
                clientConfig.EnableRandomSeed = true;
                break;
            case "false":
                clientConfig.EnableRandomSeed = false;
                break;
            default:
                break;
        }

        var logThreadName = Environment.GetEnvironmentVariable(KAFKA_LOG_THREAD_NAME);
        switch (logThreadName?.ToLowerInvariant())
        {
            case "true":
                clientConfig.LogThreadName = true;
                break;
            case "false":
                clientConfig.LogThreadName = false;
                break;
            default:
                break;
        }

        var logQueue = Environment.GetEnvironmentVariable(KAFKA_LOG_QUEUE);
        switch (logQueue?.ToLowerInvariant())
        {
            case "true":
                clientConfig.LogQueue = true;
                break;
            case "false":
                clientConfig.LogQueue = false;
                break;
            default:
                break;
        }

        var clientRack = Environment.GetEnvironmentVariable(KAFKA_CLIENT_RACK);
        if(!string.IsNullOrEmpty(clientRack)) clientConfig.ClientRack = clientRack;

        var statisticsIntervalMs = Environment.GetEnvironmentVariable(KAFKA_STATISTICS_INTERVAL_MS);
        if(!string.IsNullOrEmpty(statisticsIntervalMs)) clientConfig.StatisticsIntervalMs = int.Parse(statisticsIntervalMs);

        var reconnectBackoffMs = Environment.GetEnvironmentVariable(KAFKA_RECONNECT_BACKOFF_MS);
        if(!string.IsNullOrEmpty(reconnectBackoffMs)) clientConfig.ReconnectBackoffMs = int.Parse(reconnectBackoffMs);

        var connectionsMaxIdleMs = Environment.GetEnvironmentVariable(KAFKA_CONNECTIONS_MAX_IDLE_MS);
        if(!string.IsNullOrEmpty(connectionsMaxIdleMs)) clientConfig.ConnectionsMaxIdleMs = int.Parse(connectionsMaxIdleMs);

        var socketConnectionSetupTimeoutMs = Environment.GetEnvironmentVariable(KAFKA_SOCKET_CONNECTION_SETUP_TIMEOUT_MS);
        if(!string.IsNullOrEmpty(socketConnectionSetupTimeoutMs)) clientConfig.SocketConnectionSetupTimeoutMs = int.Parse(socketConnectionSetupTimeoutMs);

        var BrokerAddressFamily = Environment.GetEnvironmentVariable(KAFKA_BROKER_ADDRESS_FAMILY);
        switch (BrokerAddressFamily?.ToLowerInvariant())
        {
            case "any":
                clientConfig.BrokerAddressFamily = Confluent.Kafka.BrokerAddressFamily.Any;
                break;
            case "v4":
                clientConfig.BrokerAddressFamily = Confluent.Kafka.BrokerAddressFamily.V4;
                break;
            case "v6":
                clientConfig.BrokerAddressFamily = Confluent.Kafka.BrokerAddressFamily.V6;
                break;
            default:
                break;
        }

        var brokerAddressTtl = Environment.GetEnvironmentVariable(KAFKA_BROKER_ADDRESS_TTL);
        if(!string.IsNullOrEmpty(brokerAddressTtl)) clientConfig.BrokerAddressTtl = int.Parse(brokerAddressTtl);

        var socketMaxFails = Environment.GetEnvironmentVariable(KAFKA_SOCKET_MAX_FAILS);
        if(!string.IsNullOrEmpty(socketMaxFails)) clientConfig.SocketMaxFails = int.Parse(socketMaxFails);

        var socketNagleDisable = Environment.GetEnvironmentVariable(KAFKA_SOCKET_NAGLE_DISABLE);
        switch (socketNagleDisable?.ToLowerInvariant())
        {
            case "true":
                clientConfig.SocketNagleDisable = true;
                break;
            case "false":
                clientConfig.SocketNagleDisable = false;
                break;
            default:
                break;
        }

        var reconnectBackoffMaxMs = Environment.GetEnvironmentVariable(KAFKA_RECONNECT_BACKOFF_MAX_MS);
        if(!string.IsNullOrEmpty(reconnectBackoffMaxMs)) clientConfig.ReconnectBackoffMaxMs = int.Parse(reconnectBackoffMaxMs);

        var ClientDnsLookup = Environment.GetEnvironmentVariable(KAFKA_CLIENT_DNS_LOOKUP);
        switch (ClientDnsLookup?.ToLowerInvariant())
        {
            case "resolvecanonicalbootstrapserversonly":
                clientConfig.ClientDnsLookup = Confluent.Kafka.ClientDnsLookup.ResolveCanonicalBootstrapServersOnly;
                break;
            case "usealldnsips":
                clientConfig.ClientDnsLookup = Confluent.Kafka.ClientDnsLookup.UseAllDnsIps;
                break;
            default:
                break;
        }

        return clientConfig;
    }
}
