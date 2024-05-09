public record KeyValue
{
    public required byte[] Key { get; init; }
    public required byte[] Value { get; init; }
}
