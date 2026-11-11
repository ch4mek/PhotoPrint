using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Equipment.Contract;
using Modsen.Portal.Equipment.Requests.Consumers;
using Modsen.Portal.Equipment.Requests.Core.Builders;
using Modsen.Portal.Equipment.Requests.Core.BusinessRules;
using Modsen.Portal.Equipment.Requests.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.Equipment;
using Modsen.Portal.Equipment.Requests.EquipmentHistory;
using Modsen.Portal.Equipment.Requests.EquipmentPortalSettings;
using Modsen.Portal.Equipment.Requests.Users;
using Modsen.Portal.Equipment.Requests.Users.BusinessRules;

namespace Modsen.Portal.Equipment.Requests;

public sealed class EquipmentRequestsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        AddModule<EquipmentModule>();
        AddModule<EquipmentHistoryModule>();
        AddModule<UsersModule>();

        base.Load(services);

        services.AddScoped<ICanEquipmentUserBeDeactivatedBusinessRule, CanEquipmentUserBeDeactivatedBusinessRule>();
        services.AddScoped<ICanAccessUnassignedEquipmentBusinessRule, CanAccessUnassignedEquipmentBusinessRule>();
        services.AddScoped<IIsAllowedToGetUserEquipmentBusinessRule, IsAllowedToGetUserEquipmentBusinessRule>();
        services.AddScoped<IEquipmentContract, EquipmentContract>();
        services.AddScoped<IGetEquipmentByIdDbQuery, GetEquipmentByIdDbQuery>();
        services.AddScoped<IEquipmentQueryBuilder, EquipmentQueryBuilder>();
        services.AddScoped<IUserCreatedOrUpdatedDbQuery, UserCreatedOrUpdatedDbQuery>();
        services.AddScoped<IEquipmentPortalSettingsAccessor, EquipmentPortalSettingsAccessor>();
    }


    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new UserActivatedEventConsumer.RegistrationDefinition();
        yield return new UserCreatedEventConsumer.RegistrationDefinition();
        yield return new UserDeactivatedEventConsumer.RegistrationDefinition();
        yield return new UserSyncEventConsumer.RegistrationDefinition();
        yield return new UserUpdatedEventConsumer.RegistrationDefinition();
        yield return new DepartmentSyncEventConsumer.RegistrationDefinition();
    }
}
