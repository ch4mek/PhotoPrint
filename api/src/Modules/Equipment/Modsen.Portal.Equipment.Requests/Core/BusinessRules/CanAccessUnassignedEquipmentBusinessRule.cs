using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Equipment.Requests.Core.BusinessRules;

internal interface ICanAccessUnassignedEquipmentBusinessRule
{
    bool IsCorrespondsToBusiness(IAuthenticatedUserContext? authenticatedUserContext);
}

internal sealed class CanAccessUnassignedEquipmentBusinessRule : ICanAccessUnassignedEquipmentBusinessRule
{
    public bool IsCorrespondsToBusiness(IAuthenticatedUserContext? authenticatedUserContext)
    {
        var hasAvailableClaim = authenticatedUserContext!.HasClaimWithValue(EquipmentClaimTypeNames.AvailableEquipmentsWithoutAssigners,
            authenticatedUserContext.DepartmentId!);

        var isAvailableEquipmentsWithoutAssigners = authenticatedUserContext.GetRoles() switch
        {
            var roles when roles.IsHeadOfDepartment() => hasAvailableClaim,
            var roles when roles.IsGroupManager() => hasAvailableClaim,
            var roles when roles.IsSystemAdministrator() => true,
            var roles when roles.IsEmployee() => false,
            var roles when roles.IsCeo() || roles.IsCto() => true,
            _ => throw new InvalidDataAppException(PortalMessages.UnsupportedRoleTypes)
        };

        return isAvailableEquipmentsWithoutAssigners;
    }
}
