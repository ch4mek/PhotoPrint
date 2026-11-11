using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache.CacheRecords;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache;

internal interface ISeniorityLevelCache : IRecyclableCache
{
    ValueTask<List<SeniorityLevelCacheRecord>> GetAllSeniorityLevels(CancellationToken cancellationToken);
}

internal class SeniorityLevelCache : ISeniorityLevelCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private const string s_cacheKey = "__SeniorityLevel_Entities";
    private const int s_expirationInMinutes = 60;

    public SeniorityLevelCache(IMemoryCache memoryCache,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _memoryCache = memoryCache;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async ValueTask<List<SeniorityLevelCacheRecord>> GetAllSeniorityLevels(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue<List<SeniorityLevelCacheRecord>>(s_cacheKey, out var seniorityLevelsFromCache) &&
            seniorityLevelsFromCache is { Count: > 0 })
        {
            return seniorityLevelsFromCache;
        }

        var seniorityLevelsFromDatabase = await LoadFromDatabase(cancellationToken);

        var options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(s_expirationInMinutes)
        };

        _memoryCache.Set(s_cacheKey, seniorityLevelsFromDatabase, options);

        return seniorityLevelsFromDatabase;
    }

    private Task<List<SeniorityLevelCacheRecord>> LoadFromDatabase(CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.SeniorityLevels
            .AsNoTracking()
            .OrderByDescending(x => x.OrderWeight)
            .Select(x => new SeniorityLevelCacheRecord
            {
                Id = x.Id,
                Name = x.InternalName,
                OrderWeight = x.OrderWeight,
                SeniorityLevelScope = (int?)x.SeniorityLevelScope,
                Coefficient = x.Coefficient,
                SpecializationRangeTo = x.SpecializationRangeTo
            })
            .ToListAsync(cancellationToken);
    }

    public void Clear()
    {
        _memoryCache.Remove(s_cacheKey);
    }
}
