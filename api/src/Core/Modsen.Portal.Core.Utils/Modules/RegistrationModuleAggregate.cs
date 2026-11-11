using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Modsen.Portal.Core.Utils.Modules;

public sealed class RegistrationModuleAggregate(IConfiguration configuration)
{
    public Collection<RegistrationModule> Modules { get; } = [];

    private IConfiguration Configuration { get; } = configuration;

    public void AddModule<T>() where T : RegistrationModule
    {
        AddModule(Activator.CreateInstance<T>());
    }

    public void AddModule<T>(T module) where T : RegistrationModule
    {
        ArgumentNullException.ThrowIfNull(Configuration);
        ArgumentNullException.ThrowIfNull(module);

        module.Configuration = Configuration;

        Modules.Add(module);
    }

    public void LoadRoot(IServiceCollection services)
    {
        foreach (var module in Modules)
        {
            module.Load(services);
        }
    }
}
