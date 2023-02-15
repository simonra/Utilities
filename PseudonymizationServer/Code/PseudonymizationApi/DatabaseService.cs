using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

// https://learn.microsoft.com/en-us/azure/postgresql/single-server/connect-csharp

public class DatabaseService
{
    private readonly ILogger<DatabaseService> _logger;
    private readonly DatabaseConfig _dbConfig;
    private readonly string _dbConnectionString;

    public DatabaseService(ILogger<DatabaseService> logger, DatabaseConfig dbConfig)
    {
        _logger = logger;
        _dbConfig = dbConfig;
        _dbConnectionString = _dbConfig.MakeConnectionString();
    }

    public string GetPseudonym(string originalValue)
    {
        // ToDo: Look up value, if it exists return it.
        // ToDo: If it doesn't, make a new pseudonym, insert it, and return it.
        using (var dbCon = new NpgsqlConnection(_dbConnectionString))
        {
            dbCon.Open();

            using(var command = new NpgsqlCommand("SELECT Pseudonym FROM Pseudonyms WHERE Original = @o", dbCon))
            {
                command.Parameters.AddWithValue("o", originalValue);
                var reader = command.ExecuteReader();
                var values = new List<string>();
                while(reader.Read())
                {
                    values.Add(reader.GetString(0));
                }
                reader.Close();
                if(values.Count == 1)
                {
                    return values.First();
                }
                if(values.Count > 1)
                {
                    throw new Exception("Found multiple pseudonyms for original value when retrieving pseudonym, something is extremely wrong!");
                }
            }
            // If we've reached this point, there were no pre-existing pseudonyms.
            // Therefore create a new one and return that.
            // Yes, indexing random uuids has horrible performance, but that is the price of somewhat better robustness.
            string createdPseudonym = Guid.NewGuid().ToString("N"); // Format as 32 chars without spacers

            var maxRetries = 100;
            var currentRetry = 0;
            var timeout = TimeSpan.FromMilliseconds(500);
            var startTime = DateTime.Now;
            var cutoffTime = startTime.Add(timeout);

            while(DateTime.Now < cutoffTime && currentRetry < maxRetries)
            {
                using(var command = new NpgsqlCommand("SELECT Original FROM Pseudonyms WHERE Original = @o", dbCon))
                {
                    command.Parameters.AddWithValue("o", originalValue);
                    var reader = command.ExecuteReader();
                    var values = new List<string>();
                    while(reader.Read())
                    {
                        values.Add(reader.GetString(0));
                    }
                    reader.Close();
                    if(values.Count == 0)
                    {
                        break;
                    }
                }
                createdPseudonym = Guid.NewGuid().ToString("N");
                currentRetry++;
                if(DateTime.Now > cutoffTime)
                {
                    throw new Exception("Failed to find an available pseudonym in an acceptable time");
                }
                if(currentRetry >= maxRetries)
                {
                    throw new Exception("Failed to find an available pseudonym in an acceptable time");
                }
            }

            using (var command = new NpgsqlCommand("INSERT INTO Pseudonyms (Original, Pseudonym) VALUES (@o, @p)", dbCon))
            {
                command.Parameters.AddWithValue("o", originalValue);
                command.Parameters.AddWithValue("p", createdPseudonym);

                int nRows = command.ExecuteNonQuery();
                Console.Out.WriteLine(String.Format("Number of rows inserted={0}", nRows));
                if(nRows == 1)
                {
                    return createdPseudonym;
                }
                else
                {
                    throw new Exception($"Inserting new pseudonym failed, ended up inserting {nRows} rows, should have been exactly 1");
                }
            }

            throw new Exception("Did not find pseudonym tocd retrieve, and also failed to insert a new one");
        }
    }

    public bool TryGetOriginal(string pseudonym, out string result)
    {
        // ToDo: Look up value, if it exists return it
        // ToDo: If it doesn't return error?

        using (var dbCon = new NpgsqlConnection(_dbConnectionString))
        {
            dbCon.Open();

            using(var command = new NpgsqlCommand("SELECT Original FROM Pseudonyms WHERE Pseudonym = @p", dbCon))
            {
                command.Parameters.AddWithValue("p", pseudonym);
                var reader = command.ExecuteReader();
                var values = new List<string>();
                while(reader.Read())
                {
                    values.Add(reader.GetString(0));
                }
                reader.Close();
                if(values.Count == 1)
                {
                    result = values.First();
                    return true;
                }
                if(values.Count == 0)
                {
                    result = "";
                    return true;
                }
            }
        }
        result = "";
        return false;
    }
}
