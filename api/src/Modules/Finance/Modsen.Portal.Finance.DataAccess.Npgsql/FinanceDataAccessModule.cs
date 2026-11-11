using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Finance.DataAccess.Npgsql.Seeds;

namespace Modsen.Portal.Finance.DataAccess.Npgsql;

public sealed class FinanceDataAccessModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<FinanceDbContext>((serviceProvider, options) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();

            options.UsePortalMySql(factory.GetConnection(), pgOptions =>
            {
                pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, FinanceDbContext.Schema);
                pgOptions.MigrationsAssembly(typeof(FinanceDataAccessModule).Assembly.FullName);
            });
            options.AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>());
            options.ConfigureWarnings(warnings => warnings.Throw(
                RelationalEventId.MultipleCollectionIncludeWarning,
                CoreEventId.FirstWithoutOrderByAndFilterWarning));
        });

        services.AddScoped<IFinanceDbContext>(serviceProvider =>
        {
            var plDbContext = serviceProvider.GetRequiredService<FinanceDbContext>();
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            plDbContext.Database.UseTransaction(connectionFactory.GetTransaction());

            return plDbContext;
        });

        services.AddTransient<ISeedEntitiesProvider<FinanceDbContext>, CurrenciesSeed>();
    }
}
