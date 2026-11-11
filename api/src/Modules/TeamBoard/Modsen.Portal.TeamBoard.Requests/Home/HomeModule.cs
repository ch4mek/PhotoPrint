using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Home.Core.Converters;

namespace Modsen.Portal.TeamBoard.Requests.Home;

internal sealed class HomeModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ITeamboardUserToDepartmentNameConverter, TeamboardUserToDepartmentNameConverter>();
    }
}
