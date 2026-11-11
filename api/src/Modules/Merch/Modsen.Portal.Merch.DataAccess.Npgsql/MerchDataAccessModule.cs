using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Merch.DataAccess.Npgsql.Seeds;

namespace Modsen.Portal.Merch.DataAccess.Npgsql;

public sealed class MerchDataAccessModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<MerchDbContext>((serviceProvider, options) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();

            options.UseNpgsql(factory.GetConnection(), pgOptions =>
            {
                pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, MerchDbContext.Schema);
                pgOptions.MigrationsAssembly(typeof(MerchDataAccessModule).Assembly.FullName);
            });
            options.AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>());
            options.ConfigureWarnings(warnings => warnings.Throw(
                RelationalEventId.MultipleCollectionIncludeWarning,
                CoreEventId.FirstWithoutOrderByAndFilterWarning));
        });

        services.AddScoped<IMerchDbContext>(serviceProvider =>
        {
            var merchDbContext = serviceProvider.GetRequiredService<MerchDbContext>();
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            merchDbContext.Database.UseTransaction(connectionFactory.GetTransaction());

            return merchDbContext;
        });

        services.AddTransient<ISeedEntitiesProvider<MerchDbContext>, SizesSeed>();
        services.AddTransient<ISeedEntitiesProvider<MerchDbContext>, WarehousesSeed>();
        services.AddTransient<ISeedEntitiesProvider<MerchDbContext>, ItemSeed>();
        services.AddTransient<ISeedEntitiesProvider<MerchDbContext>, KitsSeed>();
        services.AddTransient<ISeedEntitiesProvider<MerchDbContext>, TransactionsSeed>();
    }
}
