using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Outbox.Delivery;

namespace Modsen.Portal.Core.Outbox.Publisher;

public sealed class OutboxEventPublisherBackgroundService<TContext> : BackgroundService where TContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    public OutboxEventPublisherBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var hasChanges = false;
            using (var scope = _serviceProvider.CreateScope())
            {
                var connectionFactory = scope.ServiceProvider.GetRequiredService<IConnectionFactory>();

                try
                {
                    hasChanges = await scope.ServiceProvider
                        .GetRequiredService<IOutboxEventPublisher<TContext>>()
                        .PublishPendingEvents(stoppingToken);
                    await connectionFactory.CommitCurrentTransaction(stoppingToken);
                }
                catch (Exception)
                {
                    await connectionFactory.GetTransaction().RollbackAsync(stoppingToken);
                }
            }

            var delay = hasChanges ? default(TimeSpan?) : TimeSpan.FromMilliseconds(500);
            if (delay.HasValue)
            {
                await Task.Delay(delay.Value, stoppingToken);
            }
        }
    }
}
