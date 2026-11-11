using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.Loaders;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Requests;

internal sealed class GetUserPermissionAssignmentRequest
    : IQueryHandler<GetUserPermissionAssignmentRequest.Query, PermissionAssignmentDto?>
{
    private readonly IPermissionAssignmentsLoader _permissionAssignmentsLoader;
    private readonly IIdentityDbContext _identityDbContext;

    public GetUserPermissionAssignmentRequest(IPermissionAssignmentsLoader permissionAssignmentsLoader,
        IIdentityDbContext identityDbContext)
    {
        _permissionAssignmentsLoader = permissionAssignmentsLoader;
        _identityDbContext = identityDbContext;
    }

    public async Task<PermissionAssignmentDto?> Handle(Query request, CancellationToken cancellationToken)
    {
        var permissionAssignmentId = await _identityDbContext.PermissionAssignments.Where(pa =>
                pa.UserId == request.UserId &&
                pa.ResourceId == request.ResourceId &&
                pa.ResourceType == request.ResourceType.ToString() &&
                pa.Permission!.Name == request.PermissionType.ToString())
            .Select(pa => pa.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var permissionAssignment = await _permissionAssignmentsLoader.LoadAsync(permissionAssignmentId, cancellationToken);

        return PermissionAssignmentDto.Of(permissionAssignment);
    }

    internal sealed record Query(
        int UserId,
        string ResourceId,
        PermissionResourceType ResourceType,
        PermissionType PermissionType) : IQuery<PermissionAssignmentDto?>;
}
