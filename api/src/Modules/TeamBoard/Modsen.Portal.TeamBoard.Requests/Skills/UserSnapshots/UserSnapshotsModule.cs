using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.Validators;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots;

public sealed class UserSnapshotsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IGetLastSnapshotByUserIdDbQuery, GetLastSnapshotByUserIdDbQuery>();
        services.AddScoped<ICreateUserSnapshotValidator, CreateUserSnapshotValidator>();
        services.AddScoped<ICreateUserSnapshotHardSkillsListValidator, CreateUserSnapshotHardSkillsListValidator>();
        services.AddScoped<ICreateUserSnapshotValidationService, CreateUserSnapshotValidationService>();
    }
}
