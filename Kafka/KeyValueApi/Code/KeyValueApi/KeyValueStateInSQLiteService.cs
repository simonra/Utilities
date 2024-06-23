using Microsoft.Data.Sqlite;

public class KeyValueStateInSQLiteService : IKeyValueStateService
{
    private readonly ILogger<KeyValueStateInSQLiteService> _logger;
    private readonly SqliteConnection _sqliteInMemDb;
    private readonly SqliteConnection _sqliteOnDiskDb;

    public KeyValueStateInSQLiteService(ILogger<KeyValueStateInSQLiteService> logger)
    {
        _logger = logger;
        _sqliteInMemDb = new SqliteConnection(GetSqliteConnectionStringForInMemory());
        _sqliteInMemDb.Open();
        InitializeDb();

        var isInitialized = false;
        var previousOnDiskCopy = GetDiskDbLocation();
        previousOnDiskCopy.Directory?.Create();
        _sqliteOnDiskDb = new SqliteConnection(GetSqliteConnectionStringForOnDiskEncrypted(previousOnDiskCopy));
        if(previousOnDiskCopy.Exists && previousOnDiskCopy.Length > 0)
        {
            _sqliteOnDiskDb.Open();
            var checkIfInitializedCommand = _sqliteOnDiskDb.CreateCommand();
            checkIfInitializedCommand.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table';";
            var checkIfInitializedResult = checkIfInitializedCommand.ExecuteScalar();
            var numberOfTables = Convert.ToInt32(checkIfInitializedResult);
            if(0 < numberOfTables)
            {
                isInitialized = true;
                OverWriteMemoryDbWithDiskDb();
            }
        }
        if(!isInitialized)
        {
            InitializeDb();
        }

        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
    }

    public bool Remove(byte[] key)
    {
        var command = _sqliteInMemDb.CreateCommand();
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
        var command = _sqliteInMemDb.CreateCommand();
        command.CommandText =
        @"
            INSERT INTO keyValueStore(kvKey, kvValue)
            VALUES ($k, $v)
            ON CONFLICT (kvKey) DO UPDATE SET kvValue=excluded.kvValue;
        ";
        command.Parameters.AddWithValue("$k", key);
        command.Parameters.AddWithValue("$v", value);
        var rowsAffected = command.ExecuteNonQuery();

        OverWriteDiskDbWithMemoryDb();

        return rowsAffected == 1;
    }

    public bool TryRetrieve(byte[] key, out byte[] value)
    {
        var command = _sqliteInMemDb.CreateCommand();
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

    private void OverWriteMemoryDbWithDiskDb()
    {
        _logger.LogDebug($"The current in memory db connection string is {GetSqliteConnectionStringForInMemory()}");
        _sqliteOnDiskDb.Open();
        var command = _sqliteOnDiskDb.CreateCommand();
        //Execute the command `VACUUM INTO 'file::memory:?cache=shared';` on CONNECTION_2.
        // command.CommandText = @"VACUUM INTO '$targetDb'";
        command.CommandText = $"VACUUM INTO 'file::memory:?cache=shared'";
        // command.Parameters.AddWithValue("$targetDb", GetSqliteConnectionStringForInMemory());
        var result = command.ExecuteNonQuery();
        _sqliteOnDiskDb.Close();
    }

    private void OverWriteDiskDbWithMemoryDb()
    {
        _logger.LogDebug("Overwriting db on disk with in mem db content");

        _logger.LogTrace("Closing disk db connection");
        _sqliteOnDiskDb.Close();

        var dbDiskLocation = GetDiskDbLocation();
        var tempPreviousFile = new FileInfo($"{dbDiskLocation.DirectoryName}/replacement-backup-old-{dbDiskLocation.Name}");
        if(dbDiskLocation.Exists)
        {
            dbDiskLocation.CopyTo(tempPreviousFile.FullName, overwrite: true);

            _logger.LogTrace($"Copy of previous version put at {tempPreviousFile.FullName}");

        }

        var tempNextFile = new FileInfo($"{dbDiskLocation.DirectoryName}/replacement-new-{dbDiskLocation.Name}");
        var tempDbConnection = new SqliteConnection(GetSqliteConnectionStringForOnDiskEncrypted(tempNextFile));
        tempDbConnection.Open();
        var tempDbInitCommand = tempDbConnection.CreateCommand();
        tempDbInitCommand.CommandText =
        @"
            CREATE TABLE IF NOT EXISTS keyValueStore (
                kvKey BLOB NOT NULL PRIMARY KEY,
                kvValue BLOB NOT NULL
            );
        ";
        tempDbInitCommand.ExecuteNonQuery();

        var command = _sqliteInMemDb.CreateCommand();
        //Execute the command `VACUUM INTO 'file::memory:?cache=shared';` on CONNECTION_2.
        command.CommandText = @"VACUUM INTO '$targetDb'";
        command.Parameters.AddWithValue("$targetDb", GetSqliteConnectionStringForOnDiskEncrypted(tempNextFile));
        var result = command.ExecuteNonQuery();

        _logger.LogTrace($"Mem db content put into temporary file {tempNextFile.FullName}");
        // tempDbConnection.Close();

        // if(tempNextFile.Exists)
        // {
        //     tempNextFile.MoveTo(dbDiskLocation.FullName, overwrite: true);
        //     _logger.LogTrace($"Replaced on disk db copy at {dbDiskLocation.FullName} with new copy from in mem db");
        // }
        // else
        // {
        //     _logger.LogTrace("Temp file for next db didn't exist. Probably because there are no entries, so it doesn't persist the structure, just nothingness?");
        //     // if(dbDiskLocation.Exists)
        //     // {
        //     //     dbDiskLocation.Delete();
        //     // }
        // }


        // if(tempPreviousFile.Exists)
        // {
        //     tempPreviousFile.Delete();
        // }

        // _logger.LogTrace("Deleted temp files");
    }

    private string GetSqliteConnectionStringForInMemory()
    {
        // var shared = "Data Source=KeyValueStateInSQLiteMemDb;Mode=Memory;Cache=Shared";

        var connectionStringBuilder = new SqliteConnectionStringBuilder();
        connectionStringBuilder.DataSource = "KeyValueStateInSQLiteMemDb";
        connectionStringBuilder.Mode = SqliteOpenMode.Memory;
        connectionStringBuilder.Cache = SqliteCacheMode.Shared;
        var connectionString = connectionStringBuilder.ToString();
        return connectionString;

        // return "Data Source=:memory:";
    }

    private string GetSqliteConnectionStringForOnDiskEncrypted(FileInfo location)
    {
        var configuredPassword = Environment.GetEnvironmentVariable(KV_API_STATE_STORAGE_SQLITE_PASSWORD);
        if(string.IsNullOrWhiteSpace(configuredPassword))
        {
            throw new InvalidOperationException($"Env var {KV_API_STATE_STORAGE_SQLITE_PASSWORD} specifying SQLite password has to be set");
        }

        var sqliteConnectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = location.FullName,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Password = configuredPassword
        }.ToString();

        return sqliteConnectionString;
    }

