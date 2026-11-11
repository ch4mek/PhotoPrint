using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Core.Loaders;

internal interface IPermissionAssignmentsLoader
{
    Task<PermissionAssignmentCoreDto?> LoadAsync(int id, CancellationToken cancellationToken);
    Task<ICollection<PermissionAssignmentCoreDto>> LoadAsync(int[] ids, CancellationToken cancellationToken);
}

internal class PermissionAssignmentsLoader : IPermissionAssignmentsLoader
{
    private readonly IIdentityDbContext _identityDbContext;

    public PermissionAssignmentsLoader(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task<PermissionAssignmentCoreDto?> LoadAsync(int id, CancellationToken cancellationToken)
    {
        var permissionAssignment = await _identityDbContext.PermissionAssignments
            .AsNoTracking()
            .Where(pa => pa.Id == id)
            .Include(pa => pa.Permission)
            .FirstOrDefaultAsync(cancellationToken);

        return permissionAssignment is null
            ? null
            : PermissionAssignmentCoreDto.Of(permissionAssignment);
    }

    public async Task<ICollection<PermissionAssignmentCoreDto>> LoadAsync(int[] ids, CancellationToken cancellationToken)
    {
        var permissionAssignments = await _identityDbContext.PermissionAssignments
            .AsNoTracking()
            .Where(pa => ids.Contains(pa.Id))
            .Include(pa => pa.Permission)
            .ToListAsync(cancellationToken);

        return permissionAssignments.Select(PermissionAssignmentCoreDto.Of).ToList();
    }
}
