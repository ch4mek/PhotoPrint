using MassTransit;

namespace Modsen.Portal.Core.Communication.ExchangeDefinition;

public interface IConsumerConfigurationRegistration
{
    Type GetConsumerType();
    void Register(IRegistrationConfigurator configurator);
}

public abstract class ConsumerConfigurationRegistration<TConsumer, TDefinition> :
    IConsumerConfigurationRegistration
    where TConsumer : class, IConsumer
    where TDefinition : class, IConsumerDefinition<TConsumer>
{
    public Type GetConsumerType()
    {
        return typeof(TConsumer);
    }

    public void Register(IRegistrationConfigurator configurator)
    {
        configurator.AddConsumer<TConsumer, TDefinition>();
    }
}
