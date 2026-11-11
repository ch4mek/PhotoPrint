using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;

namespace Modsen.Portal.TimeTrack.UseCases.BusinessRules;

internal interface ICanTimeTrackUserBeRetrievedWithSelfBusinessRule : IBusinessRuleWithFailReason<CanTimeTrackUserBeRetrievedWithSelfBusinessRuleModel>;

internal record CanTimeTrackUserBeRetrievedWithSelfBusinessRuleModel(int UserId);

internal class CanTimeTrackUserBeRetrievedWithSelfBusinessRule : ICanTimeTrackUserBeRetrievedWithSelfBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetTimeTrackUserByIdWithDepartmentDbQuery _getTimeTrackUserWithDepartmentByIdDbQuery;
    private readonly IIsTimeTrackUserExistingByUserIdDbQuery _isTimeTrackUserExistingByUserIdDbQuery;

    public CanTimeTrackUserBeRetrievedWithSelfBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IGetTimeTrackUserByIdWithDepartmentDbQuery getTimeTrackUserWithDepartmentByIdDbQuery,
        IIsTimeTrackUserExistingByUserIdDbQuery isTimeTrackUserExistingByUserIdDbQuery)
    {
        _isTimeTrackUserExistingByUserIdDbQuery = isTimeTrackUserExistingByUserIdDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
        _getTimeTrackUserWithDepartmentByIdDbQuery = getTimeTrackUserWithDepartmentByIdDbQuery;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(CanTimeTrackUserBeRetrievedWithSelfBusinessRuleModel? model,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (_authenticatedUserContext.UserId == model.UserId)
        {
            return BusinessRuleResult.Success();
        }

        if (_authenticatedUserContext.GetRoles().IsSystemAdministrator())
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

        if (user.Position.IsIntern() && !(_authenticatedUserContext.HasClaim(LabBoardClaimTypeNames.ManageAllInterns.Name)
            || _authenticatedUserContext.HasClaim(LabBoardClaimTypeNames.ManageInterns.Name)
                && user.CreatedById == _authenticatedUserContext.UserId))
        {
            return BusinessRuleResult.Fail(PortalMessages.UserNotFound);
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee())
        {
            return BusinessRuleResult.Fail(PortalMessages.UserNotFound);
        }


        if ((user.Position.IsCto() || user.Position.IsCeo()) &&
            !_authenticatedUserContext.GetRoles().IsCeo() && !_authenticatedUserContext.GetRoles().IsCto())
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        return BusinessRuleResult.Success();
    }
}

