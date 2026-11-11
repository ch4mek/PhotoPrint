using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Equipment.Requests.Equipment.Core.BusinessRules;
using Modsen.Portal.Equipment.Requests.Equipment.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.Equipment.Core.Generators;

namespace Modsen.Portal.Equipment.Requests.Equipment;

public sealed class EquipmentModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IIsSerialNumberExistsBusinessRule, IsSerialNumberExistsBusinessRule>();
        services.AddScoped<IIsEquipmentTypeExistsDbQuery, IsEquipmentTypeExistsDbQuery>();
        services.AddScoped<IIsInventoryNumberExistBusinessRule, IsInventoryNumberExistBusinessRule>();
        services.AddScoped<IEquipmentInventoryNumberGenerator, EquipmentInventoryNumberGenerator>();
    }
}
