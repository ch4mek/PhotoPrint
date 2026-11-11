using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Requests;

namespace Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

internal interface ICanUserFieldsBeUpdatedBusinessRule :
    IBusinessRuleWithFailReason<CanUserFieldsBeUpdatedBusinessRule.Request>;

internal class CanUserFieldsBeUpdatedBusinessRule : ICanUserFieldsBeUpdatedBusinessRule
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanUserFieldsBeUpdatedBusinessRule(IAuthenticatedUserContext authenticatedUserContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
    }

    public Task<BusinessRuleResult> IsCorrespondsToBusiness(Request? model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        var hasNoFieldsForAdmin = model.Command.FirstName is null &&
                                  model.Command.LastName is null &&
                                  (model.Command.Email is null || model.User.Position.IsIntern());

        var hasAllFieldsForAdmin = model.Command.FirstName is not null &&
                                   model.Command.LastName is not null &&
                                   model.Command.Email is not null;

        var result = model.Command.CurrentUserRole switch
        {
            var role when (role.IsCeo() || role.IsCto()) && hasNoFieldsForAdmin && model.Command.Location is not null =>
                ValidateOtherUsersPermissions(model.Command, model.User),

            var role when role.IsHeadOfDepartment() && hasNoFieldsForAdmin && model.Command.Location is not null &&
            model.Command.DepartmentId is not null =>
                ValidateOtherUsersPermissions(model.Command, model.User),

            var role when role.IsSystemAdministrator() && hasAllFieldsForAdmin =>
                ValidateSystemAdministratorPermissions(model.User, model.Command),

            var role when role.IsGroupManager() && hasNoFieldsForAdmin && model.Command.Location is not null &&
                model.Command.DepartmentId is not null =>
                ValidateOtherUsersPermissions(model.Command, model.User),

            _ => BusinessRuleResult.Fail(PortalMessages.UnsupportedUserUpdate)

        };

        return Task.FromResult(!result.IsSuccess ? result : BusinessRuleResult.Success());
    }

    private static BusinessRuleResult ValidateSystemAdministratorPermissions(UserDto user, UpdateUserRequest.Command command)
    {
        if (user.Position != command.Position)
        {
            return BusinessRuleResult.Fail(PortalMessages.YouCannotChangeUsersRole);
        }

        if (user.DepartmentId != command.DepartmentId)
        {
            return BusinessRuleResult.Fail(PortalMessages.YouCannotChangeUsersDepartment);
        }

        if (command.ResourceGroupId.HasValue && user.ResourceGroupId != command.ResourceGroupId)
        {
            return BusinessRuleResult.Fail(PortalMessages.YouCannotChangeUsersResourceGroup);
        }

        return user.Location != command.Location
            ? BusinessRuleResult.Fail(PortalMessages.YouCannotChangeUsersLocation)
            : BusinessRuleResult.Success();
    }

    private BusinessRuleResult ValidateOtherUsersPermissions(UpdateUserRequest.Command command, UserDto user)
    {
        return command.CurrentUserRole switch
        {
            var role when role.IsCeo() => ValidateCeoPermissions(user, command),
            var role when role.IsCto() => ValidateCtoPermissions(user, command),
            var role when role.IsHeadOfDepartment() => ValidateHeadOfDepartmentPermissions(user, command),
            var role when role.IsGroupManager() => ValidateGroupManagerPermissions(user, command),
            _ => BusinessRuleResult.Fail(PortalMessages.UnsupportedRoleTypes)
        };
    }

    private static BusinessRuleResult ValidateCeoPermissions(UserDto user, UpdateUserRequest.Command command)
    {
        return user.Position.IsCeo() && user.Position != command.Position ||
            !user.Position.IsCeo() && command.Position.IsCeo()
                ? BusinessRuleResult.Fail(PortalMessages.YouCannotChangeRoleToCeo)
                : BusinessRuleResult.Success();
    }

    private static BusinessRuleResult ValidateCtoPermissions(UserDto user, UpdateUserRequest.Command command)
    {
        return user.Position.IsCeo() || command.Position.IsCeo()
           ? BusinessRuleResult.Fail(PortalMessages.YouCannotEditUsersWithCeoRole)
           : BusinessRuleResult.Success();
    }

    private static BusinessRuleResult ValidateHeadOfDepartmentPermissions(UserDto user, UpdateUserRequest.Command command)
    {
        var userForUpdateIsCeoOrCto = user.Position.IsCeo() || user.Position.IsCto();
        var userForUpdateIsNotFromCurrentUserDepartment = user.DepartmentId != command.CurrentUserDepartmentId;
        var isDepartmentChanged = user.DepartmentId != command.DepartmentId;
        var isRoleChanged = user.Position != command.Position;

        return userForUpdateIsCeoOrCto ||
            userForUpdateIsNotFromCurrentUserDepartment ||
            isDepartmentChanged ||
            isRoleChanged
                ? BusinessRuleResult.Fail(PortalMessages.YouCanEditEmployeesOnlyFromYourDepartment)
                : BusinessRuleResult.Success();
    }

    private BusinessRuleResult ValidateGroupManagerPermissions(UserDto user, UpdateUserRequest.Command command)
    {
        var userForUpdateIsCeoOrCto = user.Position.IsCeo() || user.Position.IsCto();
        var userForUpdateIsNotFromCurrentUserDepartment = user.DepartmentId != command.CurrentUserDepartmentId;
        var userForUpdateIsNotFromCurrentUserResourceGroup = user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId
            && user.Id != _authenticatedUserContext.UserId;
        var isGroupChanged = user.ResourceGroupId != command.ResourceGroupId;
        var isDepartmentChanged = user.DepartmentId != command.DepartmentId;

        if (user.ResourceGroupId != command.ResourceGroupId)
        {
            return BusinessRuleResult.Fail(PortalMessages.YouCannotChangeUsersResourceGroup);
        }

        return userForUpdateIsCeoOrCto ||
            userForUpdateIsNotFromCurrentUserDepartment ||
            isDepartmentChanged ||
            userForUpdateIsNotFromCurrentUserResourceGroup ||
            isGroupChanged
                ? BusinessRuleResult.Fail(PortalMessages.YouCanEditEmployeesOnlyFromYourGroup)
                : BusinessRuleResult.Success();
    }

    internal record Request(UserDto User, UpdateUserRequest.Command Command);
}
