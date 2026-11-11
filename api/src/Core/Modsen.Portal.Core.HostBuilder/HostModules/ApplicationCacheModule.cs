using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Core.HostBuilder.HostModules;

internal sealed class ApplicationCacheModule(string serviceName) : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddMemoryCache();

        if (Configuration.GetValue<string>("CacheStore") == "Redis")
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                options.InstanceName = $"{serviceName.Replace(" ", "", StringComparison.InvariantCulture)}RedisCache";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }
    }
}
