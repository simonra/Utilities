using System;

using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class KeyValueController : ControllerBase
{
    private readonly ILogger<KeyValueController> _logger;
    private readonly KafkaProducerService _kafkaProducerService;
    private readonly KeyValueStateService _keyValueStateService;

    public KeyValueController(ILogger<KeyValueController> logger, KafkaProducerService kafkaProducerService, KeyValueStateService keyValueStateService)
    {
        _logger = logger;
        _kafkaProducerService = kafkaProducerService;
        _keyValueStateService = keyValueStateService;
    }

    [HttpPost]
    public void SetValue(byte[] key, byte[]? value, string correlationId)
    {
        // Dictionary<string, byte[]> headers, CorrelationId correlationId
        // _kafkaProducerService.Produce(key, value);
        throw new NotImplementedException();
    }

    [HttpPost]
    public ApiRetrieveResult RetrieveValue(byte[] key, string? correlationId)
    {
        if(string.IsNullOrEmpty(correlationId)) correlationId = System.Guid.NewGuid().ToString("D");
        var messageValue = _keyValueStateService.GetValue(key);
        // var ApiRetrieveResult = new ApiRetrieveResult {
        //     ValueB64 = messageValue,
        //     CorrelationId = correlationId,
        // };
        throw new NotImplementedException();
    }
}
