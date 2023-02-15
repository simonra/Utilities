// using System.Configuration;
// using Microsoft.Extensions.Configuration.EnvironmentVariables;
// using System.Linq;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddConfiguration();
// builder.Configuration.AddEnvironmentVariables();

// Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(builder.Configuration));

// var dbConfValues = builder.Configuration.GetSection("PseudonymizationServer").GetSection("Database");
// Console.WriteLine("DB Configuration values: ");
// Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(dbConfValues));

// var configurationBuilder = new ConfigurationBuilder();
// configurationBuilder
//     .Sources.Remove(
//         configurationBuilder.Sources.FirstOrDefault(source =>
//             source.GetType() == typeof(EnvironmentVariablesConfigurationSource)));
// configurationBuilder
//     .AddEnvironmentVariables();
// var configuration = configurationBuilder.Build();

// // var config = new ConfigurationBuilder()
// //     .AddEnvironmentVariables()
// //     // .AddCommandLine(args)
// //     // .AddJsonFile("appsettings.json")
// //     .Build();
// Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(configuration));
// var dbConfValues = configuration.GetSection("PseudonymizationServer").GetSection("Database");
// Console.WriteLine("DB Configuration values: ");
// Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(dbConfValues));

builder.Services.AddTransient((_) =>
{
    var config = new DatabaseConfig();
    builder.Configuration.Bind(nameof(DatabaseConfig), config);
    return config;
});

builder.Services.AddSingleton<DatabaseMigrationService>();
builder.Services.AddScoped<DatabaseService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var migratorService = app.Services.GetService<DatabaseMigrationService>();
migratorService?.Migrate();

app.Run();
