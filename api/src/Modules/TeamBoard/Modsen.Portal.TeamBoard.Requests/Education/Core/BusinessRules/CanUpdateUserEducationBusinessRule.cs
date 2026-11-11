using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Education.Core.BusinessRules;

internal interface ICanUpdateUserEducationBusinessRule
{
    Task<bool> IsCorrespondsToRuleAsync(int userId, CancellationToken cancellationToken);
}

internal sealed class CanUpdateUserEducationBusinessRule : ICanUpdateUserEducationBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public CanUpdateUserEducationBusinessRule(IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<bool> IsCorrespondsToRuleAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers
                .FirstOrDefaultAsync(user => user.UserId == userId, cancellationToken) ??
            throw new EntityNotFoundException(PortalMessages.UserNotFound);

        if (_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            return true;
        }

        var roles = _authenticatedUserContext.GetRoles();
        if ((roles.IsGroupManager() && _authenticatedUserContext.ResourceGroupId != user.ResourceGroupId) ||
            (roles.IsHeadOfDepartment() && _authenticatedUserContext.DepartmentId != user.DepartmentId))
        {
            return false;
        }

        return true;
    }
}
