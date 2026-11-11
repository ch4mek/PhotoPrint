using MassTransit;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Outbox.Delivery;

namespace Modsen.Portal.Core.Outbox.Transport;

internal sealed class MassTransitOutboxEventTransport : IOutboxEventTransport
{
    private readonly IBus _bus;

    public MassTransitOutboxEventTransport(IBus bus)
    {
        _bus = bus;
    }

    public Task Deliver<T>(T integrationEvent, CancellationToken cancellationToken)
        where T : IIntegrationEvent
    {
        return _bus.Publish(integrationEvent, cancellationToken);
    }
}
