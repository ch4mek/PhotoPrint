using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamWork.UseCases.TeamWorkPortalSettings;

namespace Modsen.Portal.TeamWork.UseCases;

public sealed class TeamWorkUseCasesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ITeamWorkPortalSettingsAccessor, TeamWorkPortalSettingsAccessor>();
    }
}
