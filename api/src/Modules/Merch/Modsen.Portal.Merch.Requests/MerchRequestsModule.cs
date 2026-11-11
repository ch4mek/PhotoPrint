using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Merch.Contract;
using Modsen.Portal.Merch.Requests.Consumers;
using Modsen.Portal.Merch.Requests.Core.DbQueries;
using Modsen.Portal.Merch.Requests.Core.Loaders;
using Modsen.Portal.Merch.Requests.Items;
using Modsen.Portal.Merch.Requests.Kits;
using Modsen.Portal.Merch.Requests.Storage;
using Modsen.Portal.Merch.Requests.Transactions;

namespace Modsen.Portal.Merch.Requests;

public sealed class MerchRequestsModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        AddModule<ItemsModule>();
        AddModule<KitsModule>();
        AddModule<TransactionsModule>();
        AddModule<ItemsStorageModule>();

        base.Load(services);

        services.AddScoped<IMerchContract, MerchContract>();
        services.AddScoped<IItemsCountLoader, ItemsCountLoader>();
        services.AddScoped<IUpdateIsActiveForUserDbQuery, UpdateIsActiveForUserDbQuery>();
    }


    public override IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        yield return new UserCreatedEventConsumer.RegistrationDefinition();
        yield return new UserSyncEventConsumer.RegistrationDefinition();
        yield return new UserUpdatedEventConsumer.RegistrationDefinition();
        yield return new UserActivatedEventConsumer.RegistrationDefinition();
        yield return new UserDeactivatedEventConsumer.RegistrationDefinition();
    }
}
