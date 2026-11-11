using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Merch.Requests.Storage.Core.Loaders;

namespace Modsen.Portal.Merch.Requests.Storage;

public sealed class ItemsStorageModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IStorageItemsLoader, StorageItemsLoader>();
    }
}
