using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Cache
{
    internal interface IDepartmentsCache : IRecyclableCache
    {
        ValueTask<DepartmentCacheRecord?> GetDepartmentById(string id, CancellationToken cancellationToken);
        ValueTask<List<DepartmentCacheRecord>> GetAllDepartments(CancellationToken cancellationToken);
    }

    internal record DepartmentCacheRecord
    {
        public required string Id { get; init; }
        public required string ShortName { get; init; }
        public required string FullName { get; init; }
        public string? ModsenApiValue { get; init; }
        public int OrderWeight { get; init; }
    }

    internal class DepartmentsCache : IDepartmentsCache
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IIdentityDbContext _identityDbContext;
        private const string s_cacheKey = "__Department_Entities";
        private const int s_expirationInMinutes = 120;

        public DepartmentsCache(IMemoryCache memoryCache,
            IIdentityDbContext identityDbContext)
        {
            _memoryCache = memoryCache;
            _identityDbContext = identityDbContext;
        }

        public async ValueTask<DepartmentCacheRecord?> GetDepartmentById(string id, CancellationToken cancellationToken)
        {
            var departments = await GetAllDepartments(cancellationToken);
            return departments.FirstOrDefault(x => x.Id == id);
        }

        public async ValueTask<List<DepartmentCacheRecord>> GetAllDepartments(CancellationToken cancellationToken)
        {
            if (_memoryCache.TryGetValue<List<DepartmentCacheRecord>>(s_cacheKey, out var departmentsFromCache) &&
                departmentsFromCache is { Count: > 0 })
            {
                return departmentsFromCache;
            }

            var departmentsFromDatabase = await LoadFromDatabase(cancellationToken);

            _memoryCache.Set(s_cacheKey, departmentsFromDatabase,
                absoluteExpirationRelativeToNow: TimeSpan.FromMinutes(s_expirationInMinutes));

            return departmentsFromDatabase;
        }

        private Task<List<DepartmentCacheRecord>> LoadFromDatabase(CancellationToken cancellationToken)
        {
            return _identityDbContext.Departments
                .AsNoTracking()
                .OrderBy(e => e.OrderWeight)
                .Select(x => new DepartmentCacheRecord
                {
                    Id = x.Id,
                    ShortName = x.ShortName,
                    FullName = x.FullName,
                    ModsenApiValue = x.ModsenApiValue,
                    OrderWeight = x.OrderWeight
                })
                .ToListAsync(cancellationToken);
        }

        public void Clear()
        {
            _memoryCache.Remove(s_cacheKey);
        }
    }
}
