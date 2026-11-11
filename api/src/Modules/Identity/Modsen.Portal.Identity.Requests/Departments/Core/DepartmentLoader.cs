using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.Requests.Departments.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.Departments.Core;

internal interface IDepartmentLoader
{
    Task<List<DepartmentCoreDto>> LoadDepartments(CancellationToken cancellationToken);
    Task<List<DepartmentClassificationCoreDto>> LoadDepartmentsClassifications(CancellationToken cancellationToken);
}

internal class DepartmentLoader : IDepartmentLoader
{
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IDepartmentsCache _departmentsCache;

    public DepartmentLoader(
        IIdentityDbContext identityDbContext,
        IDepartmentsCache departmentsCache)
    {
        _identityDbContext = identityDbContext;
        _departmentsCache = departmentsCache;
    }

    public async Task<List<DepartmentCoreDto>> LoadDepartments(CancellationToken cancellationToken)
    {
        var heads = await _identityDbContext.Users
            .AsNoTracking()
            .Where(x => x.IsActive && x.UserRoles.Any(u => u.Role != null && u.Role!.Name == PortalRoleNames.HeadOfDepartment))
            .Select(x => new
            {
                x.Id,
                x.DepartmentId,
                x.Email
            })
            .ToListAsync(cancellationToken);

        var departments = await _departmentsCache.GetAllDepartments(cancellationToken);

        var activeGroups = await GetActiveGroups(cancellationToken);

        return departments.Select(x => new DepartmentCoreDto
        {
            DepartmentName = x.FullName,
            Groups = activeGroups
                .Where(g => g.DepartmentId == x.Id)
                .OrderBy(g => g.Id)
                .ToList(),
            HeadOfDepartmentId = heads.Find(h => h.DepartmentId == x.Id)?.Email
        }).ToList();
    }

    public async Task<List<DepartmentClassificationCoreDto>> LoadDepartmentsClassifications(CancellationToken cancellationToken)
    {
        var departments = await _departmentsCache.GetAllDepartments(cancellationToken);

        return departments.ConvertAll(x => new DepartmentClassificationCoreDto(x.Id, x.ShortName, x.FullName));
    }

    private async Task<List<DepartmentGroupCoreDto>> GetActiveGroups(CancellationToken cancellationToken)
    {
        var query = _identityDbContext.ResourceGroups
            .AsNoTracking()
            .Where(group => group.IsActive)
            .AsQueryable();

        return await query.GroupJoin(
            _identityDbContext.Users.Where(
                user => user.IsActive && user.UserRoles.First().Role!.Name == PortalRoleNames.GroupManager),
            group => group.Id,
            user => user.ResourceGroupId,
            (group, users) => new { group, manager = users.FirstOrDefault() })
            .Select(
            g => new DepartmentGroupCoreDto
            {
                Id = g.group.Id,
                Name = g.group.Name,
                DepartmentId = g.group.DepartmentId,
                GroupManagerEmail = g.manager != null ? g.manager.Email : null
            })
            .ToListAsync(cancellationToken);
    }
}
