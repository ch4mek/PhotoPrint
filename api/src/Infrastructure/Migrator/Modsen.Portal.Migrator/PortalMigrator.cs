using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.Migrator.DataAccess;
using Modsen.Portal.Notification.DataAccess.Npgsql;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

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
            .UsePortalMySql(factory.GetConnection(), options =>
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
            // EnsureCreated у EF — all-or-nothing на всю базу: после первой же таблицы
            // остальные контексты бы молча пропускались. Поэтому выполняем DDL-скрипт
            // контекста по инструкции, пропуская уже созданные таблицы (идемпотентно).
            var script = context.Database.GenerateCreateScript();
            var created = 0;
            foreach (var raw in script.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                var statement = raw.Trim();
                if (statement.Length == 0) continue;
                try
                {
                    context.Database.ExecuteSqlRaw(statement);
                    created++;
                }
                catch (MySqlConnector.MySqlException ex) when (ex.ErrorCode == MySqlConnector.MySqlErrorCode.TableExists)
                {
                    // таблица уже создана ранее — норма при повторном старте
                }
                catch (MySqlConnector.MySqlException ex) when (ex.Message.Contains("Duplicate key name"))
                {
                    // индекс уже существует — норма при повторном старте
                }
                catch (MySqlConnector.MySqlException ex) when (ex.Message.Contains("Invalid default value"))
                {
                    var patched = statement
                        .Replace("DEFAULT now()", "DEFAULT CURRENT_TIMESTAMP(6)", StringComparison.OrdinalIgnoreCase)
                        .Replace("DEFAULT now(6)", "DEFAULT CURRENT_TIMESTAMP(6)", StringComparison.OrdinalIgnoreCase);
                    patched = System.Text.RegularExpressions.Regex.Replace(
                        patched,
                        @"DEFAULT\s+'0{3}\d-\d\d-\d\d[^']*'",
                        string.Empty,
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (patched == statement)
                    {
                        logger.LogError("DDL не удалось починить ({Message}): {Statement}", ex.Message, statement[..Math.Min(400, statement.Length)]);
                        throw;
                    }
                    try { context.Database.ExecuteSqlRaw(patched); created++; }
                    catch (MySqlConnector.MySqlException ie) when (
                        ie.ErrorCode == MySqlConnector.MySqlErrorCode.TableExists ||
                        ie.Message.Contains("Duplicate key name")) { }
                }
                catch (MySqlConnector.MySqlException ex) when (ex.Message.Contains("Duplicate foreign key constraint name"))
                {
                    // В PostgreSQL имена FK были уникальны в пределах схемы; в MySQL — в пределах базы.
                    // Префиксуем имена ограничений схемой контекста.
                    var schema = context.Database.IsRelational() && context.Model.GetDefaultSchema() is { } s ? s : contextName.Replace("DbContext", "").Replace("Module", "").ToLowerInvariant();
                    var patched = System.Text.RegularExpressions.Regex.Replace(
                        statement,
                        @"(CONSTRAINT\s+`)(FK_[^`]+)`",
                        $"$1{schema}_$2`");
                    if (patched == statement)
                    {
                        logger.LogError("FK-конфликт не удалось починить: {Statement}", statement[..Math.Min(300, statement.Length)]);
                        throw;
                    }
                    context.Database.ExecuteSqlRaw(patched);
                    created++;
                }
            }

            logger.LogInformation("Database associated with context {Name} has been created ({Created} statements)", contextName, created);
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
