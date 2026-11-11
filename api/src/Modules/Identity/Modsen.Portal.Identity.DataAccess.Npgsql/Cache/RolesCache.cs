using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Cache;

internal interface IRolesCache : IRecyclableCache
{
    ValueTask<List<RoleCacheRecord>> GetAllRoles(CancellationToken cancellationToken);
}

internal record RoleCacheRecord
{
    public int Id { get; set; }
    public int OrderWeight { get; set; }
    public string? PublicName { get; set; }
    public string? Name { get; set; }
}

internal class RolesCache : IRolesCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly IIdentityDbContext _identityDbContext;
    private const string s_cacheKey = "__Role_Entities";
    private const int s_expirationInMinutes = 120;

    public RolesCache(IMemoryCache memoryCache,
            IIdentityDbContext identityDbContext)
    {
        _memoryCache = memoryCache;
        _identityDbContext = identityDbContext;
    }

    public async ValueTask<List<RoleCacheRecord>> GetAllRoles(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue<List<RoleCacheRecord>>(s_cacheKey, out var rolesFromCache)
            && rolesFromCache is { Count: > 0 })
        {
            return rolesFromCache;
        }

        var rolesFromDatabase = await LoadFromDatabase(cancellationToken);

        _memoryCache.Set(s_cacheKey, rolesFromDatabase,
            absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(s_expirationInMinutes));

        return rolesFromDatabase;
    }

    private async Task<List<RoleCacheRecord>> LoadFromDatabase(CancellationToken cancellationToken)
    {
        return await _identityDbContext.Roles
            .AsNoTracking()
            .Select(x => new RoleCacheRecord
            {
                Id = x.Id,
                PublicName = x.PublicName,
                OrderWeight = x.OrderWeight,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }

    public void Clear()
    {
        _memoryCache.Remove(s_cacheKey);
    }
}
