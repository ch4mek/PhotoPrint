using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Payslips.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Payslips.Core.Encryptor;

namespace Modsen.Portal.TeamBoard.Requests.Payslips;

public sealed class PayslipsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IPayslipEncryptor, PayslipEncryptor>();
        services.AddScoped<IIsUserHaveAccessToPayslipFileBusinessRule, IsUserHaveAccessToPayslipFileBusinessRule>();
    }
}
