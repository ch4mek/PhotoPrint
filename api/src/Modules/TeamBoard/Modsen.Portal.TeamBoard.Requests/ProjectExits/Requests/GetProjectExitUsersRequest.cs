using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;

internal sealed class GetProjectExitUsersRequest : IQueryHandler<GetProjectExitUsersRequest.Query, List<ProjectExitUserLookupDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetProjectExitUsersRequest(IAuthenticatedUserContext authenticatedUserContext, ITeamBoardDbContext teamBoardDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<List<ProjectExitUserLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var hasManageClaim = _authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name);
        var hasViewClaim = _authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ViewProjectExits.Name);
        if (!hasManageClaim
            && !hasViewClaim
            && !_authenticatedUserContext.GetRoles().IsCeo()
            && !_authenticatedUserContext.GetRoles().IsCto()
            && !_authenticatedUserContext.GetRoles().IsHeadOfDepartment()
            && !_authenticatedUserContext.GetRoles().IsGroupManager()
            && !_authenticatedUserContext.GetRoles().IsSystemAdministrator())
        {
            return [];
        }

        var query = _teamBoardDbContext.TeamBoardUsers.AsNoTracking();

        if (!request.IncludeInactive)
        {
            query = query.Where(user => user.IsActive);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() && !hasManageClaim)
        {
            query = query.Where(user => user.DepartmentId == _authenticatedUserContext.DepartmentId);

        } else if (_authenticatedUserContext.GetRoles().IsGroupManager() && !hasManageClaim)
        {
            query = query.Where(user => user.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        query = query.OrderBy(user => user.UserId);

        return await query.Select(user => new ProjectExitUserLookupDto
            {
                Id = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FirstNameRu = user.FirstNameRu,
                LastNameRu = user.LastNameRu,
                DepartmentId = user.DepartmentId,
                ResourceGroupName = user.ResourceGroup == null || !user.ResourceGroup.IsActive
                    ? null
                    : user.ResourceGroup.ResourceGroupName,
                Color = user.AvatarColor,
                IsActive = user.IsActive,
                AvatarFileId = user.AvatarFileId,
                ResourceGroupId =  user.ResourceGroupId
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(bool IncludeInactive) : IQuery<List<ProjectExitUserLookupDto>>;
}
