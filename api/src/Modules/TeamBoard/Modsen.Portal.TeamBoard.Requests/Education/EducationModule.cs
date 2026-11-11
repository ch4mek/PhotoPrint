using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Education.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Education.Core.Calculators;

namespace Modsen.Portal.TeamBoard.Requests.Education;

internal sealed class EducationModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ICanUpdateUserEducationBusinessRule, CanUpdateUserEducationBusinessRule>();
        services.AddScoped<ICanGetUserEducationBusinessRule, CanGetUserEducationBusinessRule>();

        services.AddScoped<IEducationLevelOrderWeightCalculator, EducationLevelOrderWeightCalculator>();
        services.AddScoped<IStudyFormOrderWeightCalculator, StudyFormOrderWeightCalculator>();
    }
}
