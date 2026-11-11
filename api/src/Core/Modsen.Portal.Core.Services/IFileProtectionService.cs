namespace Modsen.Portal.Core.Services;

public interface IFileProtectionService
{
    byte[]? Encrypt(byte[]? fileData);
    byte[]? Decrypt(byte[]? encryptedFileData);
}
