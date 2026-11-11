using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TeamBoard.Requests.Payload.Core;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.Loaders;

namespace Modsen.Portal.TeamBoard.Requests.Payload;

internal sealed class PayloadModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ITotalUserTimeByMonthsCalculator, TotalUserTimeByMonthsCalculator>();
        services.AddScoped<IPayloadUserDetailsLoader, PayloadUserDetailsLoader>();
        services.AddScoped<IGetPayloadUsersQueryBuilder, GetPayloadUsersQueryBuilder>();
        services.AddScoped<IEmployeesPayloadComposer, EmployeesPayloadComposer>();
        services.AddScoped<ITotalEmployeesPayloadCalculator, TotalEmployeesPayloadCalculator>();
        services.AddScoped<IPayloadCalendarWorkingHourDetailsLoader, PayloadCalendarWorkingHourDetailsLoader>();
        services.AddScoped<IWorkingDaysCalculator, WorkingDaysCalculator>();
        services.AddScoped<IUserProjectHoursCalculator, UserProjectHoursCalculator>();
        services.AddScoped<IIsUserAssignedBusinessRule, IsUserAssignedBusinessRule>();
        services.AddScoped<IWorkingHoursInMonthCalculator, WorkingHoursInMonthCalculator>();
        services.AddScoped<IUserHoursCalculator, UserHoursCalculator>();
        services.AddScoped<IKpiColorCalculator, KpiColorCalculator>();
        services.AddScoped<IUserActivityMonthsCalculator, UserActivityMonthsCalculator>();
    }
}
