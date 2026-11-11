using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache.CacheRecords;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache;

internal interface IEnglishLevelCache : IRecyclableCache
{
    ValueTask<List<EnglishLevelCacheRecord>> GetAllEnglishLevels(CancellationToken cancellationToken);
}

internal class EnglishLevelCache : IEnglishLevelCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ITeamBoardDbContext _dbContext;
    private const string s_cacheKey = "__EnglishLevel_Entities";
    private const int s_expirationInMinutes = 60;

    public EnglishLevelCache(IMemoryCache memoryCache,
        ITeamBoardDbContext dbContext)
    {
        _memoryCache = memoryCache;
        _dbContext = dbContext;
    }

    public async ValueTask<List<EnglishLevelCacheRecord>> GetAllEnglishLevels(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue<List<EnglishLevelCacheRecord>>(s_cacheKey, out var englishLevelsFromCache) &&
            englishLevelsFromCache is { Count: > 0 })
        {
            return englishLevelsFromCache;
        }

        var englishLevelsFromDatabase = await LoadFromDatabase(cancellationToken);
        var options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(s_expirationInMinutes)
        };

        _memoryCache.Set(s_cacheKey, englishLevelsFromDatabase, options);

        return englishLevelsFromDatabase;
    }

    private Task<List<EnglishLevelCacheRecord>> LoadFromDatabase(CancellationToken cancellationToken)
    {
        return _dbContext.EnglishLevels
            .AsNoTracking()
            .OrderByDescending(x => x.OrderWeight)
            .Select(x => new EnglishLevelCacheRecord
            {
                Id = x.Id,
                Name = x.InternalName,
                OrderWeight = x.OrderWeight
            })
            .ToListAsync(cancellationToken);
    }

    public void Clear()
    {
        _memoryCache.Remove(s_cacheKey);
    }
}
