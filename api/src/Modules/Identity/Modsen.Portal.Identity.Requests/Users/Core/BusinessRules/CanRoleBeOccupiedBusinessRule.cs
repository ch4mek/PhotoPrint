using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;

namespace Modsen.Portal.Identity.Requests.Users.Core.BusinessRules;

internal interface ICanRoleBeOccupiedBusinessRule : IBusinessRuleWithFailReason<CanRoleBeOccupiedBusinessRule.Request>;

internal class CanRoleBeOccupiedBusinessRule : ICanRoleBeOccupiedBusinessRule
{
    private readonly IDepartmentsCache _departmentsCache;
    private readonly IIdentityDbContext _identityDbContext;

    public CanRoleBeOccupiedBusinessRule(
        IIdentityDbContext identityDbContext,
        IDepartmentsCache departmentsCache)
    {
        _departmentsCache = departmentsCache;
        _identityDbContext = identityDbContext;
    }

    public async Task<BusinessRuleResult> IsCorrespondsToBusiness(Request? request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var isManagerPosition = await _identityDbContext.Roles
            .AnyAsync(x => x.Name == request.Position && x.IsCompanyManager, cancellationToken);

        if (isManagerPosition)
        {
            var isRoleOccupied = await IsRoleOccupied(request, cancellationToken);

            if (isRoleOccupied && request.Position.IsHeadOfDepartment())
            {
                var departments = await _departmentsCache.GetAllDepartments(cancellationToken);
                var department = departments.First(x => x.Id == request.DepartmentId);

                return BusinessRuleResult.Fail($"User with Head of {department.FullName} department role already exists");
            }

            if (isRoleOccupied && !request.Position.IsGroupManager())
            {
                return BusinessRuleResult.Fail($"User with {request.Position} role is already exists");
            }

            if(isRoleOccupied && request.Position.IsGroupManager())
            {
                return BusinessRuleResult.Fail($"Group manager of resource group with id {request.ResourceGroupId} is already exists");
            }
        }

        return BusinessRuleResult.Success();
    }

    internal record Request(string? Position, string? DepartmentId, int? ResourceGroupId);

    private Task<bool> IsRoleOccupied(Request request, CancellationToken cancellationToken)
    {
        var query = _identityDbContext.Users
            .Where(x => x.UserRoles
                .Any(userRole => userRole.Role!.Name! == request.Position)
            )
            .Where(x => x.IsActive);

        query = request.DepartmentId is null || request.Position.IsCeo() || request.Position.IsCto()
            ? query
            : query.Where(x => x.DepartmentId == request.DepartmentId);

        query = request.Position.IsGroupManager()
            ? query.Where(x => x.ResourceGroupId == request.ResourceGroupId && x.ResourceGroup != null)
            : query;

        return query.AnyAsync(cancellationToken);
    }
}
