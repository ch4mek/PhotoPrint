using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.Seeds;

internal static class SeedDataExtensions
{
    internal static IServiceCollection AddSeeds(this IServiceCollection services)
    {
        services.AddTransient<ISeedEntitiesProvider<EquipmentDbContext>, EquipmentTypeSeed>();
        services.AddTransient<ISeedEntitiesProvider<EquipmentDbContext>, EquipmentSeed>();
        services.AddTransient<ISeedEntitiesProvider<EquipmentDbContext>, EquipmentAssignSeed>();

        return services;
    }
}
