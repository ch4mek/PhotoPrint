using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills;

public sealed class SoftSkillsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IIsSoftSkillNameUniqueBusinessRule, IsSoftSkillNameUniqueBusinessRule>();

        services.AddScoped<IGetSoftSkillDbQuery, GetSoftSkillDbQuery>();
    }
}
