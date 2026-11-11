using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.ApiHandlers;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Encryptor;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary;

internal sealed class UserSalaryModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ICanUserAccessSalaryBusinessRule, CanUserAccessSalaryBusinessRule>();

        services.AddScoped<ISalaryEncryptor, SalaryEncryptor>();

        services.AddScoped<IUserSalaryApi, UserSalaryApi>();
        services.AddScoped<IUpdateUserSalaryListWithCreatingApiHandler, UpdateUserSalaryListWithCreatingApiHandler>();
    }
}
