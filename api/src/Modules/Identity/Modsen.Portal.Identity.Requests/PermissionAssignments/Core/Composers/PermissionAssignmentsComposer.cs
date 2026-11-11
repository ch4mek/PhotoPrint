using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Core.Composers;

internal interface IPermissionAssignmentsComposer
{
    Task<ICollection<PermissionAssignmentCoreDto>> ComposeAsync(PermissionAssignmentsComposerRequest request,
        CancellationToken cancellationToken);
}

internal sealed class PermissionAssignmentsComposer : IPermissionAssignmentsComposer
{
    private readonly IIdentityDbContext _identityDbContext;

    public PermissionAssignmentsComposer(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task<ICollection<PermissionAssignmentCoreDto>> ComposeAsync(
        PermissionAssignmentsComposerRequest request,
        CancellationToken cancellationToken)
    {
        var permissionName = request.PermissionType.ToString();

        var permission = _identityDbContext.PermissionDefinitions
            .FirstOrDefault(p => p.Name == permissionName);

        if (permission is null)
        {
            throw new EntityNotFoundException($"Permission \"{permissionName}\" does not exist");
        }

        var newAssignments = request.Resources
            .Select(resource => new PermissionAssignment
            {
                PermissionDefinitionId = permission.Id,
                UserId = request.UserId,
                ResourceId = resource.Key,
                ResourceName = resource.Value,
                ResourceType = request.ResourceType.ToString()
            })
            .ToList();

        _identityDbContext.PermissionAssignments.AddRange(newAssignments);
        await _identityDbContext.SaveChanges(cancellationToken);

        return newAssignments.Select(PermissionAssignmentCoreDto.Of).ToList();
    }
}

internal sealed class PermissionAssignmentsComposerRequest
{
    public int UserId { get; set; }
    public PermissionResourceType ResourceType { get; set; }
    public required IReadOnlyDictionary<string, string> Resources { get; set; }
    public PermissionType PermissionType { get; set; }
}
