using Microsoft.EntityFrameworkCore;

namespace Modsen.Portal.Core.Outbox.Delivery;

public interface IOutboxEventPublisher<out TContext> where TContext : DbContext
{
    TContext StoringContext { get; }
    Task<bool> PublishPendingEvents(CancellationToken cancellationToken);
}
