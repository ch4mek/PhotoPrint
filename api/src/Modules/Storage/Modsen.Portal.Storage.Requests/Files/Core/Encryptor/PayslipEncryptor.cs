using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Storage.Requests.Files.Core.Encryptor;

public interface IPayslipEncryptor
{
    public byte[] Decrypt(byte[]? encryptedFile);
}

internal sealed class PayslipEncryptor : IPayslipEncryptor
{
    private readonly IFileProtectionService _fileProtectionService;

    public PayslipEncryptor(IFileProtectionFactory fileProtectionFactory)
    {
        _fileProtectionService = fileProtectionFactory.Create("Payslips");
    }

    public byte[] Decrypt(byte[]? encryptedFile)
    {
        if (encryptedFile is null)
        {
            return [];
        }

        return _fileProtectionService.Decrypt(encryptedFile) ?? [];
    }
}
