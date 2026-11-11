using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Storage.Controllers.Jobs;

namespace Modsen.Portal.Storage.Controllers;

public sealed class StorageControllersModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddScoped<IRecurringJobScheduler, BackfillApplicationImageVariantsJobScheduler>();
        services.AddScoped<IRecurringJobScheduler, CreateAvatarSmallSquaresJobScheduler>();
    }
}
