public record ApiParamRemove
{
    public required string Key { get; init; }
    public string? CorrelationId { get; init; }
}
