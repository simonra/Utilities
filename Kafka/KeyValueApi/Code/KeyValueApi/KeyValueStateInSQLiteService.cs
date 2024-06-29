using Microsoft.Data.Sqlite;

public class KeyValueStateInSQLiteService : IKeyValueStateService
{
    private readonly ILogger<KeyValueStateInSQLiteService> _logger;
    private readonly SqliteConnection _sqliteDb;

    public KeyValueStateInSQLiteService(ILogger<KeyValueStateInSQLiteService> logger)
    {
        _logger = logger;
        _sqliteDb = new SqliteConnection(GetSqliteConnectionString());
        _logger.LogInformation($"Connection to db using connection string \"{GetSqliteConnectionString()}\" set up");
        _sqliteDb.Open();
        InitializeDb();
    }

    public bool Remove(byte[] key)
    {
        var command = _sqliteDb.CreateCommand();
        command.CommandText =
        @"
            DELETE FROM keyValueStore
            WHERE kvKey = $k;
        ";
        command.Parameters.AddWithValue("$k", key);
        var rowsAffected = command.ExecuteNonQuery();
        return rowsAffected == 1;
    }

    public bool Store(byte[] key, byte[] value)
    {
        var command = _sqliteDb.CreateCommand();
        command.CommandText =
        @"
            INSERT INTO keyValueStore(kvKey, kvValue)
            VALUES ($k, $v)
            ON CONFLICT (kvKey) DO UPDATE SET kvValue=excluded.kvValue;
        ";
        command.Parameters.AddWithValue("$k", key);
        command.Parameters.AddWithValue("$v", value);
        var rowsAffected = command.ExecuteNonQuery();

        return rowsAffected == 1;
    }

    public bool TryRetrieve(byte[] key, out byte[] value)
    {
        var command = _sqliteDb.CreateCommand();
        command.CommandText =
        @"
            SELECT kvValue
            FROM keyValueStore
            WHERE kvKey = $k
        ";
        command.Parameters.AddWithValue("$k", key);
        var result = command.ExecuteScalar();
        if(result != null && result is byte[] converted)
        {
            value = converted;
            return true;
        }
        value = [];
        return false;
    }

    public List<KafkaTopicPartitionOffset> GetLastConsumedTopicPartitionOffsets()
    {
        List<KafkaTopicPartitionOffset> topicPartitionOffsets = [];

        var command = _sqliteDb.CreateCommand();
        command.CommandText =
        @"
            SELECT topic, partition, offset
            FROM topicPartitionOffsets
        ";
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var topic = reader.GetString(0);
                var partition = reader.GetInt32(1);
                var offset = reader.GetInt64(2);

                topicPartitionOffsets.Add(new KafkaTopicPartitionOffset
                    {
                        Topic = new KafkaTopic { Value = topic },
                        Partition = new KafkaPartition { Value = partition },
                        Offset = new KafkaOffset{ Value = offset }
                    });
            }
        }
        return topicPartitionOffsets;
    }

    public bool UpdateLastConsumedTopicPartitionOffsets(KafkaTopicPartitionOffset topicPartitionOffsets)
    {
        var command = _sqliteDb.CreateCommand();
        command.CommandText =
        @"
            INSERT INTO topicPartitionOffsets(topic, partition, offset)
            VALUES ($t, $p, $o)
            ON CONFLICT (topic, partition) DO UPDATE SET offset=excluded.offset;
        ";
        command.Parameters.AddWithValue("$t", topicPartitionOffsets.Topic.Value);
        command.Parameters.AddWithValue("$p", topicPartitionOffsets.Partition.Value);
        command.Parameters.AddWithValue("$o", topicPartitionOffsets.Offset.Value);
        var rowsAffected = command.ExecuteNonQuery();

        return rowsAffected == 1;
    }

    private string GetSqliteConnectionString()
    {
        var configuredLocation = Environment.GetEnvironmentVariable(KV_API_STATE_STORAGE_SQLITE_LOCATION);
        if(string.IsNullOrWhiteSpace(configuredLocation))
        {
            if(string.IsNullOrEmpty(configuredLocation))
            {
                _logger.LogInformation($"Failed to read content of environment variable \"{KV_API_STATE_STORAGE_SQLITE_LOCATION}\", got null/empty string");
            }
            else
            {
                _logger.LogWarning($"Failed to read proper content of environment variable \"{KV_API_STATE_STORAGE_SQLITE_LOCATION}\", contained only whitespaces");
            }
            _logger.LogInformation($"Because {KV_API_STATE_STORAGE_SQLITE_LOCATION} was not set, setting up in memory sqlite");
            var connectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = "KeyValueStateInSQLiteMemDb",
                Mode = SqliteOpenMode.Memory,
                Cache = SqliteCacheMode.Shared
            };
            var connectionString = connectionStringBuilder.ToString();
            return connectionString;
        }

        var location = new FileInfo(configuredLocation);
        if(!location.Exists)
        {
            location.Directory?.Create();
        }
        var configuredPassword = Environment.GetEnvironmentVariable(KV_API_STATE_STORAGE_SQLITE_PASSWORD);
        if(string.IsNullOrWhiteSpace(configuredPassword))
        {
            _logger.LogInformation($"Env var {KV_API_STATE_STORAGE_SQLITE_PASSWORD} specifying SQLite password has to be set, running without encrypting sqlite database on disk");
            return new SqliteConnectionStringBuilder()
            {
                DataSource = location.FullName,
                Mode = SqliteOpenMode.ReadWriteCreate
            }.ToString();
        }

        var sqliteConnectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = location.FullName,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Password = configuredPassword
        }.ToString();

        return sqliteConnectionString;
    }

    private void InitializeDb()
    {
        var command = _sqliteDb.CreateCommand();
        command.CommandText =
        @"
            CREATE TABLE IF NOT EXISTS keyValueStore (
                kvKey BLOB NOT NULL PRIMARY KEY,
                kvValue BLOB NOT NULL
            );

            CREATE TABLE IF NOT EXISTS topicPartitionOffsets (
                topic TEXT NOT NULL,
                partition INTEGER NOT NULL,
                offset INTEGER NOT NULL,
                PRIMARY KEY(topic, partition)
            );
        ";
        command.ExecuteNonQuery();
    }
}
