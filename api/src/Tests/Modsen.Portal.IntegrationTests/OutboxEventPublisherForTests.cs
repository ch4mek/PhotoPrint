using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Outbox.Delivery;

namespace Modsen.Portal.IntegrationTests;

internal class OutboxEventPublisherForTests : ICommunicationBus
{
    private readonly IOutboxEventTransport _outboxEventTransport;

    public OutboxEventPublisherForTests(IOutboxEventTransport outboxEventTransport)
    {
        _outboxEventTransport = outboxEventTransport;
    }
    
    public Task PublishAsync<T>(T @event, CancellationToken cancellationToken) where T : IIntegrationEvent
    {       
        _outboxEventTransport.Deliver(@event, cancellationToken);

        return Task.Delay(300, cancellationToken);
    }

    public async Task PublishAsync<T>(T[] events, CancellationToken cancellationToken) where T : IIntegrationEvent
    {        
        foreach (var @event in events)
        {
            await _outboxEventTransport.Deliver(@event, cancellationToken);           
        }
    }
}
