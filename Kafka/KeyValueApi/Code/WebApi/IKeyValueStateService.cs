public interface IKeyValueStateService
{
    public bool Store(byte[] key, byte[] value);
    public bool TryRetrieve(byte[] key, out byte[] value);
    public bool Remove(byte[] key);
}
