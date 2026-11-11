using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Cache;

internal class PortalSettingCache : IPortalSettingsAccessor
{
    private readonly IDistributedCache _distributedCache;
    private readonly IIdentityDbContext _identityDbContext;

    private const string _cacheKey = "__PortalSettings";
    private const int _expirationInMinutes = 15;

    public PortalSettingCache(IDistributedCache distributedCache, IIdentityDbContext identityDbContext)
    {
        _distributedCache = distributedCache;
        _identityDbContext = identityDbContext;
    }

    public async Task<string> GetStringValueByName(string settingName, CancellationToken token)
    {
        if (settingName.IsNullOrEmpty())
        {
            return string.Empty;
        }

        var settings = await GetAll(token);
        return settings
            .Where(x => string.Equals(x.Name, settingName, StringComparison.Ordinal))
            .Select(x => x.Value)
            .FirstOrDefault() ?? throw new AppException($"Portal setting '{settingName}' is not configured");
    }

    public async Task<IReadOnlyCollection<PortalSettingValue>> GetByNames(string[] settingNames, CancellationToken token)
    {
        if (settingNames.Length == 0)
        {
            return [];
        }

        var settings = await GetAll(token);
        return settings
            .Where(x => settingNames.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase))
            .ToArray();
    }

    public async Task<IReadOnlyCollection<PortalSettingValue>> GetAll(CancellationToken token)
    {
        var content = await _distributedCache.GetStringAsync(_cacheKey, token);
        if (content.IsNullOrEmpty())
        {
            var settings = await LoadFromDatabase(token);

            await _distributedCache.SetStringAsync(_cacheKey, JsonSerializer.Serialize(settings),
                new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(_expirationInMinutes) }, token);

            return settings;
        }

        return JsonSerializer.Deserialize<PortalSettingValue[]>(content) ?? [];
    }

    public void Clear()
    {
        _distributedCache.Remove(_cacheKey);
    }

    private Task<List<PortalSettingValue>> LoadFromDatabase(CancellationToken cancellationToken)
    {
        return _identityDbContext.PortalSettings
            .AsNoTracking()
            .Select(x => new PortalSettingValue
            {
                Name = x.Name,
                Value = x.Value,
                ValueType = x.ValueType
            })
            .ToListAsync(cancellationToken);
    }
}
