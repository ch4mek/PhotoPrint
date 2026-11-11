using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;
using StackExchange.Redis;

namespace Modsen.Portal.Core.HostBuilder.HostModules;

internal sealed class SignalRModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        var signalRBuilder = services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        });

        if (Configuration.GetValue<string>("CacheStore") == "Redis")
        {
            signalRBuilder.AddStackExchangeRedis(Configuration.GetConnectionString("Redis")!, options =>
            {
                options.Configuration.ChannelPrefix = new RedisChannel("MMS", RedisChannel.PatternMode.Auto);
            });
        }
    }
}
