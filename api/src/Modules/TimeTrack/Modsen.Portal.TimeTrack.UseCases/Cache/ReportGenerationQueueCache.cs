using Microsoft.Extensions.Caching.Memory;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TimeTrack.UseCases.Cache;

internal interface IReportGenerationQueueCache : IRecyclableCache
{
    bool Exists(string jsonHash, string userId);
    void Set(string jsonHash, string userId);
    void Remove(string jsonHash, string userId);
}

internal class ReportGenerationQueueCache : IReportGenerationQueueCache
{
    private readonly IMemoryCache _memoryCache;
    private const string s_cacheKey = "__ReportGenerationQueues";
    private const int s_expirationInMinutes = 30;

    public ReportGenerationQueueCache(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public bool Exists(string jsonHash, string userId)
    {
        return _memoryCache.TryGetValue($"{s_cacheKey}_{userId}_{jsonHash}", out _);
    }

    public void Set(string jsonHash, string userId)
    {
        _memoryCache.Set($"{s_cacheKey}_{userId}_{jsonHash}", string.Empty,
                absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(s_expirationInMinutes));
    }

    public void Remove(string jsonHash, string userId)
    {
        _memoryCache.Remove($"{s_cacheKey}_{userId}_{jsonHash}");
    }

    public void Clear()
    {
        _memoryCache.Remove(s_cacheKey);
    }
}
