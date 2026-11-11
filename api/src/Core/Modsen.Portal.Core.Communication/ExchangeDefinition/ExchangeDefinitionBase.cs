using MassTransit;

namespace Modsen.Portal.Core.Communication.ExchangeDefinition;

public interface IExchangeDefinitionBase
{
    void Configure(IRabbitMqBusFactoryConfigurator publishConfig);
}

public abstract class ExchangeDefinitionBase<TEvent> : IExchangeDefinitionBase where TEvent : class
{
    public abstract string ExchangeName { get; }

    public void Configure(IRabbitMqBusFactoryConfigurator publishConfig)
    {
        ArgumentNullException.ThrowIfNull(publishConfig);
        publishConfig.Message<TEvent>(x => x.SetEntityName(ExchangeName));
        publishConfig.Publish<TEvent>(e =>
        {
            e.ExchangeType = "fanout";
            e.Durable = true;
            e.AutoDelete = false;
        });
    }
}
