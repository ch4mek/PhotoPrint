using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Equipment.DataAccess.Npgsql;

namespace Modsen.Portal.Equipment.Requests.Users.BusinessRules;

internal interface IIsAllowedToGetUserEquipmentBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int? userId, CancellationToken cancellationToken);
}

internal class IsAllowedToGetUserEquipmentBusinessRule : IIsAllowedToGetUserEquipmentBusinessRule, IBusinessRule<int?>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IEquipmentDbContext _equipmentDbContext;

    public IsAllowedToGetUserEquipmentBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IEquipmentDbContext equipmentDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _equipmentDbContext = equipmentDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(int? userId, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.UserId == userId)
        {
            return true;
        }

        var isEquipmentUserFromDepartment = _authenticatedUserContext.DepartmentId != null &&
            await _equipmentDbContext.EquipmentUsers
                .Where(x => x.UserId == userId)
                .Where(x => x.DepartmentId == _authenticatedUserContext.DepartmentId)
                .AnyAsync(cancellationToken);

        var isEquipmentUserFromResourceGroup = _authenticatedUserContext.ResourceGroupId != null &&
            await _equipmentDbContext.EquipmentUsers
                .Where(x => x.UserId == userId)
                .Where(x => x.ResourceGroupId == _authenticatedUserContext.ResourceGroupId)
                .AnyAsync(cancellationToken);

        return _authenticatedUserContext.GetRoles() switch
        {
            var roles when roles.IsCeo() || roles.IsCto() => true,
            var roles when roles.IsHeadOfDepartment() => isEquipmentUserFromDepartment,
            var roles when roles.IsGroupManager() => isEquipmentUserFromResourceGroup,
            _ => false
        };
    }
}
