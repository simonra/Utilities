global using static EnvVarNames;

using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<KafkaAdminClient>();
var configuredStorageType = Environment.GetEnvironmentVariable(KV_API_STATE_STORAGE_TYPE);
if(configuredStorageType == "sqlite")
{
    Console.WriteLine($"Setting up local state storage to use SQLite");
    builder.Services.AddSingleton<IKeyValueStateService,KeyValueStateInSQLiteService>();
}
else if(configuredStorageType == "disk")
{
    Console.WriteLine($"Setting up local state storage to use disk");
    builder.Services.AddSingleton<IKeyValueStateService,KeyValeStateOnFileSystemService>();
}
else
{
    // Fall back to in memory storage
    Console.WriteLine($"Setting up local state storage to use in memory dict");
    builder.Services.AddSingleton<IKeyValueStateService,KeyValueStateInDictService>();
}
builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddSingleton<KafkaProducerService>();

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

app.MapPost("/store", (HttpContext http, ApiParamStore postContent, KafkaProducerService kafkaProducerService) =>
{
    var correlationIdValue = System.Guid.NewGuid().ToString("D");
    if(postContent.Headers?.ContainsKey("correlationId") ?? false) correlationIdValue = postContent.Headers["correlationId"];
    if(!string.IsNullOrEmpty(postContent.CorrelationId)) correlationIdValue = postContent.CorrelationId;
    var correlationId = new CorrelationId { Value = correlationIdValue };
    http.Response.Headers.Append("X-Correlation-Id", correlationId.Value);

    var eventKeyBytes = System.Text.Encoding.UTF8.GetBytes(postContent.Key);
    var eventValueBytes = System.Text.Encoding.UTF8.GetBytes(postContent.Value);

    Dictionary<string, byte[]> headers = [];
    foreach(var kvp in postContent.Headers ?? [])
    {
        headers.Add(kvp.Key, System.Text.Encoding.UTF8.GetBytes(kvp.Value));
    }
    if(!headers.ContainsKey("correlationId")) headers["correlationId"] = System.Text.Encoding.UTF8.GetBytes(correlationId.Value);

    var produceSuccess = kafkaProducerService.Produce(eventKeyBytes, eventValueBytes, headers, correlationId);
    if(produceSuccess)
    {
        return Results.Ok($"Stored");
    }
    return Results.Text(
        content: $"Storage failed",
        contentType: "text/html",
        contentEncoding: Encoding.UTF8,
        statusCode: (int?) HttpStatusCode.InternalServerError);
});

app.MapPost("/retrieve", (HttpContext http, ApiParamRetrieve postContent, IKeyValueStateService keyValueStateService) =>
{
    var correlationIdValue = System.Guid.NewGuid().ToString("D");
    if(!string.IsNullOrEmpty(postContent.CorrelationId)) correlationIdValue = postContent.CorrelationId;
    var correlationId = new CorrelationId { Value = correlationIdValue };
    http.Response.Headers.Append("X-Correlation-Id", correlationId.Value);

    var keyBytes = System.Text.Encoding.UTF8.GetBytes(postContent.Key);
    var retrieveSuccess = keyValueStateService.TryRetrieve(keyBytes, out var valueBytes);
    var retrievedValueAsString = System.Text.Encoding.UTF8.GetString(valueBytes);
    return Results.Ok(retrievedValueAsString);
});

app.MapPost("/remove", (HttpContext http, ApiParamRemove postContent, KafkaProducerService kafkaProducerService) =>
{
    var correlationIdValue = System.Guid.NewGuid().ToString("D");
    if(!string.IsNullOrEmpty(postContent.CorrelationId)) correlationIdValue = postContent.CorrelationId;
    var correlationId = new CorrelationId { Value = correlationIdValue };
    http.Response.Headers.Append("X-Correlation-Id", correlationId.Value);

    var eventKeyBytes = System.Text.Encoding.UTF8.GetBytes(postContent.Key);

    Dictionary<string, byte[]> headers = [];
    headers["correlationId"] = System.Text.Encoding.UTF8.GetBytes(correlationId.Value);

    var produceSuccess = kafkaProducerService.Produce(eventKeyBytes, null, headers, correlationId);
    if(produceSuccess)
    {
        return Results.Ok($"Removed");
    }
    return Results.Text(
        content: $"Removal failed",
        contentType: "text/html",
        contentEncoding: Encoding.UTF8,
        statusCode: (int?) HttpStatusCode.InternalServerError);
});


app.MapPost("/store/b64", (HttpContext http, ApiParamStore postContent, KafkaProducerService kafkaProducerService) =>
{
    var correlationIdValue = System.Guid.NewGuid().ToString("D");
    if(postContent.Headers?.ContainsKey("correlationId") ?? false) correlationIdValue = postContent.Headers["correlationId"];
    if(!string.IsNullOrEmpty(postContent.CorrelationId)) correlationIdValue = postContent.CorrelationId;
    var correlationId = new CorrelationId { Value = correlationIdValue };
    http.Response.Headers.Append("X-Correlation-Id", correlationId.Value);

    var eventKeyBytes = Convert.FromBase64String(postContent.Key);
    var eventValueBytes = Convert.FromBase64String(postContent.Value);

    Dictionary<string, byte[]> headers = [];
    foreach(var kvp in postContent.Headers ?? [])
    {
        headers.Add(kvp.Key, Convert.FromBase64String(kvp.Value));
    }
    if(!headers.ContainsKey("correlationId")) headers["correlationId"] = System.Text.Encoding.UTF8.GetBytes(correlationId.Value);

    var produceSuccess = kafkaProducerService.Produce(eventKeyBytes, eventValueBytes, headers, correlationId);
    if(produceSuccess)
    {
        return Results.Ok($"Stored");
    }
    return Results.Text(
        content: $"Storage failed",
        contentType: "text/html",
        contentEncoding: Encoding.UTF8,
        statusCode: (int?) HttpStatusCode.InternalServerError);
});

app.MapPost("/retrieve/b64", (HttpContext http, ApiParamRetrieve postContent, IKeyValueStateService keyValueStateService) =>
{
    var correlationIdValue = System.Guid.NewGuid().ToString("D");
    if(!string.IsNullOrEmpty(postContent.CorrelationId)) correlationIdValue = postContent.CorrelationId;
    var correlationId = new CorrelationId { Value = correlationIdValue };
    http.Response.Headers.Append("X-Correlation-Id", correlationId.Value);

    var keyBytes = Convert.FromBase64String(postContent.Key);
    var retrieveSuccess = keyValueStateService.TryRetrieve(keyBytes, out var valueBytes);
    var retrievedValueAsString = Convert.ToBase64String(valueBytes);
    return Results.Ok(retrievedValueAsString);
});

app.Run();
