using System.Globalization;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;

internal interface ICanTrackBillableHoursBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int userId, int projectId, int? trackedBillableMinutes,
        CancellationToken cancellationToken);
}

internal class CanTrackBillableHoursBusinessRule : ICanTrackBillableHoursBusinessRule
{
    private readonly IIdentityContract _identityContract;

    public CanTrackBillableHoursBusinessRule(IIdentityContract identityContract)
    {
        _identityContract = identityContract;
    }

    public async Task<bool> IsCorrespondsToBusiness(int userId, int projectId, int? trackedBillableMinutes,
        CancellationToken cancellationToken)
    {
        var permissionAssignmentDto = await _identityContract.GetPermissionAssignment(userId,
            projectId.ToString(CultureInfo.InvariantCulture),
            PermissionResourceType.Project,
            PermissionType.TrackBillableWorkingHours,
            cancellationToken);

        if (permissionAssignmentDto is null && trackedBillableMinutes != null)
        {
            return false;
        }

        return true;
    }
}
