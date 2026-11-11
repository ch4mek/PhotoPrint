using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TeamBoard.Requests.Payslips.Core.Encryptor;

public interface IPayslipEncryptor
{
    public byte[]? Encrypt(byte[]? payslipFile);
}

internal sealed class PayslipEncryptor : IPayslipEncryptor
{
    private readonly IFileProtectionService _fileProtectionService;

    public PayslipEncryptor(IFileProtectionFactory fileProtectionFactory)
    {
        _fileProtectionService = fileProtectionFactory.Create("Payslips");
    }

    public byte[]? Encrypt(byte[]? payslipFile)
    {
        if (payslipFile is null || payslipFile.LongLength == 0)
        {
            return null;
        }

        return _fileProtectionService.Encrypt(payslipFile);
    }
}
