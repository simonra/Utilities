public class KeyValeStateOnFileSystemService : IKeyValueStateService
{
    private readonly ILogger<KeyValeStateOnFileSystemService> _logger;
    private readonly string _storageRootDirectoryPath;
    private readonly Func<byte[], byte[]> _encrypt;
    private readonly Func<byte[], byte[]> _decrypt;
    public KeyValeStateOnFileSystemService(ILogger<KeyValeStateOnFileSystemService> logger)
    {
        _logger = logger;
        var storageRootEnvVarName = KV_API_KEY_VALUE_STORE_ROOT_DIR;
        var storageRoot = Environment.GetEnvironmentVariable(storageRootEnvVarName);
        if(string.IsNullOrWhiteSpace(storageRoot))
        {
            if(string.IsNullOrEmpty(storageRoot))
            {
                _logger.LogWarning($"Failed to read content of environment variable \"{storageRootEnvVarName}\", got null/empty string");
            }
            else
            {
                _logger.LogWarning($"Failed to read proper content of environment variable \"{storageRootEnvVarName}\", contained only whitespaces");
            }
            storageRoot = "."; // Remove possibility of null, using . further reduces chances of doing weird stuff at root of file system
        }
        _storageRootDirectoryPath = storageRoot;
        if (Environment.GetEnvironmentVariable(KV_API_ENCRYPT_DATA_IN_STATE_STORAGE) == "true")
        {
            var cryptoService = new CryptoService();
            _encrypt = cryptoService.Encrypt;
            _decrypt = cryptoService.Decrypt;
        }
        else
        {
            _encrypt = delegate(byte[] input) { return input; };
            _decrypt = delegate(byte[] input) { return input; };
        }
        _logger.LogInformation($"{nameof(KeyValeStateOnFileSystemService)} initialized");
    }

    public bool Store(byte[] keyRaw, byte[] valueRaw)
    {
        var keyEncrypted = _encrypt(keyRaw);
        var valueEncrypted = _encrypt(valueRaw);
        // Because AES relies on prepending new random garbage every time anything is encrypted, all comparisons have to be against unencrypted.
        var directory = GetDirectoryForKey(keyRaw);
        // For future me: Directory.CreateDirectory() handles the case where it already exists.
        // So same as with mkdir -p, call it just in case instead of bloating path with if()'s.
        Directory.CreateDirectory(directory); // Create if not exists
        string[] preExistingFiles = Directory.GetFiles(directory);
        var keyPath = string.Empty;
        var valuePath = string.Empty;
        if(preExistingFiles.Length == 0)
        {
            keyPath = $"{directory}/0.key";
            valuePath = $"{directory}/0.value";
            File.WriteAllBytes(keyPath, keyEncrypted);
            File.WriteAllBytes(valuePath, valueEncrypted);
            return true;
        }
        var keyFiles = preExistingFiles.Where(fileName => fileName.EndsWith(".key")).ToArray();
        foreach(var keyFile in keyFiles)
        {
            if(_decrypt(File.ReadAllBytes(keyFile)).SequenceEqual(keyRaw))
            {
                var associatedValueFile = keyFile[0..^3] + "value";
                File.WriteAllBytes(associatedValueFile, valueEncrypted);
                return true;
            }
        }

        // Detect holes, and insert in hole or if none at end
        var keyFileNames = keyFiles
            .Select(fullPath => Path.GetFileNameWithoutExtension(fullPath))
            .ToArray();
        var fileNumbers = new List<int>();
        foreach(var keyFileName in keyFileNames)
        {
            if(int.TryParse(keyFileName, out var baseName) && 0 <= baseName)
            {
                fileNumbers.Add(baseName);
            }
        }
        fileNumbers.Sort();

        var nextAvailableBaseName = fileNumbers.Count;

        if(fileNumbers[^1] != fileNumbers.Count - 1)
        {
            // Here there be gaps. Unless there are duplicates. But that is hard to imagine (I don't know any file system allowing for duplicate names). Or someone snuck in a weird value that is parsable as an int. At which point, shrek it, the retrieval will end up finding this anyways.
            for(int i = 0; i < fileNumbers.Count; i++)
            {
                if(fileNumbers[i] != i)
                {
                    nextAvailableBaseName = i;
                    break;
                }
            }
        }
        keyPath = $"{directory}/{nextAvailableBaseName}.key";
        valuePath = $"{directory}/{nextAvailableBaseName}.value";
        File.WriteAllBytes(keyPath, keyEncrypted);
        File.WriteAllBytes(valuePath, valueEncrypted);
        return true;
    }

    public bool TryRetrieve(byte[] keyRaw, out byte[] value)
    {
        // var keyEncrypted = _encrypt(keyRaw);
        var directory = GetDirectoryForKey(keyRaw);
        if(!Directory.Exists(directory))
        {
            value = [];
            return false;
        }
        string[] preExistingFiles = Directory.GetFiles(directory);
        var keyFiles = preExistingFiles.Where(fileName => fileName.EndsWith(".key")).ToArray();
        foreach(var keyFile in keyFiles)
        {
            if(_decrypt(File.ReadAllBytes(keyFile)).SequenceEqual(keyRaw))
            {
                var associatedValueFile = keyFile[0..^3] + "value";
                if(File.Exists(associatedValueFile))
                {
                    value = _decrypt(File.ReadAllBytes(associatedValueFile));
                    return true;
                }
                else
                {
                    _logger.LogWarning($"On retrieve found matching key at path {keyFile}, but no corresponding file/path for the value as expected at {associatedValueFile}. Checking other keys jut to be sure, but something truly weird is going on.");
                    // But it matters not, if we for some inexplicable reason have a duplicate key, we will just find and use the first proper match.
                    // If we on the other hand don't have an actual match at all, it will fall through to the "no result" below.
                }
            }
        }
        value = [];
        return false;
    }

    public bool Remove(byte[] keyRaw)
    {
        // var keyEncrypted = _encrypt(keyRaw);
        var directory = GetDirectoryForKey(keyRaw);
        if(!Directory.Exists(directory))
        {
            _logger.LogWarning($"Someone tried to delete {directory} which doesn't exist, weird");
            return true;
        }
        string[] preExistingFiles = Directory.GetFiles(directory);
        var keyFiles = preExistingFiles.Where(fileName => fileName.EndsWith(".key")).ToArray();
        foreach(var keyFile in keyFiles)
        {
            if(_decrypt(File.ReadAllBytes(keyFile)).SequenceEqual(keyRaw))
            {
                var associatedValueFile = keyFile[0..^3] + "value";
                if(File.Exists(associatedValueFile))
                {
                    File.Delete(associatedValueFile);
                }
                else
                {
                    _logger.LogWarning($"On delete request found matching key at path {keyFile}, but no corresponding file/path for the value as expected at {associatedValueFile}.");
                }
                File.Delete(keyFile);

                try
                {
                    if(Directory.GetFiles(directory).Length == 0)
                    {
                        var parentDirectory = Directory.GetParent(directory)!.FullName;
                        Directory.Delete(directory, false);
                        if(Directory.GetDirectories(parentDirectory).Length == 0)
                        {
                            Directory.Delete(parentDirectory, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Got exception when trying to clean up directories that at the time were thought to be empty. Perhaps something new has been inserted? The directory in question/it's parent is/was \"{directory}\"");
                }
                return true;
            }
        }
        return true;
    }

    private string GetDirectoryForKey(byte[] key)
    {
        // Make directory for first 3 (16 ^ 3 = up to 4096 directories per level), then next 3, then dump files
        // var keyHash = System.IO.Hashing.Crc32.Hash(key);
        // var keyHashHex = Convert.ToHexString(keyHash).ToLowerInvariant();
        var hash = key.GetHashString();
        var firstLevel = hash.Substring(0, 3);
        var secondLevel = hash.Substring(3, 3);
        return $"{_storageRootDirectoryPath}/{firstLevel}/{secondLevel}";
    }
}
