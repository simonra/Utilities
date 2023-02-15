using System;
using System.Text;

public record DatabaseConfig
{
    public string? Username { get; init; }
    public string? Password { get; init; }
    public string? ServerAddress { get; init; }
    public string? Port { get; init; }
    public string? DbName { get; init; }

    public string MakeConnectionString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Server=").Append(ServerAddress).Append(';');
        sb.Append("Username=").Append(Username).Append(';');
        sb.Append("Database=").Append(DbName).Append(';');
        sb.Append("Port=").Append(Port).Append(';');
        sb.Append("Password=").Append(Password).Append(';');
        sb.Append("SSLMode=").Append("Prefer");
        return sb.ToString();
    }
}
