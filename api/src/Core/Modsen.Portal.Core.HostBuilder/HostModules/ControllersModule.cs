using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Api.Converters;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Core.HostBuilder.HostModules;

internal sealed class ControllersModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
            .ConfigureApplicationPartManager(manager =>
            {
                manager.FeatureProviders.Add(new InternalControllerFeatureProvider());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
                options.JsonSerializerOptions.Converters.Add(new TrimmingConverter());
            });

        services.AddEndpointsApiExplorer();
    }

    private sealed class InternalControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            return typeof(ControllerBase).IsAssignableFrom(typeInfo);
        }
    }
}
