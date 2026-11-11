using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Cache;

internal interface IRedirectRulesCache : IRecyclableCache
{
    ValueTask<RedirectRuleCacheRecord?> GetRedirectRuleByKey(string key, CancellationToken cancellationToken);
    ValueTask<List<RedirectRuleCacheRecord>> GetAllRedirectRules(CancellationToken cancellationToken);
}

internal class RedirectRulesCache : IRedirectRulesCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly IIdentityDbContext _identityDbContext;
    private const string s_cacheKey = "__RedirectRule_Entities";
    private const int s_expirationInMinutes = 60;

    public RedirectRulesCache(IMemoryCache memoryCache,
            IIdentityDbContext identityDbContext)
    {
        _memoryCache = memoryCache;
        _identityDbContext = identityDbContext;
    }

    public async ValueTask<RedirectRuleCacheRecord?> GetRedirectRuleByKey(string key, CancellationToken cancellationToken)
    {
        var redirectRules = await GetAllRedirectRules(cancellationToken);
        return redirectRules.FirstOrDefault(x => x.RedirectKey.ToString() == key);
    }

    public async ValueTask<List<RedirectRuleCacheRecord>> GetAllRedirectRules(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue<List<RedirectRuleCacheRecord>>(s_cacheKey, out var redirectRuleFromCache)
            && redirectRuleFromCache is { Count: > 0 })
        {
            return redirectRuleFromCache;
        }

        var redirectRulesFromDatabase = await LoadFromDatabase(cancellationToken);

        _memoryCache.Set(s_cacheKey, redirectRulesFromDatabase,
            absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(s_expirationInMinutes));

        return redirectRulesFromDatabase;
    }

    private async Task<List<RedirectRuleCacheRecord>> LoadFromDatabase(CancellationToken cancellationToken)
    {
        return await _identityDbContext.RedirectRules
            .AsNoTracking()
            .Select(x => new RedirectRuleCacheRecord(x.Id, x.RedirectKey, x.RedirectTo))
            .ToListAsync(cancellationToken);
    }

    public void Clear()
    {
        _memoryCache.Remove(s_cacheKey);
    }
}

public record RedirectRuleCacheRecord(Guid Id, string RedirectKey, string RedirectTo);
