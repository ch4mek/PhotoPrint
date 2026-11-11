using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;

namespace Modsen.Portal.TeamBoard.Requests.Interviews.Core.AuthenticationFilters;

internal interface IInterviewsAuthenticationFilter
{
    IQueryable<UserInterview> Apply(IQueryable<UserInterview> interviews);
    Task<IQueryable<TeamBoardUser>> Apply(IQueryable<TeamBoardUser> users, CancellationToken cancellationToken);
}

internal class InterviewsAuthenticationFilter : IInterviewsAuthenticationFilter
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardPortalSettingsAccessor _teamBoardPortalSettingsAccessor;


    public InterviewsAuthenticationFilter(IAuthenticatedUserContext authenticatedUserContext, ITeamBoardPortalSettingsAccessor teamBoardPortalSettingsAccessor)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardPortalSettingsAccessor = teamBoardPortalSettingsAccessor;
    }

    public IQueryable<UserInterview> Apply(IQueryable<UserInterview> interviews)
    {
        interviews = interviews.Where(x => x.User!.Position != PortalRoleNames.Intern);

        var authRoles = _authenticatedUserContext.GetRoles();

        if (_authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name))
        {
            return interviews;
        }

        if (_authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ManageInterviews.Name))
        {
            if (authRoles.IsHeadOfDepartment())
            {
                return interviews.Where(x => x.User!.DepartmentId == _authenticatedUserContext.DepartmentId
                    || x.CreatedById == _authenticatedUserContext.UserId
                    || x.CreatedByUser!.DepartmentId == _authenticatedUserContext.DepartmentId
                    || x.SalesResponsibleUserId == _authenticatedUserContext.UserId);
            }

            if (authRoles.IsGroupManager())
            {
                return interviews.Where(x => x.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId
                    || x.CreatedById == _authenticatedUserContext.UserId
                    || x.CreatedByUser!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId
                    || x.SalesResponsibleUserId == _authenticatedUserContext.UserId);
            }

            return interviews.Where(x => x.CreatedById == _authenticatedUserContext.UserId
                    || x.SalesResponsibleUserId == _authenticatedUserContext.UserId);
        }

        throw new ActionNotAllowedException();
    }

    public async Task<IQueryable<TeamBoardUser>> Apply(IQueryable<TeamBoardUser> users, CancellationToken cancellationToken)
    {
        var authRoles = _authenticatedUserContext.GetRoles();
        if (_authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name))
        {
            return users;
        }

        var settings = await _teamBoardPortalSettingsAccessor.GetSettings(cancellationToken);
        var salesDepartmentId = settings.SalesDepartmentId;

        if (_authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ManageInterviews.Name))
        {
            if (_authenticatedUserContext.DepartmentId == salesDepartmentId)
            {
                return users;
            }

            if (authRoles.IsHeadOfDepartment())
            {
                return users.Where(x => x.DepartmentId == _authenticatedUserContext.DepartmentId);
            }

            if (authRoles.IsGroupManager())
            {
                return users.Where(x => x.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
            }

            return users.Take(0);
        }

        throw new ActionNotAllowedException();
    }
}
