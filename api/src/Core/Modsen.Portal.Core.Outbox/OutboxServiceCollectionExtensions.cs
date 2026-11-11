using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Outbox.Delivery;
using Modsen.Portal.Core.Outbox.Publisher;
using Modsen.Portal.Core.Outbox.Storage;
using Modsen.Portal.Core.Outbox.Transport;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Core.Outbox;

public static class OutboxServiceCollectionExtensions
{
    public static void AddOutboxRepository<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.TryAddScoped<IOutboxEventRepository<TContext>>(sp =>
        {
            var context = sp.GetRequiredService<TContext>();
            var connectionFactory = sp.GetRequiredService<IConnectionFactory>();

            context.Database.UseTransaction(connectionFactory.GetTransaction());

            return new OutboxEventRepository<TContext>(context,
                sp.GetRequiredService<IClockService>(),
                sp.GetRequiredService<IAuthenticatedUserContext>(),
                sp.GetRequiredService<ICorrelationContext>());
        });

        services.AddScoped<IOutboxEventRepository>(serviceProvider => serviceProvider
            .GetRequiredService<IOutboxEventRepository<TContext>>());

        services.TryAddScoped<IOutboxEventStorageResolver, OutboxEventStorageResolver>();
        services.TryAddScoped<IOutboxEventSynchronizer, OutboxEventSynchronizer>();
        services.TryAddScoped<IOutboxEventPublisher<TContext>, OutboxEventPublisher<TContext>>();
        services.TryAddScoped<IOutboxEventTransport, MassTransitOutboxEventTransport>();

        services.AddHostedService<OutboxEventPublisherBackgroundService<TContext>>();
    }

    public static IServiceCollection HandleEventViaOutbox<TEvent, TContext>(this IServiceCollection services)
        where TContext : DbContext
        where TEvent : IIntegrationEvent
    {
        OutboxEventStorageConfiguration.RegisterStorageForEvent<TEvent, IOutboxEventRepository<TContext>, TContext>();
        return services;
    }
}