    private FileInfo GetDiskDbLocation()
    {
        var configuredLocation = Environment.GetEnvironmentVariable(KV_API_STATE_STORAGE_SQLITE_LOCATION);
        if(string.IsNullOrWhiteSpace(configuredLocation))
        {
            if(string.IsNullOrEmpty(configuredLocation))
            {
                _logger.LogWarning($"Failed to read content of environment variable \"{KV_API_STATE_STORAGE_SQLITE_LOCATION}\", got null/empty string");
            }
            else
            {
                _logger.LogWarning($"Failed to read proper content of environment variable \"{KV_API_STATE_STORAGE_SQLITE_LOCATION}\", contained only whitespaces");
            }
            configuredLocation = "./KeyValueStateInSQLite.db";
        }
        return new FileInfo(configuredLocation);
    }

    private void InitializeDb()
    {
        var command = _sqliteInMemDb.CreateCommand();
        command.CommandText =
        @"
            CREATE TABLE keyValueStore (
                kvKey BLOB NOT NULL PRIMARY KEY,
                kvValue BLOB NOT NULL
            );
        ";
        command.ExecuteNonQuery();

    }

    private void OnProcessExit(object? sender, EventArgs e)
    {
        // Because finalizers are not necessarily called on program exit in newer dotnet:
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/finalizers
        // Could maybe be handled by making this a BackgroundService and using the provided shutdown handling there,
        // but then again this is not really for doing long running background work.
        _logger.LogInformation("SQLite state service process termination triggered.");
        _logger.LogInformation($"SQLite state service process termination: In memory db connection state is {_sqliteInMemDb.State}.");
        try
        {
            if(_sqliteInMemDb.State == System.Data.ConnectionState.Open)
            {
                OverWriteDiskDbWithMemoryDb();
                _sqliteInMemDb.Close();
            }

        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "SQLite state service got exception while overwriting disk content during process termination");
        }
    }

    ~KeyValueStateInSQLiteService()
    {
        _logger.LogInformation("SQLite state service finalizer called.");
        _logger.LogInformation($"SQLite state service finalizer: In memory db connection state is {_sqliteInMemDb.State}.");
        try
        {
            if(_sqliteInMemDb.State == System.Data.ConnectionState.Open)
            {
                OverWriteDiskDbWithMemoryDb();
                _sqliteInMemDb.Close();
            }
            else
            {
                _logger.LogInformation("SQLite state service finalizer: In mem db connection not open (probably closed by process termination event handler), not overwriting disk instance.");
            }
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "SQLite state service got exception while overwriting disk content during finalization");
        }
    }
}
