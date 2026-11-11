using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Migrator.DataAccess;
using Modsen.Portal.Migrator.MigrationTasks;

namespace Modsen.Portal.Migrator;

public sealed class MigratorModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<MigratorDbContext>((serviceProvider, optionsBuilder) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();

            optionsBuilder.UseNpgsql(factory.GetConnection(),
                pgOptions =>
                    pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, MigratorDbContext.Schema));
        });

        services.AddScoped<IMigrationTask, PublishExistingUsersToBookingMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingUsersToEquipmentMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingDepartmentsToTeamBoardMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingResourceGroupsToTeamBoardMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingResourceGroupsToTimeTrackMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingResourceGroupsToAttendanceMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingUsersToTeamBoardMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingUsersCountriesToTeamBoardMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingUsersRegisteredLocationsToTeamBoardMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingUsersToTimeTrackMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingDepartmentsToEquipmentMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingProjectsToTeamBoardMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingProjectUsersToTeamBoardMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingUsersToKnowledgeBaseMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingUsersToInquiriesMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingUsersToMerchMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingUsersToCvMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingUsersToAttendanceMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingClientsToCvMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingProjectsToCvMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingClientsToTeamBoardMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingUsersToNotificationMigrationTask>();

        //Finance Migration Tasks
        services.AddScoped<IMigrationTask, PublishExistingUsersToFinanceMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingDepartmentsToFinanceMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingProjectsToFinanceMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingClientsToFinanceMigrationTask>();
        services.AddScoped<IMigrationTask, PublishExistingProjectUsersToFinanceMigrationTask>();
    }
}
