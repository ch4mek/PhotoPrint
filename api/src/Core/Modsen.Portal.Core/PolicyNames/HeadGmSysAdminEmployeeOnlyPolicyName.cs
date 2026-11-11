using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Core.PolicyNames;

public static class HeadGmSysAdminEmployeeOnlyPolicyName
{
    public const string Name = "HeadGmSysAdminEmployeeOnlyPolicy";
    public static readonly IReadOnlyCollection<string> Values =
    [
        PortalRoleNames.HeadOfDepartment,
        PortalRoleNames.GroupManager,
        PortalRoleNames.SystemAdministrator,
        PortalRoleNames.Employee,
    ];
}
