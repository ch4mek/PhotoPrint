using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache.CacheRecords;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache;

internal interface IHardSkillGroupCache : IRecyclableCache
{
    ValueTask<List<HardSkillGroupCacheRecord>> GetAllHardSkillGroups(CancellationToken cancellationToken);
}

internal class HardSkillGroupsCache : IHardSkillGroupCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private const string s_cacheKey = "__HardSkillGroup_Entities";
    private const int s_expirationInMinutes = 60;

    public HardSkillGroupsCache(IMemoryCache memoryCache,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _memoryCache = memoryCache;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async ValueTask<List<HardSkillGroupCacheRecord>> GetAllHardSkillGroups(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue<List<HardSkillGroupCacheRecord>>(s_cacheKey, out var hardSkillGroupsFromCache) &&
            hardSkillGroupsFromCache is { Count: > 0 })
        {
            return hardSkillGroupsFromCache;
        }

        var hardSkillGroupsFromDatabase = await LoadFromDatabase(cancellationToken);

        var options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(s_expirationInMinutes)
        };

        _memoryCache.Set(s_cacheKey, hardSkillGroupsFromDatabase, options);

        return hardSkillGroupsFromDatabase;
    }

    private Task<List<HardSkillGroupCacheRecord>> LoadFromDatabase(CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.HardSkillGroups
            .AsNoTracking()
            .Select(x => new HardSkillGroupCacheRecord
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }

    public void Clear()
    {
        _memoryCache.Remove(s_cacheKey);
    }
}
