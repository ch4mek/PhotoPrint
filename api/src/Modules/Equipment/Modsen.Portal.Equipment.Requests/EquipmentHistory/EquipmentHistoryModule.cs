using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Equipment.Requests.EquipmentHistory.Core.DbQueries;

namespace Modsen.Portal.Equipment.Requests.EquipmentHistory;

public sealed class EquipmentHistoryModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IIsEquipmentExistByIdDbQuery, IsEquipmentExistByIdDbQuery>();
    }
}
