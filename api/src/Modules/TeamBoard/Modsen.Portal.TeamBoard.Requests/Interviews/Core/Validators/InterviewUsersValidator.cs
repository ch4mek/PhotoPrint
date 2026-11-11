using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;

namespace Modsen.Portal.TeamBoard.Requests.Interviews.Core.Validators;

internal interface IInterviewUsersValidator
{
    Task Validate(InterviewUsersValidator.Request? model, CancellationToken cancellationToken);
}

internal class InterviewUsersValidator : IInterviewUsersValidator
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardPortalSettingsAccessor _teamBoardPortalSettingsAccessor;

    public InterviewUsersValidator(IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardPortalSettingsAccessor teamBoardPortalSettingsAccessor)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardPortalSettingsAccessor = teamBoardPortalSettingsAccessor;
    }

    public async Task Validate(Request? model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        var settings = await _teamBoardPortalSettingsAccessor.GetSettings(cancellationToken);
        var salesDepartmentId = settings.SalesDepartmentId;

        if (model.User == null || !model.User.IsActive)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        if (_authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ManageAllInterviews.Name))
        {
            return;
        }

        if (_authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ManageInterviews.Name))
        {
            if (_authenticatedUserContext.DepartmentId == salesDepartmentId)
            {
                return;
            }

            if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
                model.User.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
            {
                throw new ActionNotAllowedException("You don't have access to user from another group");
            }

            if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
                model.User.DepartmentId != _authenticatedUserContext.DepartmentId)
            {
                throw new ActionNotAllowedException("You don't have access to user from another department");
            }

            return;
        }

        throw new ActionNotAllowedException();
    }

    public record Request(TeamBoardUser? User);
}
