using System.Collections.Generic;

public record ApiParamRetrieve
{
    public required string Key { get; init; }
    public string? CorrelationId { get; init; }
}
