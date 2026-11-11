using MassTransit;

namespace Modsen.Portal.Core.Communication.ExchangeDefinition;

public abstract class ConsumerDefinitionBase<TEvent> : ConsumerDefinition<TEvent>
    where TEvent : class, IConsumer
{
    private readonly string _exchange;

    protected ConsumerDefinitionBase(string queue, string exchange)
    {
        ArgumentException.ThrowIfNullOrEmpty(queue);
        ArgumentException.ThrowIfNullOrEmpty(exchange);

        _exchange = exchange;
        EndpointName = queue;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<TEvent> consumerConfigurator, IRegistrationContext context)
    {
        if (endpointConfigurator is IRabbitMqReceiveEndpointConfigurator rabbitMqConfigurator)
        {
            rabbitMqConfigurator.Bind(_exchange);

            rabbitMqConfigurator.Durable = true;
            rabbitMqConfigurator.AutoDelete = false;
            rabbitMqConfigurator.PurgeOnStartup = false;
        }
    }
}
