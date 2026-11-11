using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Services;

internal class UserAccessService : IUserAccessService
{
    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly IGetTimeTrackUserByIdWithDepartmentDbQuery _getTimeTrackUserByIdWithDepartmentDbQuery;

    public UserAccessService(IAuthenticatedUserContext authenticatedUser,
        IGetTimeTrackUserByIdWithDepartmentDbQuery getTimeTrackUserByIdWithDepartmentDbQuery)
    {
        _authenticatedUser = authenticatedUser;
        _getTimeTrackUserByIdWithDepartmentDbQuery = getTimeTrackUserByIdWithDepartmentDbQuery;
    }

    public async Task<bool> CanAccessUserReportInformation(int userId, CancellationToken cancellationToken)
    {
        if(_authenticatedUser.GetRoles().IsCeo() || _authenticatedUser.GetRoles().IsCto() ||
            userId == _authenticatedUser.UserId)
        {
            return true;
        }

        var user = await _getTimeTrackUserByIdWithDepartmentDbQuery.ExecuteAsync(userId, cancellationToken);

        if(_authenticatedUser.GetRoles().IsHeadOfDepartment() && user.DepartmentId == _authenticatedUser.DepartmentId)
        {
            return true;
        }

        if (_authenticatedUser.GetRoles().IsGroupManager() && user.ResourceGroupId == _authenticatedUser.ResourceGroupId)
        {
            return true;
        }

        return false;
    }
}
