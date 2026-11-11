using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;

internal sealed class GetUserDetailsRequest : IQueryHandler<GetUserDetailsRequest.Query, UserPlanDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetUserDetailsRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<UserPlanDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers
            .Where(userDetails => userDetails.UserId == request.UserId)
            .Select(user => new
            {
                user.ResourceGroupId,
                user.DepartmentId,
                UserDetails = user.UserPayloadDetails
                    .Select(details => new UserPlanDto(details.Plan))
                    .FirstOrDefault() ?? new UserPlanDto(null)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            user.DepartmentId != _authenticatedUserContext.DepartmentId)
        {
            throw new ActionNotAllowedException(PortalMessages.YouCannotViewPlanOfUserFromAnotherDepartment);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
            user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
        {
            throw new ActionNotAllowedException(PortalMessages.YouCannotViewPlanOfUserFromAnotherResourceGroup);
        }

        if (_authenticatedUserContext.UserId == request.UserId)
        {
            throw new ActionNotAllowedException(PortalMessages.YouCannotViewYourOwnPlan);
        }

        return user.UserDetails;
    }

    internal sealed record Query(int UserId) : IQuery<UserPlanDto>;
}

