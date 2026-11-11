using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.Requests.Dto;

namespace Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

internal interface ICanUserBeUpdatedBusinessRule :
    IBusinessRuleWithFailReason<CanUserBeUpdatedBusinessRule.Request>;

internal class CanUserBeUpdatedBusinessRule : ICanUserBeUpdatedBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanUserBeUpdatedBusinessRule(IAuthenticatedUserContext authenticatedUserContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
    }

    public Task<BusinessRuleResult> IsCorrespondsToBusiness(Request? model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        var result =  BusinessRuleResult.Success();

        if (!_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            result = _authenticatedUserContext.GetRoles() switch
            {
                var role when role.IsCeo() =>
                    ValidateCeoPermissions(model),

                var role when role.IsCto() =>
                    ValidateCtoPermissions(model),

                var role when role.IsHeadOfDepartment() =>
                    ValidateHeadOfDepartmentPermissions(model),

                var role when role.IsGroupManager() =>
                    ValidateGroupManagerPermissions(model),

                _ => BusinessRuleResult.Fail(PortalMessages.UnsupportedRoleTypes)
            };
        }

        return Task.FromResult(!result.IsSuccess ? result : BusinessRuleResult.Success());
    }

    private static BusinessRuleResult ValidateCeoPermissions(Request request)
    {
        return request.User.Position.IsCto() || request.User.Position.IsCeo()
            ? BusinessRuleResult.Fail(PortalMessages.UnsupportedUserUpdate)
            : BusinessRuleResult.Success();
    }

    private static BusinessRuleResult ValidateCtoPermissions(Request request)
    {
        return request.User.Position.IsCto() || request.User.Position.IsCeo()
            ? BusinessRuleResult.Fail(PortalMessages.UnsupportedUserUpdate)
            : BusinessRuleResult.Success();
    }

    private BusinessRuleResult ValidateHeadOfDepartmentPermissions(Request request)
    {
        var isEditingHimself = request.User.Id == _authenticatedUserContext.UserId;
        var isDepartmentChanged = request.User.DepartmentId != request.DepartmentId;
        var isEmailChanged = request.User.Email != request.Email;
        var userForUpdateIsCeoOrCto = request.User.Position.IsCeo() || request.User.Position.IsCto();
        var userForUpdateIsNotFromCurrentUserDepartment = request.User.DepartmentId != _authenticatedUserContext.DepartmentId;

        if (isEditingHimself)
        {
            return BusinessRuleResult.Fail("Head of department cannot edit himself");
        }

        if (isEmailChanged || isDepartmentChanged)
        {
            return BusinessRuleResult.Fail(PortalMessages.UnsupportedUserUpdate);
        }

        if (userForUpdateIsCeoOrCto || userForUpdateIsNotFromCurrentUserDepartment)
        {
            return BusinessRuleResult.Fail(PortalMessages.YouCanEditEmployeesOnlyFromYourDepartment);
        }

        return BusinessRuleResult.Success();
    }

    private BusinessRuleResult ValidateGroupManagerPermissions(Request request)
    {
        var userForUpdateIsCeoOrCto = request.User.Position.IsCeo() || request.User.Position.IsCto();
        var userForUpdateIsNotFromCurrentUserDepartment = request.User.DepartmentId != _authenticatedUserContext.DepartmentId;
        var userForUpdateIsNotFromCurrentUserResourceGroup = request.User.ResourceGroupId != _authenticatedUserContext.ResourceGroupId;
        var isGroupChanged = request.User.ResourceGroupId != request.ResourceGroupId;
        var isDepartmentChanged = request.User.DepartmentId != request.DepartmentId;
        var isGroupManagerEditingHimself = request.User.Id == _authenticatedUserContext.UserId;
        var isEmailChanged = request.User.Email != request.Email;

        if (isGroupManagerEditingHimself)
        {
            return BusinessRuleResult.Fail("GM cannot edit himself");
        }

        if (isGroupChanged)
        {
            return BusinessRuleResult.Fail(PortalMessages.YouCannotChangeUsersResourceGroup);
        }

        if (userForUpdateIsCeoOrCto ||
            userForUpdateIsNotFromCurrentUserDepartment ||
            userForUpdateIsNotFromCurrentUserResourceGroup ||
            isDepartmentChanged || isGroupChanged)
        {
            return BusinessRuleResult.Fail(PortalMessages.YouCanEditEmployeesOnlyFromYourGroup);
        }

        if (isEmailChanged)
        {
            return BusinessRuleResult.Fail(PortalMessages.UnsupportedUserUpdate);
        }

        return BusinessRuleResult.Success();
    }

    internal record Request(UserDto User, string? DepartmentId, int? ResourceGroupId, string? Email);
}
