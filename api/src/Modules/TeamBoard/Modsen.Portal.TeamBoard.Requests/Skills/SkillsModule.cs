using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Resolvers;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills;

namespace Modsen.Portal.TeamBoard.Requests.Skills;

internal sealed class SkillsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        AddModule<HardSkillsModule>();
        AddModule<SoftSkillsModule>();
        AddModule<SpecializationTemplatesModule>();
        AddModule<UserSnapshotsModule>();
        AddModule<UserSoftSkillsModule>();
        base.Load(services);

        services.AddScoped<ISpecializationSnapshotComposer, SpecializationSnapshotComposer>();
        services.AddScoped<ISpecializationSeniorityResolver, SpecializationSeniorityResolver>();
        services.AddScoped<IUserSkillsSnapshotLoader, UserSkillsSnapshotLoader>();
        services.AddScoped<IUserSkillsSnapshotsLoader, UserSkillsSnapshotsLoader>();
        services.AddScoped<IUserSpecializationLoader, UserSpecializationLoader>();
        services.AddScoped<IAreAllHardSkillsListedBusinessRule, AreAllHardSkillsListedBusinessRule>();
    }
}
