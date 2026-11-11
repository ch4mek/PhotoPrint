using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Equipment.Requests.Users.BusinessRules;

namespace Modsen.Portal.Equipment.Requests.Users;

internal sealed class UsersModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IIsAllowedToGetUserEquipmentBusinessRule, IsAllowedToGetUserEquipmentBusinessRule>();
    }
}
