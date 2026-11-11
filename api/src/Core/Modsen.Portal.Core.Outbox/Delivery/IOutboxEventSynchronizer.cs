namespace Modsen.Portal.Core.Outbox.Delivery;

public interface IOutboxEventSynchronizer
{
    public Task<bool> CanOutboxEventBeProcessed(Guid eventId, CancellationToken cancellationToken);
    public Task<bool> MarkOutboxEventAsProcessed(Guid eventId, CancellationToken cancellationToken);
}
