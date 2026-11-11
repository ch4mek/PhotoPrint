using System.Security.Cryptography;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.UseCases;

public class DataProtectionService : IDataProtectionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public DataProtectionService(string key, string iv)
    {
        _key = Convert.FromBase64String(key);
        _iv = Convert.FromBase64String(iv);
    }

    public string? Encrypt(string? decrypted)
    {
        if (decrypted.IsNullOrEmpty())
        {
            return null;
        }

        using var aes = Aes.Create();

        aes.Key = _key;
        aes.IV = _iv;

        using var transform = aes.CreateEncryptor();
        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(decrypted);
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public string? Decrypt(string? encrypted)
    {
        if (encrypted.IsNullOrEmpty())
        {
            return null;
        }

        using var aes = Aes.Create();

        aes.Key = _key;
        aes.IV = _iv;

        string text;

        try
        {
            var encryptedBytes = Convert.FromBase64String(encrypted);

            using (var transform = aes.CreateDecryptor())
            using (var memoryStream = new MemoryStream(encryptedBytes))
            using (var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Read))
            using (var streamReader = new StreamReader(cryptoStream))
            {
                text = streamReader.ReadToEnd();
            }
        }
        catch (Exception)
        {
            throw new InvalidDataAppException();
        }


        return text;
    }
}
