using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.Validators;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills;

public sealed class UserSoftSkillsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IUserSoftSkillsSnapshotComposer, UserSoftSkillsSnapshotComposer>();

        services.AddScoped<IUserSoftSkillSnapshotValidator, UserSoftSkillSnapshotValidator>();
    }
}
