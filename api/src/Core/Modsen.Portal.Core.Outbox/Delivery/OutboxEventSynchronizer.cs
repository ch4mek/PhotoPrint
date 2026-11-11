using Microsoft.Extensions.Caching.Distributed;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.Outbox.Delivery;

internal sealed class OutboxEventSynchronizer : IOutboxEventSynchronizer
{
    private readonly IDistributedCache _distributedCache;

    public OutboxEventSynchronizer(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<bool> CanOutboxEventBeProcessed(Guid eventId, CancellationToken cancellationToken)
    {
        var key = GetCacheKey(eventId);
        var value = await _distributedCache.GetStringAsync(key, cancellationToken);

        return value.IsNullOrEmpty();
    }

    public async Task<bool> MarkOutboxEventAsProcessed(Guid eventId, CancellationToken cancellationToken)
    {
        var key = GetCacheKey(eventId);
        var value = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (!value.IsNullOrEmpty())
        {
            return false;
        }

        var cacheDuration = new TimeSpan(0, 0, 30);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheDuration
        };

        await _distributedCache.SetAsync(key, [byte.MinValue], cacheOptions, cancellationToken);

        return true;
    }

    private static string GetCacheKey(Guid eventId)
    {
        return $"outbox-event-{eventId}";
    }
}
