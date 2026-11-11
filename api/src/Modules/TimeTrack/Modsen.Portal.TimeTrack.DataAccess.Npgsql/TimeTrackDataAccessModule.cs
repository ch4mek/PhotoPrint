using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Outbox;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Providers;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Seeds;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql;

public sealed class TimeTrackDataAccessModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<TimeTrackDbContext>((serviceProvider, optionsBuilder) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();

            optionsBuilder.UsePortalMySql(factory.GetConnection(), pgOptions =>
                {
                    pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, TimeTrackDbContext.Schema);
                })
                .AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>())
                .ConfigureWarnings(warnings => warnings.Throw(
                    RelationalEventId.MultipleCollectionIncludeWarning,
                    CoreEventId.FirstWithoutOrderByAndFilterWarning));
        });

        services.AddScoped<ITimeTrackDbContext>(serviceProvider =>
        {
            var context = serviceProvider.GetRequiredService<TimeTrackDbContext>();
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            context.Database.UseTransaction(connectionFactory.GetTransaction());

            return context;
        });

        services.AddProviders();

        services.AddTransient<ISeedEntitiesProvider<TimeTrackDbContext>, ClientsSeed>();
        services.AddTransient<ISeedEntitiesProvider<TimeTrackDbContext>, ProjectsSeed>();

        services.AddOutboxRepository<TimeTrackDbContext>();
    }
}
