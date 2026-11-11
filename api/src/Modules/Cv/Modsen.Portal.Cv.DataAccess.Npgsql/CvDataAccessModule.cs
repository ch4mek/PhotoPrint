using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Cv.DataAccess.Npgsql.Seeds;

namespace Modsen.Portal.Cv.DataAccess.Npgsql;

public sealed class CvDataAccessModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<CvDbContext>((serviceProvider, optionsBuilder) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();

            optionsBuilder.UseNpgsql(factory.GetConnection(), pgOptions =>
            {
                pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, CvDbContext.Schema);
            })
            .AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>())
            .ConfigureWarnings(warnings => warnings.Throw(
                RelationalEventId.MultipleCollectionIncludeWarning,
                CoreEventId.FirstWithoutOrderByAndFilterWarning));
        });

        services.AddScoped<ICvDbContext>(serviceProvider =>
        {
            var context = serviceProvider.GetRequiredService<CvDbContext>();
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            context.Database.UseTransaction(connectionFactory.GetTransaction());

            return context;
        });

        services.AddTransient<ISeedEntitiesProvider<CvDbContext>, CvSeed>();
    }
}
