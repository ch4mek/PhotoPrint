using Modsen.Portal.Agent;
using Modsen.Portal.Api.Context;
using Modsen.Portal.Api.Jobs;
using Modsen.Portal.Attendance.Controllers;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.Requests;
using Modsen.Portal.Authentication.Microsoft;
using Modsen.Portal.BackgroundJob.Hangfire;
using Modsen.Portal.BcseApi.Adapter;
using Modsen.Portal.Booking.Controllers;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.Requests;
using Modsen.Portal.Certificate.UseCases.Requests;

using Modsen.Portal.CloudStorage.AwsS3;
using Modsen.Portal.Core.Authentication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Csv;
using Modsen.Portal.Cv.Controllers;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.Requests;
using Modsen.Portal.Equipment.Controllers;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.Requests;
using Modsen.Portal.Excel;
using Modsen.Portal.Finance.Controllers;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Controllers;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests;
using Modsen.Portal.Inquiries.Contract;
using Modsen.Portal.Inquiries.Controllers;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests;
using Modsen.Portal.Jira;
using Modsen.Portal.KnowledgeBase.Controllers;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests;
using Modsen.Portal.Merch.Controllers;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests;
using Modsen.Portal.Migrator;
using Modsen.Portal.ModsenApi.Adapter;
using Modsen.Portal.Notification.Contract;
using Modsen.Portal.Notification.Controllers;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Notification.UseCases;
using Modsen.Portal.PowerAutomate;
using Modsen.Portal.StimulsoftReport;
using Modsen.Portal.Storage.Contract;
using Modsen.Portal.Storage.Controllers;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.Requests;
using Modsen.Portal.TeamBoard.Controllers;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests;
using Modsen.Portal.TeamWork.UseCases;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Controllers;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Clients;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.DbQueries;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Timesheets;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Users;
using Modsen.Portal.TimeTrack.UseCases;
using Modsen.Portal.TimeTrack.UseCases.Timesheets;
using Modsen.Portal.Utilities.Controllers;

namespace Modsen.Portal.Api.Modules
{
    public sealed class ApiServiceModule : RegistrationModule
    {
        private readonly RegistrationModuleAggregate _rootModule;

        public ApiServiceModule(RegistrationModuleAggregate rootModule)
        {
            _rootModule = rootModule;
        }

        public override void Load(IServiceCollection services)
        {
            RegisterBookingModules();
            RegisterEquipmentModules();
            RegisterFinanceModule();
            RegisterIdentityModule();
            RegisterTimeTrackModule();
            RegisterApiModules();
            RegisterTeamBoardModule();
            RegisterCertificatesModule();
            RegisterStorageModule();
            RegisterNotificationModule();
            RegisterContext(services);
            RegisterTeamWorkModule();
            RegisterKnowledgeBaseModule();
            RegisterInquiriesModule();
            RegisterMerchModule();            
            RegisterUtilitiesModule();
            RegisterChatGptModule();
            RegisterPowerAutomateModule();
            RegisterBackgroundJobs(services);
            RegisterCvModule();
            RegisterInfrastructureModules();
            RegisterAttendanceModule();
            RegisterJiraModule();

            base.Load(services);
        }

        private void RegisterInfrastructureModules()
        {
            AddModule<MigratorModule>();
            AddModule<ModsenApiAdapterModule>();
            AddModule<CsvModule>();
            AddModule<ExcelModule>();
            AddModule<StimulsoftReportModule>();
            AddModule<BcseApiAdapterModule>();

            AddModule<MicrosoftAuthenticationModule>();

            AddModule<S3Module>();
            AddModule<HangfireModule>();
            AddModule(new AuthenticationModule(_rootModule));
        }

        private void RegisterBookingModules()
        {
            AddModule<BookingDataAccessModule>();
            AddModule<BookingUseCasesModule>();
            AddModule<BookingControllersModule>();
        }

