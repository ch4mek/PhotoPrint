using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Calculators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Composers;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Loaders;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Resolvers;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets;

public sealed class TimesheetsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<ITimesheetQueryBuilder, TimesheetQueryBuilder>();

        services.AddScoped<ICanGetSubmitStatusBusinessRule, CanGetSubmitStatusBusinessRule>();
        services.AddScoped<ICanGetUserTimesheetBusinessRule, CanGetUserTimesheetBusinessRule>();
        services.AddScoped<ICanModifyTimesheetForDateBusinessRule, CanModifyTimesheetForDateBusinessRule>();
        services.AddScoped<ICanTrackBillableHoursBusinessRule, CanTrackBillableHoursBusinessRule>();
        services.AddScoped<ICanUserUpdateTimesheetBusinessRule, CanUserUpdateTimesheetBusinessRule>();
        services.AddScoped<IHasUserActiveAssignmentForDateBusinessRule, HasUserActiveAssignmentForDateBusinessRule>();
        services.AddScoped<INewTaskNameHasNoCollisionsBusinessRule, NewTaskNameHasNoCollisionsBusinessRule>();

        services.AddScoped<ITimesheetBlockedDaysCalculator, TimesheetBlockedDaysCalculator>();

        services.AddScoped<ISubmitStatusComposer, SubmitStatusComposer>();
        services.AddScoped<ITimesheetComposer, TimesheetComposer>();

        services.AddScoped<IGetProjectBillableStatusIfExistsDbQuery, GetProjectBillableStatusIfExistsDbQuery>();
        services.AddScoped<IGetProjectUserBillableStatusDbQuery, GetProjectUserBillableStatusDbQuery>();
        services.AddScoped<IGetUserActiveStatusDbQuery, GetUserActiveStatusDbQuery>();
        services.AddScoped<IGetTimesheetDbQuery, GetTimesheetDbQuery>();
        services.AddScoped<IGetTimesheetSubmitStatusDbQuery, GetTimesheetSubmitStatusDbQuery>();
        services.AddScoped<IGetProjectActiveStatusDbQuery, GetProjectActiveStatusDbQuery>();

        services.AddScoped<IPageableUserTimesheetSnapshotsLoader, PageableUserTimesheetSnapshotsLoader>();

        services.AddScoped<ICreateTimesheetResolver, CreateTimesheetResolver>();
        services.AddScoped<IUpdateTimesheetResolver, UpdateTimesheetResolver>();

        services.AddScoped<ICreateTimesheetValidator, CreateTimesheetValidator>();
        services.AddScoped<IDeleteTimesheetValidator, DeleteTimesheetValidator>();
        services.AddScoped<IGetTimesheetValidator, GetTimesheetValidator>();
        services.AddScoped<IUpdateSubmitStatusValidator, UpdateSubmitStatusValidator>();
        services.AddScoped<IUpdateTaskNameValidator, UpdateTaskNameValidator>();
        services.AddScoped<IUpdateTimesheetValidator, UpdateTimesheetValidator>();
    }
}
