using System.Reflection;
using Hangfire;
using Modsen.Portal.Api.Hangfire;
using Modsen.Portal.Api.Middleware;
using Modsen.Portal.Api.Modules;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.HostBuilder;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Migrator;
using Modsen.Portal.Notification.Controllers.Hubs;

var version = Assembly.GetEntryAssembly()?
    .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
    .Cast<AssemblyInformationalVersionAttribute>()
    .FirstOrDefault()?
    .InformationalVersion ?? "n/a";

await ServiceHostBuilder
    .Create(args, "Portal API", version, PortalSharedConstants.EnvVariablesApiPrefix)
    .WithModule((_, rootModule) => new ApiServiceModule(rootModule))
    .WithSignalR()
    .WithErrorHandlerMiddleware<ErrorHandlerMiddleware>()
    .ConfigureMiddleware(app =>
    {
        app.MapHub<NotificationsHub>("/hubs/notifications");
        app.UseHangfireDashboard("/hangfire-dashboard", new DashboardOptions
        {
            DashboardTitle = "Modsen Background Services",
            DarkModeEnabled = false,
            DefaultRecordsPerPage = 20,
            AsyncAuthorization =
            [
                new HangfireDashboardJwtAuthorizationFilter()
            ]
        });
    })
    .Build()
    .ExecuteInScope(serviceProvider =>
    {
        serviceProvider.MigrateMigratorDb();
        serviceProvider.MigrateTeamBoardDb();
        serviceProvider.MigrateIdentityDb();
        serviceProvider.MigrateTimeTrackDb();
        serviceProvider.MigrateBookingDb();
        serviceProvider.MigrateEquipmentDb();
        serviceProvider.MigrateStorageDb();
        serviceProvider.MigrateNotificationDb();
        serviceProvider.MigrateKnowledgeBaseDb();
        serviceProvider.MigrateInquiriesDb();
        serviceProvider.MigrateMerchDb();
        serviceProvider.MigrateCvDb();
        serviceProvider.MigrateAttendancesDb();
        serviceProvider.MigrateFinanceDb();
        serviceProvider.ExecuteIdentitySeed();
        serviceProvider.ExecuteFinanceSeed();

        serviceProvider.ApplyMigrationTasks([
            "PublishExistingUsersToBooking",
        ]);

        serviceProvider.ApplyMigrationTasks([
            "PublishExistingUsersToEquipment",
        ]);

        serviceProvider.ApplyMigrationTasks([
            "PublishExistingDepartmentsToTeamBoard",
            "PublishExistingDepartmentsToFinance"
        ]);

        serviceProvider.ApplyMigrationTasks([
            "PublishExistingResourceGroupsToTeamBoard",
            "PublishExistingResourceGroupsToTimeTrack",
            "PublishExistingResourceGroupsToAttendance",
        ]);

        serviceProvider.ApplyMigrationTasks([
            "PublishExistingUsersToTeamBoard",
            "PublishExistingUsersToTimeTrack",
            "PublishExistingUsersToKnowledgeBase",
            "PublishExistingUsersToInquiries",
            "PublishExistingUsersToMerch",
            "PublishExistingUsersToCv",
            "PublishExistingUsersToAttendance",
            "PublishExistingUsersToFinance",
            "PublishExistingUsersToNotification",
        ]);

        serviceProvider.ExecuteTimeTrackSeed();
        serviceProvider.ExecuteBookingSeed();
        serviceProvider.ExecuteEquipmentSeed();
        serviceProvider.ExecuteTeamBoardSeed();
        serviceProvider.ExecuteInquiriesSeed();
        serviceProvider.ExecuteKnowledgeBaseSeed();
        serviceProvider.ExecuteMerchSeed();

        serviceProvider.ApplyMigrationTasks([
            "PublishExistingDepartmentsToEquipment",
        ]);

        serviceProvider.ApplyMigrationTasks([
            "PublishExistingUsersCountriesToTeamBoard",
            "PublishExistingUsersRegisteredLocationsToTeamBoard",
            "PublishExistingProjectsToTeamBoard",
            "PublishExistingProjectUsersToTeamBoard",
            "PublishExistingClientsToCv",
            "PublishExistingProjectsToCv",
            "PublishExistingClientsToTeamBoard",
        ]);
        serviceProvider.ApplyMigrationTasks([
            "PublishExistingClientsToFinance",
        ]);
        serviceProvider.ApplyMigrationTasks([
            "PublishExistingProjectsToFinance",
            "PublishExistingProjectUsersToFinance"
        ]);


        serviceProvider.ExecuteCvSeed();
        serviceProvider.ExecuteAttendancesSeed();

        serviceProvider.VerifyDataConsistency();
    })
    .ExecuteInScope(serviceProvider =>
    {
        serviceProvider.GetRequiredService<IPortalSettingsAccessor>().Clear();
        serviceProvider.ScheduleRecurringJobs();
    })
    .RunAsync();

namespace Modsen.Portal.Api
{
    public class Program;
}
