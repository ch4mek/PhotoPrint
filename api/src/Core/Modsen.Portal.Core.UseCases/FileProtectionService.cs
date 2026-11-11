using System.Security.Cryptography;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Core.UseCases;

public class FileProtectionService : IFileProtectionService
{
    private readonly byte[] _key;

    public FileProtectionService(string key)
    {
        _key = Convert.FromBase64String(key);
    }

    public byte[]? Encrypt(byte[]? fileData)
    {
        if (fileData == null || fileData.Length == 0)
        {
            return null;
        }

        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();

            using var memoryStream = new MemoryStream();
            memoryStream.Write(aes.IV, 0, aes.IV.Length);

            using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cryptoStream.Write(fileData, 0, fileData.Length);
            }

            return memoryStream.ToArray();
        }
        catch (CryptographicException)
        {
            throw new InvalidOperationException("Some error are occured during the encryption.");
        }
    }

    public byte[]? Decrypt(byte[]? encryptedFileData)
    {
        if (encryptedFileData == null || encryptedFileData.Length < 16)
        {
            return null;
        }

        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;

            var iv = new byte[16];
            Array.Copy(encryptedFileData, 0, iv, 0, 16);
            aes.IV = iv;

            using var transform = aes.CreateDecryptor();
            using var memoryStream = new MemoryStream();
            using var inputMemoryStream = new MemoryStream(encryptedFileData, 16, encryptedFileData.Length - 16);
            using (var cryptoStream = new CryptoStream(inputMemoryStream, transform, CryptoStreamMode.Read))
            {
                cryptoStream.CopyTo(memoryStream);
            }

            return memoryStream.ToArray();
        }
        catch (CryptographicException)
        {
            throw new InvalidOperationException("Some error are occured during the decryption.");
        }
    }
}
