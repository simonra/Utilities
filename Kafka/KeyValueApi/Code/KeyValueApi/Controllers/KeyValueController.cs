// using System;

// using Microsoft.AspNetCore.Mvc;

// namespace WebApi.Controllers;

// [ApiController]
// [Route("[controller]")]
// public class KeyValueController : ControllerBase
// {
//     private readonly ILogger<KeyValueController> _logger;
//     private readonly KafkaProducerService _kafkaProducerService;
//     private readonly IKeyValueStateService _keyValueStateService;

//     public KeyValueController(ILogger<KeyValueController> logger, KafkaProducerService kafkaProducerService, IKeyValueStateService keyValueStateService)
//     {
//         _logger = logger;
//         _kafkaProducerService = kafkaProducerService;
//         _keyValueStateService = keyValueStateService;
//     }

//     [HttpPost]
//     public void SetValue(byte[] key, byte[]? value, string correlationId)
//     {
//         // Dictionary<string, byte[]> headers, CorrelationId correlationId
//         // _kafkaProducerService.Produce(key, value);
//         throw new NotImplementedException();
//     }

//     [HttpPost]
//     public IResult RetrieveValue(byte[] key, string? correlationId)
//     {
//         // if(string.IsNullOrEmpty(correlationId)) correlationId = System.Guid.NewGuid().ToString("D");
//         // var valueIsFound = _keyValueStateService.TryRetrieve(key, out byte[] valueRaw);
//         // // if(!valueIsFound)
//         // // {
//         // //     return Results.NoContent();
//         // // }
//         // var result = Results.Bytes(valueRaw, );
//         // // Results.
//         // return valueRaw;
//         // var valueEncoded = Convert.ToBase64String(valueRaw);
//         // return new ApiRetrieveResult
//         // {
//         //     CorrelationId = correlationId,
//         //     ValueB64 = valueEncoded,
//         //     KafkaEventMetadata = new KafkaEventMetadata { Topic = }
//         // };

//         // // var ApiRetrieveResult = new ApiRetrieveResult {
//         // //     ValueB64 = messageValue,
//         // //     CorrelationId = correlationId,
//         // // };
//         throw new NotImplementedException();
//     }

//     // [HttpPost]
//     // public ApiRetrieveResult RetrieveValue(byte[] key, string? correlationId)
//     // {
//     //     if(string.IsNullOrEmpty(correlationId)) correlationId = System.Guid.NewGuid().ToString("D");
//     //     var valueIsFound = _keyValueStateService.TryRetrieve(key, out byte[] valueRaw);
//     //     var valueEncoded = Convert.ToBase64String(valueRaw);
//     //     return new ApiRetrieveResult
//     //     {
//     //         CorrelationId = correlationId,
//     //         ValueB64 = valueEncoded,
//     //         KafkaEventMetadata = new KafkaEventMetadata { Topic = }
//     //     };

//     //     // var ApiRetrieveResult = new ApiRetrieveResult {
//     //     //     ValueB64 = messageValue,
//     //     //     CorrelationId = correlationId,
//     //     // };
//     //     throw new NotImplementedException();
//     // }
// }
