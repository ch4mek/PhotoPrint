using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Seeds;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Attendance.DataAccess.Npgsql;

public class AttendanceDataAccessModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<AttendanceDbContext>((serviceProvider, optionsBuilder) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();

            optionsBuilder.UsePortalMySql(factory.GetConnection(),
                    pgOptions =>
                    {
                        pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName,
                            AttendanceDbContext.Schema);
                    })
                .AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>())
                .ConfigureWarnings(warnings => warnings.Throw(
                    RelationalEventId.MultipleCollectionIncludeWarning,
                    CoreEventId.FirstWithoutOrderByAndFilterWarning));
        });

        services.AddScoped<IAttendanceDbContext>(serviceProvider =>
        {
            var context = serviceProvider.GetRequiredService<AttendanceDbContext>();
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            context.Database.UseTransaction(connectionFactory.GetTransaction());

            return context;
        });

        services.AddTransient<ISeedEntitiesProvider<AttendanceDbContext>, AttendancesSeed>();
    }
}
