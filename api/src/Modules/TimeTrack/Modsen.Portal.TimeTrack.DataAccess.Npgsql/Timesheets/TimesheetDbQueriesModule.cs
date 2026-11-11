using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Timesheets;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Users;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Timesheets;

public sealed class TimesheetDbQueriesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IIsTimesheetUserFromDepartmentDbQuery, IsTimesheetUserFromDepartmentDbQuery>();
        services.AddScoped<ICreateReportRequestDbQuery, CreateReportRequestDbQuery>();
        services.AddScoped<IGetExistingReportRequestDbQuery, GetExistingReportRequestDbQuery>();
        services.AddScoped<IIsTimeTrackUserExistingDbQuery, IsTimeTrackUserExistingDbQuery>();
        services.AddScoped<IIsTimesheetByProjectIdAndUserIdFilledDbQuery, IsTimesheetByProjectIdAndUserIdFilledDbQuery>();
        services.AddScoped<IGetTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery, GetTimesheetByProjectIdAndUserIdAndDateIntervalDbQuery>();
        services.AddScoped<IGetBillableProjectsIdsDbQuery, GetBillableProjectsIdsDbQuery>();
        services.AddScoped<IGetUsersWithInactiveTimeTrackingDbQuery, GetUsersWithInactiveTimeTrackingDbQuery>();
        services.AddScoped<IGetUserIdsByResourceGroupDbQuery, GetUserIdsByResourceGroupDbQuery>();
    }
}
