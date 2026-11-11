using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.ResourceGroups.Requests.RequestDtos;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Requests;

internal sealed class GetResourceGroupByIdRequest : IQueryHandler<GetResourceGroupByIdRequest.Query, GroupDto>
{
    private readonly IIdentityDbContext _identityDbContext;

    public GetResourceGroupByIdRequest(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task<GroupDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var group = await _identityDbContext.ResourceGroups
            .Where(resourceGroup => resourceGroup.Id == request.Id)
            .Select(resourceGroup => new
            {
                resourceGroup.Id,
                resourceGroup.Name,
                resourceGroup.DepartmentId
            })
            .FirstOrDefaultAsync(cancellationToken);

        var groupManager = await _identityDbContext.Users
            .Where(user => user.ResourceGroupId == request.Id)
            .Where(user => user.IsActive)
            .Where(user => user.UserRoles.First().Role!.Name == PortalRoleNames.GroupManager)
            .Select(user => new
            {
                user.Id,
                user.FirstName,
                user.LastName
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (group is null)
        {
            throw new EntityNotFoundException(PortalMessages.GroupNotFound);
        }

        return new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            GroupManagerId = groupManager?.Id,
            ManagerFirstName = groupManager?.FirstName,
            ManagerLastName = groupManager?.LastName,
            DepartmentId = group.DepartmentId
        };
    }

    internal sealed record Query(int Id) : IQuery<GroupDto>;
}
