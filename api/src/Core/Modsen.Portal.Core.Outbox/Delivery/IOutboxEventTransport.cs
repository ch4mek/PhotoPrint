using Modsen.Portal.Core.Communication;

namespace Modsen.Portal.Core.Outbox.Delivery;

public interface IOutboxEventTransport
{
    Task Deliver<T>(T integrationEvent, CancellationToken cancellationToken) where T : IIntegrationEvent;
}
