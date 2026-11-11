using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.UseCases;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.Encryptor;

public interface IProjectRateEncryptor : INumberEncryptor;

internal sealed class ProjectRateEncryptor : NumberEncryptorBase, IProjectRateEncryptor
{
    public ProjectRateEncryptor(IDataProtectionFactory factory)
        : base(factory, NumberEncryptorScopes.ProjectRate.GetDescription())
    {
    }
}
