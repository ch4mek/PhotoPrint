using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.UseCases;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Encryptor;

public interface ISalaryEncryptor : INumberEncryptor;

internal sealed class SalaryEncryptor : NumberEncryptorBase, ISalaryEncryptor
{
    public SalaryEncryptor(IDataProtectionFactory factory)
        : base(factory, NumberEncryptorScopes.Salary.GetDescription())
    {
    }
}
