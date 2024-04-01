using System.Collections.Generic;

public record ApiSetParam
{
    public required string KeyB64 { get; init; }
    public required string ValueB64 { get; init; }
    public Dictionary<string, string>? KafkaHeaders { get; init; }
    public string? CorrelationId { get; init; }
}
