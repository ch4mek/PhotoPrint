namespace Modsen.Portal.Core.Services;

public interface IDataProtectionService
{
    string? Encrypt(string? decrypted);
    string? Decrypt(string? encrypted);
}
