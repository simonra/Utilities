using Npgsql;

public class DatabaseMigrationService
{
    private readonly ILogger<DatabaseMigrationService> _logger;
    private readonly DatabaseConfig _dbConfig;
    private readonly string _dbConnectionString;

    public DatabaseMigrationService(ILogger<DatabaseMigrationService> logger, DatabaseConfig dbConfig)
    {
        _logger = logger;
        _dbConfig = dbConfig;
        _dbConnectionString = _dbConfig.MakeConnectionString();
        _logger.LogInformation($"DB Config\n{_dbConfig}");
    }

    public void Migrate()
    {
        using (var dbCon = new NpgsqlConnection(_dbConnectionString))
        {
            Console.Out.WriteLine("Opening connection");
            dbCon.Open();

            // using (var command = new NpgsqlCommand("DROP TABLE IF EXISTS Pseudonyms", dbCon))
            // {
            //     command.ExecuteNonQuery();
            //     Console.Out.WriteLine("Finished dropping table (if existed)");
            // }

            using (var command = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS Pseudonyms(id serial PRIMARY KEY, Pseudonym VARCHAR(32), Original VARCHAR)", dbCon))
            {
                command.ExecuteNonQuery();
                Console.Out.WriteLine("Finished creating table");
            }
        }
    }
}
