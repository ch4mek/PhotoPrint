using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;

namespace Modsen.Portal.TimeTrack.UseCases.BusinessRules;

internal interface ICanUserReportBeExportedBusinessRule : IBusinessRuleWithFailReason<int>;

internal class CanUserReportBeExportedBusinessRule : ICanUserReportBeExportedBusinessRule
{
    private readonly IGetTimeTrackUserByIdWithDepartmentDbQuery _getTimeTrackUserByIdWithDepartmentDbQuery;
    private readonly IAuthenticatedUserContext _userContext;

    public CanUserReportBeExportedBusinessRule(
        IGetTimeTrackUserByIdWithDepartmentDbQuery getTimeTrackUserByIdWithDepartmentDbQuery,
        IAuthenticatedUserContext userContext)
    {
        _getTimeTrackUserByIdWithDepartmentDbQuery = getTimeTrackUserByIdWithDepartmentDbQuery;
        _userContext = userContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(int userId, CancellationToken cancellationToken)
    {
        var user = await _getTimeTrackUserByIdWithDepartmentDbQuery.ExecuteAsync(userId, cancellationToken);

        if (user.DepartmentId is null)
        {
            throw new EntityNotFoundException($"User with id = {userId} does not have departmentId " +
                $"or does not exist");
        }

        if (_userContext.GetRoles().IsHeadOfDepartment()
            && _userContext.DepartmentId != user.DepartmentId)
        {
            return BusinessRuleResult.Fail(PortalMessages.HeadCantExportReportOfUserFromOtherDepartment);
        }

        if (_userContext.GetRoles().IsGroupManager()
            && _userContext.ResourceGroupId != user.ResourceGroupId)
        {
            return BusinessRuleResult.Fail(PortalMessages.GroupManagerCantExportReportOfUserFromOtherGroup);
        }

        return BusinessRuleResult.Success();
    }
}
