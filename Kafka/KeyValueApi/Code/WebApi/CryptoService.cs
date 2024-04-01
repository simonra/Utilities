using System;
using System.IO;
using System.Security.Cryptography;

// Mostly based on https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=net-8.0
public class CryptoService
{
    private readonly ILogger<CryptoService> _logger;

    // At this point (using specific implementations like this is deprecated now) 128 bits (16 bytes) is the only valid size in .Net
    // https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aesmanaged.blocksize?view=net-8.0#remarks
    // Seems to be the general size anyways: https://crypto.stackexchange.com/a/50786
    // But, if this is to survive over some time, specify it explicitly so that stuff doesn't randomly stop working.
    private const int AesBlockSizeBytes = 16;
    private readonly byte[] _aesKey;
    public const string aesKeyEnvVarName = "AES_KEY";

    public CryptoService(ILogger<CryptoService> logger)
    {
        _logger = logger;

        // To generate 256 bit/32 byte hex string securely enough:
        // `openssl rand -hex 32`
        // Or for rapid proof of concepting on machine without openssl this js also works:
        // `Array.from(crypto.getRandomValues(new Uint8Array(32))).map(b => b.toString(16).padStart(2, '0')).join('');`
        var aesKeyHexString = Environment.GetEnvironmentVariable(aesKeyEnvVarName);
        if(aesKeyHexString?.Length != 64)
        {
            throw new ArgumentException(message: $"Env variable {aesKeyEnvVarName} must be hex representation of 256 bits/32 bytes, that is 64 characters long, the supplied value was not", paramName: aesKeyEnvVarName);
        }
        if(!aesKeyHexString.All(c => "0123456789abcdefABCDEF".Contains(c)))
        {
            throw new ArgumentException(message: $"Env variable {aesKeyEnvVarName} must be hex value, the supplied variable contained values that are not legal hex (0-9a-fA-F)", paramName: aesKeyEnvVarName);
        }
        _aesKey = Convert.FromHexString(aesKeyHexString);
    }

    public byte[] Encrypt(byte[] unencryptedInput)
    {
        if(unencryptedInput.Length <= 0)
        {
            throw new ArgumentNullException(message: "Unencrypted input has to be supplied", paramName: "unencryptedInput");
        }
        using Aes aesAlg = Aes.Create();
        var randomPerMessageIv = GenerateKnownSizeAesIv();
        // aesAlg.KeySize = 256; // Note: regenerates the key when set
        // aesAlg.BlockSize = 128; // 128 bits is the only valid size for AES.
        // aesAlg.Mode = CipherMode.CBC; // The default is CBC.
        aesAlg.Key = _aesKey;
        aesAlg.IV = randomPerMessageIv;
        // aesAlg.Padding = PaddingMode.PKCS7; // The default is PKCS7

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using var ms = new MemoryStream();
        using var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();

        encrypted = ms.ToArray();

        // This could maybe be avoided by starting with writing the IV to the ms MemoryStream right after creating it.
        // But, it's late, and I don't have time to experiment with it now or make up opinions about future behaviour of CryptoStream.
        // (If it doesn't overwrite the stream now but only appends, will it continue to do so forever?)
        encryptedWithPrependedIv = new byte[randomPerMessageIv.Length + encrypted.Length];
        randomPerMessageIv.CopyTo(encryptedWithPrependedIv, 0);
        encrypted.CopyTo(encryptedWithPrependedIv, randomPerMessageIv.Length);
        return encryptedWithPrependedIv;
    }

    public byte[] Decrypt(byte[] encryptedInput)
    {
        if(encryptedInput.Length <= AesBlockSizeBytes + 1)
        {
            throw new ArgumentNullException(message: "Encrypted input must be at least the length of the prepended IV and an additional encrypted byte", paramName: "encryptedInput");
        }

        // Range indices are fun! https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/ranges-indexes
        var aesIv = encryptedInput[0..AesBlockSizeBytes];
        var firstMessageByte = AesBlockSizeBytes + 1;
        var encryptedMessage = encryptedInput[firstMessageByte..];

        using Aes aesAlg = Aes.Create();
        aesAlg.Key = _aesKey;
        aesAlg.IV = aesIv;
        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using var ms = new MemoryStream();
        using var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Write);
        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();

        return ms.ToArray();
    }

    private byte[] GenerateKnownSizeAesIv()
    {
        return System.Security.Cryptography.RandomNumberGenerator.GetBytes(AesBlockSizeBytes);
    }
}
