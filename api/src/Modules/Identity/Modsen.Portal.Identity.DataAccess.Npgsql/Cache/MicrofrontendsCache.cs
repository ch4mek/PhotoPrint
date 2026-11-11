using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Cache;

internal interface IMicrofrontendsCache : IRecyclableCache
{
    ValueTask<MicrofrontendsCacheRecord> GetMicrofrontendsConfig(CancellationToken cancellationToken);
}

internal record MicrofrontendsCacheRecord
{
    public int Id { get; init; }
    public string? JsonData { get; init; }
}

internal class MicrofrontendsCache : IMicrofrontendsCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly IIdentityDbContext _identityDbContext;
    private const string s_cacheKey = "__Microfrontends_Entities";
    private const int s_expirationInMinutes = 120;

    public MicrofrontendsCache(
        IMemoryCache memoryCache,
        IIdentityDbContext identityDbContext)
    {
        _memoryCache = memoryCache;
        _identityDbContext = identityDbContext;
    }

    public async ValueTask<MicrofrontendsCacheRecord> GetMicrofrontendsConfig(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue<MicrofrontendsCacheRecord?>(s_cacheKey, out var cacheRecord) &&
            cacheRecord is not null)
        {
            return cacheRecord;
        }

        var dbRecord = await LoadFromDatabase(cancellationToken) ?? throw new EntityNotFoundException("MF config not found");

        _memoryCache.Set(s_cacheKey, dbRecord, absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(s_expirationInMinutes));

        return dbRecord;
    }

    private async Task<MicrofrontendsCacheRecord?> LoadFromDatabase(CancellationToken cancellationToken)
    {
        return await _identityDbContext.MicrofrontendsConfigs
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new MicrofrontendsCacheRecord
            {
                Id = x.Id,
                JsonData = x.JsonData,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void Clear()
    {
        _memoryCache.Remove(s_cacheKey);
    }
}
