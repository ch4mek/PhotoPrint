using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Cache;
using Modsen.Portal.TimeTrack.UseCases.Clients;
using Modsen.Portal.TimeTrack.UseCases.Consumers;
using Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Dashboard;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core.Composers;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core.Validators;
using Modsen.Portal.TimeTrack.UseCases.Options;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Reports;
using Modsen.Portal.TimeTrack.UseCases.Services;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.TimeTrackPortalSettings;

namespace Modsen.Portal.TimeTrack.UseCases;

public sealed class TimeTrackUseCasesModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        AddModule<ClientsModule>();
        AddModule<DashboardModule>();
        AddModule<ReportsModule>();

        base.Load(services);

        services.Configure<ReportProcessingOptions>(Configuration.GetSection("ReportProcessingOptions"));
        services.AddScoped<ITimeTrackContract, TimeTrackContract>();
        services.AddScoped<IUserAccessService, UserAccessService>();
        services.AddScoped<IDateService, DateService>();
        services.AddScoped<IPeriodService, PeriodService>();
        services.AddScoped<IReportNameComposer, ReportNameComposer>();
        services.AddScoped<IReportQuerySelector, ReportQuerySelector>();

        services.AddScoped<IGetTimeTrackUserDbQuery, GetTimeTrackUserDbQuery>();
        services.AddScoped<IIsUserManagerOfAnyPublicProjectDbQuery, IsUserManagerOfAnyPublicProjectDbQuery>();

        services.AddScoped<IProjectQueryBuilder, ProjectQueryBuilder>();
        services.AddScoped<ICanUserGetProjectBusinessRule, CanUserGetProjectBusinessRule>();
        services.AddScoped<ICanUserReportBeExportedBusinessRule, CanUserReportBeExportedBusinessRule>();
        services.AddScoped<ICanUserProjectBeCreatedBusinessRule, CanUserProjectBeCreatedBusinessRule>();
        services.AddScoped<ICanTrackBillableHoursBusinessRule, CanTrackBillableHoursBusinessRule>();
        services.AddScoped<ICanUserBeProjectManagerBusinessRule, CanUserBeProjectManagerBusinessRule>();
        services.AddScoped<ICanTimeTrackUserBeRetrievedBusinessRule, CanTimeTrackUserBeRetrievedBusinessRule>();
        services.AddScoped<ICanTimeTrackUserBeRetrievedWithSelfBusinessRule, CanTimeTrackUserBeRetrievedWithSelfBusinessRule>();
        services.AddScoped<IGetAccessedDepartmentIdForCurrentUserBusinessRule, GetAccessedDepartmentIdForCurrentUserBusinessRule>();
        services.AddScoped<ICanGetSubmitStatusBusinessRule, CanGetSubmitStatusBusinessRule>();
        services.AddScoped<ICanModifyTimesheetForDateBusinessRule, CanModifyTimesheetForDateBusinessRule>();
        services.AddScoped<ICanUserProjectAssignmentBeUpdatedBusinessRule, CanUserProjectAssignmentBeUpdatedBusinessRule>();

        services.AddScoped<ICanUserProjectBeDecreasedBusinessRule, CanUserProjectBeDecreasedBusinessRule>();
        services.AddScoped<ICanUserProjectBeCreatedBusinessRule, CanUserProjectBeCreatedBusinessRule>();

        services.AddScoped<IIsUserSalesManagerBusinessRule, IsUserSalesManagerBusinessRule>();

        services.AddScoped<ITimeTrackPortalSettingsAccessor, TimeTrackPortalSettingsAccessor>();

        services.AddScoped<IGetTimesheetsByPeriodWithFilterDbQuery, GetTimesheetsByPeriodWithFilterDbQuery>();
        services.AddScoped<IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery, GetTimesheetsWithClientInfoByPeriodWithFilterDbQuery>();

        services.AddScoped<IReportGenerationQueueCache, ReportGenerationQueueCache>();
        services.AddScoped<IRecyclableCache>(sp => sp.GetRequiredService<IReportGenerationQueueCache>());

        services.AddScoped<IJiraTimesheetImportComposer, JiraTimesheetImportComposer>();
        services.AddScoped<IGetJiraSettingDbQuery, GetJiraSettingDbQuery>();
        services.AddScoped<IJiraSettingsDataProtectionFactory, JiraSettingsDataProtectionFactory>();
        services.AddScoped<IJiraSettingValidator, JiraSettingValidator>();
    }

    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new UserActivatedEventConsumer.RegistrationDefinition();
        yield return new UserCreatedEventConsumer.RegistrationDefinition();
        yield return new UserDeactivatedEventConsumer.RegistrationDefinition();
        yield return new UserSyncEventConsumer.RegistrationDefinition();
        yield return new UserUpdatedEventConsumer.RegistrationDefinition();
        yield return new ResourceGroupCreatedEventConsumer.RegistrationDefinition();
        yield return new ResourceGroupUpdatedEventConsumer.RegistrationDefinition();
        yield return new ResourceGroupSyncEventConsumer.RegistrationDefinition();
    }
}
