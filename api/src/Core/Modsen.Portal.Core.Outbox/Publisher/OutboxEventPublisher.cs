using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Outbox.Delivery;
using Modsen.Portal.Core.Outbox.Storage;

namespace Modsen.Portal.Core.Outbox.Publisher;

internal sealed class OutboxEventPublisher<TContext> : BaseOutboxEventPublisher, IOutboxEventPublisher<TContext> where TContext : DbContext
{
    private readonly IOutboxEventRepository<TContext> _outboxEventRepository;
    private readonly IOutboxEventTransport _outboxEventTransport;
    private readonly IOutboxEventSynchronizer _outboxEventSynchronizer;

    public OutboxEventPublisher(
        TContext dbContext,
        IOutboxEventRepository<TContext> outboxEventRepository,
        IOutboxEventTransport outboxEventTransport,
        IOutboxEventSynchronizer outboxEventSynchronizer)
    {
        StoringContext = dbContext;
        _outboxEventRepository = outboxEventRepository;
        _outboxEventTransport = outboxEventTransport;
        _outboxEventSynchronizer = outboxEventSynchronizer;
    }

    public TContext StoringContext { get; }

    public async Task<bool> PublishPendingEvents(CancellationToken cancellationToken)
    {
        var @event = await _outboxEventRepository.GetNextUnsentEvent(cancellationToken);
        if (@event != null)
        {
            await Deliver(@event, cancellationToken);
            return await _outboxEventRepository.SaveChanges(cancellationToken) > 0;
        }

        return false;
    }

    private async Task Deliver(OutboxEvent outboxEvent, CancellationToken cancellationToken)
    {
        var canOutboxEventBeProcessed = await _outboxEventSynchronizer.CanOutboxEventBeProcessed(outboxEvent.Id, cancellationToken);
        if (canOutboxEventBeProcessed)
        {
            var isOutboxEventMarkedAsProcessed = await _outboxEventSynchronizer.MarkOutboxEventAsProcessed(outboxEvent.Id, cancellationToken);
            if (isOutboxEventMarkedAsProcessed)
            {
                var @event = DeserializeOutboxEvent(outboxEvent);
                if (@event != null)
                {
                    await _outboxEventTransport.Deliver(@event, cancellationToken);
                    outboxEvent.WasSent = true;
                }
            }
        }
    }
}
