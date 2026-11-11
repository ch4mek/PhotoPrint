using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Migrator.DataAccess;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Npgsql;

namespace Modsen.Portal.Migrator;

public static class PortalMigrator
{
    public static void MigrateMigratorDb(this IServiceProvider serviceProvider)
    {
        using var context = new MigratorDbContext(
            serviceProvider.CreateContextOptions<MigratorDbContext>(MigratorDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateIdentityDb(this IServiceProvider serviceProvider)
    {
        using var context = new IdentityModuleDbContext(
            serviceProvider.CreateContextOptions<IdentityModuleDbContext>(IdentityModuleDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateTimeTrackDb(this IServiceProvider serviceProvider)
    {
        using var context = new TimeTrackDbContext(
            serviceProvider.CreateContextOptions<TimeTrackDbContext>(TimeTrackDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateBookingDb(this IServiceProvider serviceProvider)
    {
        using var context = new BookingDbContext(
            serviceProvider.CreateContextOptions<BookingDbContext>(BookingDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateEquipmentDb(this IServiceProvider serviceProvider)
    {
        using var context = new EquipmentDbContext(
            serviceProvider.CreateContextOptions<EquipmentDbContext>(EquipmentDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateTeamBoardDb(this IServiceProvider serviceProvider)
    {
        using var context = new TeamBoardDbContext(
            serviceProvider.CreateContextOptions<TeamBoardDbContext>(TeamBoardDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateStorageDb(this IServiceProvider serviceProvider)
    {
        using var context = new StorageDbContext(
            serviceProvider.CreateContextOptions<StorageDbContext>(StorageDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateNotificationDb(this IServiceProvider serviceProvider)
    {
        using var context = new NotificationDbContext(
            serviceProvider.CreateContextOptions<NotificationDbContext>(NotificationDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateKnowledgeBaseDb(this IServiceProvider serviceProvider)
    {
        using var context = new KnowledgeBaseDbContext(
            serviceProvider.CreateContextOptions<KnowledgeBaseDbContext>(KnowledgeBaseDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateInquiriesDb(this IServiceProvider serviceProvider)
    {
        using var context = new InquiriesDbContext(
            serviceProvider.CreateContextOptions<InquiriesDbContext>(InquiriesDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateMerchDb(this IServiceProvider serviceProvider)
    {
        using var context = new MerchDbContext(
            serviceProvider.CreateContextOptions<MerchDbContext>(MerchDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateCvDb(this IServiceProvider serviceProvider)
    {
        using var context = new CvDbContext(
            serviceProvider.CreateContextOptions<CvDbContext>(CvDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateAttendancesDb(this IServiceProvider serviceProvider)
    {
        using var context = new AttendanceDbContext(
            serviceProvider.CreateContextOptions<AttendanceDbContext>(AttendanceDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void MigrateFinanceDb(this IServiceProvider serviceProvider)
    {
        using var context = new FinanceDbContext(
            serviceProvider.CreateContextOptions<FinanceDbContext>(FinanceDbContext.Schema)
        );

        context.MigrateDbContext(serviceProvider);
    }

    public static void ExecuteIdentitySeed(this IServiceProvider serviceProvider)
    {
        serviceProvider.ExecuteSeeds<IdentityModuleDbContext>();
    }

    public static void ExecuteTimeTrackSeed(this IServiceProvider serviceProvider)
    {
        serviceProvider.ExecuteSeeds<TimeTrackDbContext>();
    }

    public static void ExecuteBookingSeed(this IServiceProvider serviceProvider)
    {
        serviceProvider.ExecuteSeeds<BookingDbContext>();
    }

    public static void ExecuteEquipmentSeed(this IServiceProvider serviceProvider)
    {
        serviceProvider.ExecuteSeeds<EquipmentDbContext>();
    }

    public static void ExecuteTeamBoardSeed(this IServiceProvider serviceProvider)
    {
        serviceProvider.ExecuteSeeds<TeamBoardDbContext>();
    }

    public static void ExecuteInquiriesSeed(this IServiceProvider serviceProvider)
    {
        serviceProvider.ExecuteSeeds<InquiriesDbContext>();
    }

    public static void ExecuteKnowledgeBaseSeed(this IServiceProvider serviceProvider)
    {
        serviceProvider.ExecuteSeeds<KnowledgeBaseDbContext>();
    }

    public static void ExecuteMerchSeed(this IServiceProvider serviceProvider)
    {
        serviceProvider.ExecuteSeeds<MerchDbContext>();
    }

    public static void ExecuteCvSeed(this IServiceProvider serviceProvider)
    {
        serviceProvider.ExecuteSeeds<CvDbContext>();
    }

    public static void ExecuteAttendancesSeed(this IServiceProvider serviceProvider)
    {
        serviceProvider.ExecuteSeeds<AttendanceDbContext>();
    }

    public static void ExecuteFinanceSeed(this IServiceProvider serviceProvider)
    {
        serviceProvider.ExecuteSeeds<FinanceDbContext>();
    }

    private static DbContextOptions<TContext> CreateContextOptions<TContext>(this IServiceProvider scope, string schema) where TContext : DbContext
    {
        var factory = scope.GetRequiredService<IConnectionFactory>();
        var options = new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(factory.GetConnection(), options =>
            {
                options.CommandTimeout(120);
                options.MigrationsHistoryTable(
                    HistoryRepository.DefaultTableName,
                    schema);
            });

        return options.Options;
    }

    private static void MigrateDbContext<TContext>(this TContext context, IServiceProvider scope) where TContext : DbContext
    {
        var logger = scope.GetRequiredService<ILogger<TContext>>();
        var contextName = typeof(TContext).Name;

        try
        {
            context.Database.Migrate();
            logger.LogInformation("Database associated with context {Name} has been migrated", contextName);
        }
        finally
        {
            context.Dispose();
        }
    }


    public static void ApplyMigrationTasks(this IServiceProvider scope, string[] names)
    {
        using var migrationTaskScope = scope.CreateScope();
        var migrationTasks = migrationTaskScope.ServiceProvider.GetServices<IMigrationTask>();
        var migrationDbContext = migrationTaskScope.ServiceProvider.GetRequiredService<MigratorDbContext>();

        foreach (var migrationTask in migrationTasks.Where(x => names.Contains(x.TaskName)))
        {
            var hasTask = migrationDbContext.MigrationTasks.Any(x => x.TaskName == migrationTask.TaskName);
            if (!hasTask)
            {
                migrationDbContext.MigrationTasks.Add(new MigrationTask { TaskName = migrationTask.TaskName });
                migrationTask.Execute();
            }
        }

        migrationDbContext.SaveChanges();
    }
}
