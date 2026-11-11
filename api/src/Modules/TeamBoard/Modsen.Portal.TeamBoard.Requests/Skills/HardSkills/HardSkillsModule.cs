using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.Builders;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.Validators;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills;

public sealed class HardSkillsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IIsHardSkillNameUniqueBusinessRule, IsHardSkillNameUniqueBusinessRule>();
        services.AddScoped<IHardSkillGroupExistsBusinessRule, HardSkillGroupExistsBusinessRule>();
        services.AddScoped<IHardSkillUpdateValidator, HardSkillUpdateValidator>();
        services.AddScoped<IGetHardSkillDbQuery, GetHardSkillDbQuery>();
        services.AddScoped<IHardSkillQueryBuilder, HardSkillQueryBuilder>();
        services.AddScoped<IHardSkillsExistBusinessRule, HardSkillsExistBusinessRule>();
    }
}
