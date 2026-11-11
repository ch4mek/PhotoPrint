using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;

namespace Modsen.Portal.TimeTrack.UseCases.BusinessRules;

internal interface ICanTimeTrackUserBeRetrievedBusinessRule : IBusinessRuleWithFailReason<CanTimeTrackUserBeRetrievedBusinessRuleModel>;

internal record CanTimeTrackUserBeRetrievedBusinessRuleModel(int UserId);

internal class CanTimeTrackUserBeRetrievedBusinessRule : ICanTimeTrackUserBeRetrievedBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetTimeTrackUserByIdWithDepartmentDbQuery _getTimeTrackUserWithDepartmentByIdDbQuery;
    private readonly IIsTimeTrackUserExistingByUserIdDbQuery _isTimeTrackUserExistingByUserIdDbQuery;

    public CanTimeTrackUserBeRetrievedBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IGetTimeTrackUserByIdWithDepartmentDbQuery getTimeTrackUserWithDepartmentByIdDbQuery,
        IIsTimeTrackUserExistingByUserIdDbQuery isTimeTrackUserExistingByUserIdDbQuery)
    {
        _isTimeTrackUserExistingByUserIdDbQuery = isTimeTrackUserExistingByUserIdDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _getTimeTrackUserWithDepartmentByIdDbQuery = getTimeTrackUserWithDepartmentByIdDbQuery;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(CanTimeTrackUserBeRetrievedBusinessRuleModel? model,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (_authenticatedUserContext.GetRoles().IsSystemAdministrator() ||
            _authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsIntern())
        {
            return BusinessRuleResult.Fail(PortalMessages.UserNotFound);
        }

        var isUserExists = await _isTimeTrackUserExistingByUserIdDbQuery
            .ExecuteAsync(model.UserId, cancellationToken);

        if (!isUserExists)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        var user = await _getTimeTrackUserWithDepartmentByIdDbQuery.ExecuteAsync(model.UserId, cancellationToken);

        if (user.IsActive == false)
        {
            return BusinessRuleResult.Fail(PortalMessages.UserIsInactive);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() && user.DepartmentId != _authenticatedUserContext.DepartmentId)
        {
            return BusinessRuleResult.Fail(PortalMessages.HeadDoNotHaveAccessToOtherDepartments);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager() && user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
        {
            return BusinessRuleResult.Fail(PortalMessages.GroupManagerDoNotHaveAccessToOtherGroup);
        }

        if ((user.Position.IsCto() || user.Position.IsCeo()) &&
            !_authenticatedUserContext.GetRoles().IsCeo() && !_authenticatedUserContext.GetRoles().IsCto())
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        return BusinessRuleResult.Success();
    }
}
