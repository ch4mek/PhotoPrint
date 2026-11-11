using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Equipment.DataAccess.Npgsql.Cache;
using Modsen.Portal.Equipment.DataAccess.Npgsql.Seeds;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql;

public sealed class EquipmentDataAccessModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddDbContext<EquipmentDbContext>((serviceProvider, optionsBuilder) =>
        {
            var factory = serviceProvider.GetRequiredService<IConnectionFactory>();

            optionsBuilder.UseNpgsql(factory.GetConnection(), pgOptions =>
            {
                pgOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, EquipmentDbContext.Schema);
            })
            .AddInterceptors(serviceProvider.GetRequiredService<ISaveChangesInterceptor>())
            .ConfigureWarnings(warnings => warnings.Throw(
                RelationalEventId.MultipleCollectionIncludeWarning,
                CoreEventId.FirstWithoutOrderByAndFilterWarning));
        });

        services.AddScoped<IEquipmentDbContext>(serviceProvider =>
        {
            var context = serviceProvider.GetRequiredService<EquipmentDbContext>();
            var connectionFactory = serviceProvider.GetRequiredService<IConnectionFactory>();

            context.Database.UseTransaction(connectionFactory.GetTransaction());

            return context;
        });

        services.AddScoped<IEquipmentTypesCache, EquipmentTypesCache>();
        services.AddScoped<IRecyclableCache>(sp => sp.GetRequiredService<IEquipmentTypesCache>());

        services.AddSeeds();
    }
}
