using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Outbox;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Storage.DataAccess.Npgsql;

public sealed class StorageDataAccessModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<StorageDbContext>((serviceProvider, optionsBuilder) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();

            optionsBuilder.UsePortalMySql(factory.GetConnection(),
                    pgOptions =>
                    {
                        pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, StorageDbContext.Schema);
                    })
                .AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>())
                .ConfigureWarnings(warnings => warnings.Throw(
                    RelationalEventId.MultipleCollectionIncludeWarning,
                    CoreEventId.FirstWithoutOrderByAndFilterWarning));
        });

        services.AddScoped<IStorageDbContext>(serviceProvider =>
        {
            var context = serviceProvider.GetRequiredService<StorageDbContext>();
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            context.Database.UseTransaction(connectionFactory.GetTransaction());

            return context;
        });

        services.AddOutboxRepository<StorageDbContext>();
    }
}
