using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.Cache;

internal interface IEquipmentTypesCache : IRecyclableCache
{
    ValueTask<EquipmentTypeRecord?> GetEquipmentTypeById(string id, CancellationToken cancellationToken);
    ValueTask<List<EquipmentTypeRecord>> GetAllEquipmentTypes(CancellationToken cancellationToken);
}

internal record EquipmentTypeRecord
{
    public string? Id  { get; set; }
    public string? ShortName { get; set; }
    public string? FullName { get; set; }
}

internal class EquipmentTypesCache : IEquipmentTypesCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly IEquipmentDbContext _equipmentDbContext;
    private const string s_cacheKey = "__Equipment_Entities";
    private const int s_expirationInMinutes = 60;

    public EquipmentTypesCache(IMemoryCache memoryCache, IEquipmentDbContext equipmentDbContext)
    {
        _memoryCache = memoryCache;
        _equipmentDbContext = equipmentDbContext;
    }

    public async ValueTask<List<EquipmentTypeRecord>> GetAllEquipmentTypes(CancellationToken cancellationToken)
    {
        if (_memoryCache.TryGetValue<List<EquipmentTypeRecord>>(s_cacheKey, out var equipmentTypesFromCache)
            && equipmentTypesFromCache is {Count: > 0})
        {
            return equipmentTypesFromCache;
        }

        var equipmentTypesFromDatabase = await LoadFromDatabase(cancellationToken);

        _memoryCache.Set(s_cacheKey, equipmentTypesFromDatabase,
            absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(s_expirationInMinutes));

        return equipmentTypesFromDatabase;
    }

    public async ValueTask<EquipmentTypeRecord?> GetEquipmentTypeById(string id, CancellationToken cancellationToken)
    {
        var equipments = await GetAllEquipmentTypes(cancellationToken);
        return equipments.FirstOrDefault(e => e.Id == id);
    }

    private Task<List<EquipmentTypeRecord>> LoadFromDatabase(CancellationToken cancellationToken)
    {
        return _equipmentDbContext.EquipmentTypes
            .AsNoTracking()
            .OrderBy(t => t.OrderWeight)
            .ThenBy(t => t.FullName)
            .Select(t => new EquipmentTypeRecord
            {
                Id = t.Id,
                FullName = t.FullName,
                ShortName = t.ShortName
            })
            .ToListAsync(cancellationToken);
    }

    public void Clear()
    {
        _memoryCache.Remove(s_cacheKey);
    }
}
