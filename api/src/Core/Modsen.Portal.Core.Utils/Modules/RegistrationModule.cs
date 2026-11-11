using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Core.Utils.Modules;

public abstract class RegistrationModule
{
    private List<RegistrationModule> Modules { get; } = [];
    public IConfiguration Configuration { get; set; } = null!;

    public virtual void Load(IServiceCollection services)
    {
        foreach (var module in Modules)
        {
            module.Load(services);
        }
    }
    
    public virtual IEnumerable<(string Name, AuthorizationPolicy Policy)> GetPolicies()
    {
        return Modules.SelectMany(x => x.GetPolicies());
    }

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
    
    public virtual IEnumerable<IExchangeDefinitionBase> GetExchangeDefinitions()
    {
        return Modules.SelectMany(x => x.GetExchangeDefinitions());
    }

    public virtual IEnumerable<IConsumerConfigurationRegistration> GetConsumerRegistrationConfigurations()
    {
        return Modules.SelectMany(x => x.GetConsumerRegistrationConfigurations());
    }
}