        private void RegisterEquipmentModules()
        {
            AddModule<EquipmentDataAccessModule>();
            AddModule<EquipmentRequestsModule>();
            AddModule<EquipmentControllersModule>();
        }
        private void RegisterFinanceModule()
        {
            AddModule<FinanceDataAccessModule>();
            AddModule<FinanceRequestsModule>();
            AddModule<FinanceControllersModule>();
        }

        private void RegisterIdentityModule()
        {
            AddModule<IdentityDataAccessModule>();
            AddModule<IdentityControllersModule>();
            AddModule<IdentityContractModule>();
            AddModule<IdentityUseCaseModule>();
        }

        private void RegisterTimeTrackModule()
        {
            AddModule<TimeTrackDataAccessModule>();
            AddModule<TimeTrackUseCasesModule>();
            AddModule<TimesheetsModule>();
            AddModule<TimesheetDbQueriesModule>();
            AddModule<ProjectsDbQueriesModule>();
            AddModule<ClientsDbQueriesModule>();
            AddModule<TimeTrackContractsModule>();
            AddModule<TimeTrackContractModule>();
            AddModule<UsersDbQueriesModule>();
        }

        private void RegisterTeamWorkModule()
        {
            AddModule<TeamWorkUseCasesModule>();
        }

        private void RegisterApiModules()
        {
            AddModule<DataProtectionModule>();
            AddModule<OutboxEventModule>();
        }

        private void RegisterTeamBoardModule()
        {
            AddModule<TeamBoardDataAccessModule>();
            AddModule<TeamBoardRequestsModule>();
            AddModule<TeamBoardContractsModule>();
            AddModule<TeamBoardControllersModule>();
        }

        private void RegisterCertificatesModule()
        {
            AddModule<CertificatesUseCasesModule>();
        }

        private void RegisterStorageModule()
        {
            AddModule<StorageDataAccessModule>();
            AddModule<StorageUseCaseModule>();
            AddModule<StorageControllersModule>();
            AddModule<StorageContractModule>();
        }

        private void RegisterNotificationModule()
        {
            AddModule<NotificationDataAccessModule>();
            AddModule<NotificationUseCasesModule>();
            AddModule<NotificationContractsModule>();
            AddModule<NotificationContractModule>();
        }

        private void RegisterKnowledgeBaseModule()
        {
            AddModule<KnowledgeBaseDataAccessModule>();
            AddModule<KnowledgeBaseRequestsModule>();
            AddModule<KnowledgeBaseControllersModule>();
        }

        private void RegisterInquiriesModule()
        {
            AddModule<InquiriesDataAccessModule>();
            AddModule<InquiriesRequestsModule>();
            AddModule<InquiriesControllersModule>();
            AddModule<InquiryContractModule>();
        }

        private void RegisterMerchModule()
        {
            AddModule<MerchDataAccessModule>();
            AddModule<MerchRequestsModule>();
            AddModule<MerchControllersModule>();
        }

        private void RegisterUtilitiesModule()
        {
            AddModule<UtilitiesControllersModule>();
        }

        private void RegisterChatGptModule()
        {
            AddModule<AgentModule>();
        }

        private void RegisterPowerAutomateModule()
        {
            AddModule<PowerAutomateModule>();
        }

        private static void RegisterContext(IServiceCollection services)
        {
            services.AddSingleton<IEnvironmentFeatureContext, EnvironmentFeatureContext>();
        }

        private static void RegisterBackgroundJobs(IServiceCollection services)
        {
            services.AddScoped<IRecurringJobScheduler, ClearCacheJobScheduler>();
        }

        private void RegisterCvModule()
        {
            AddModule<CvRequestsModule>();
            AddModule<CvDataAccessModule>();
            AddModule<CvControllersModule>();
        }

        private void RegisterAttendanceModule()
        {
            AddModule<AttendanceDataAccessModule>();
            AddModule<AttendanceRequestsModule>();
            AddModule<AttendanceControllersModule>();
        }

        private void RegisterJiraModule()
        {
            AddModule<JiraModule>();
        }
    }
}

