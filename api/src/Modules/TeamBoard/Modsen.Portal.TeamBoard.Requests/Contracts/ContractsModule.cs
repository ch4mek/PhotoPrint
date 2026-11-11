using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Contracts.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Contracts;

internal sealed class ContractsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IIsUserHasPermissionsToViewContractsBusinessRule, IsUserHasPermissionsToViewContractsBusinessRule>();
        services.AddScoped<IIsUserHasPermissionsToMutateContractsBusinessRule, IsUserHasPermissionsToMutateContractsBusinessRule>();
        services.AddScoped<IIsFireDateLaterThanHireDateDbQuery, IsFireDateLaterThanHireDateDbQuery>();
    }
}
