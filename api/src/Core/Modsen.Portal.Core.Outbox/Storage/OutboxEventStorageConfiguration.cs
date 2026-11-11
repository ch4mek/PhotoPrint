using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication;

namespace Modsen.Portal.Core.Outbox.Storage;

internal static class OutboxEventStorageConfiguration
{
    private static readonly ConcurrentDictionary<string, Type> _configuration = new();

    public static void RegisterStorageForEvent<TEvent, TRepository, TContext>()
        where TEvent : IIntegrationEvent
        where TContext : DbContext
        where TRepository : IOutboxEventRepository<TContext>
    {
        _configuration.TryAdd(typeof(TEvent).Name, typeof(TRepository));
    }


    public static Type? GetStorageForEvent<TEvent>() where TEvent : IIntegrationEvent
    {
        return _configuration.GetValueOrDefault(typeof(TEvent).Name);
    }
}
