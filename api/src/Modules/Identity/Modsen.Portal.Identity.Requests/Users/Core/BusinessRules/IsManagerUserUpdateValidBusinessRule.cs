using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;

namespace Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

internal interface IIsManagerUserUpdateValidBusinessRule :
    IBusinessRuleWithFailReason<IsManagerUserUpdateValidBusinessRule.Request>;

internal class IsManagerUserUpdateValidBusinessRule : IIsManagerUserUpdateValidBusinessRule
{
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IDepartmentsCache _departmentsCache;

    public IsManagerUserUpdateValidBusinessRule(IIdentityDbContext identityDbContext, IDepartmentsCache departmentsCache)
    {
        _identityDbContext = identityDbContext;
        _departmentsCache = departmentsCache;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(
        Request? model,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        var isRequestProvidingDepartmentToCeoOrCto = (model.newPosition.IsCeo() || model.newPosition.IsCto()) &&
                                                     model.newDepartmentId != null;

        if (isRequestProvidingDepartmentToCeoOrCto)
        {
            return BusinessRuleResult.Fail(PortalMessages.CtoAndCeoCannotHaveDepartment);
        }

        var isManagerPosition = await _identityDbContext.Roles
            .AnyAsync(x => x.Name == model.newPosition && x.IsCompanyManager, cancellationToken);
        var isRequestChangingManagerPositionOrDepartment = isManagerPosition &&
            (model.userPosition != model.newPosition ||
            model.userDepartmentId != model.newDepartmentId);

        if (isRequestChangingManagerPositionOrDepartment)
        {
            var isRoleOccupied = await IsRoleOccupiedAsync(
                model.newPosition,
                model.newDepartmentId,
                model.newResourceGroupId,
                cancellationToken);

            if (isRoleOccupied && model.newPosition == PortalRoleNames.HeadOfDepartment)
            {
                var departments = await _departmentsCache.GetAllDepartments(cancellationToken);
                var department = departments.Single(department => department.Id == model.newDepartmentId);

                return BusinessRuleResult.Fail($"User with Head of {department.FullName} department role already exists");
            }

            if (isRoleOccupied)
            {
                return BusinessRuleResult.Fail($"A User {model.newPosition} is already exist");
            }
        }

        return BusinessRuleResult.Success();
    }

    private async Task<bool> IsRoleOccupiedAsync(
    string roleName,
    string? departmentId,
    int? resourceGroupId,
    CancellationToken cancellationToken)
    {
        var query = _identityDbContext.Users
            .Where(x => x.UserRoles
                .Any(userRole => userRole.Role!.Name! == roleName)
            )
            .Where(x => x.IsActive);

        query = departmentId is null || roleName.IsCeo() || roleName.IsCto()
            ? query
            : query.Where(x => x.DepartmentId == departmentId);

        query = roleName.IsGroupManager()
            ? query.Where(x => x.ResourceGroupId == resourceGroupId && x.ResourceGroupId != null)
            : query;

        return await query.AnyAsync(cancellationToken);
    }

    internal record Request(
        string newPosition,
        string? newDepartmentId,
        int? newResourceGroupId,
        string userPosition,
        string? userDepartmentId);
}
