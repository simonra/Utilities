using System.Collections.Generic;

public record ApiParamStore
{
    public required string Key { get; init; }
    public required string Value { get; init; }
    public Dictionary<string, string>? Headers { get; init; }
    public string? CorrelationId { get; init; }
}
