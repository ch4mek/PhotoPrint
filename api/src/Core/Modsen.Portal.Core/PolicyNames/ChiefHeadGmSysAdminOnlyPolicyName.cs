using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Core.PolicyNames;

public static class ChiefHeadGmSysAdminOnlyPolicyName
{
    public const string Name = "ChiefHeadGmSysAdminOnlyPolicy";
    public static readonly IReadOnlyCollection<string> Values =
    [
        PortalRoleNames.Ceo,
        PortalRoleNames.Cto,
        PortalRoleNames.HeadOfDepartment,
        PortalRoleNames.GroupManager,
        PortalRoleNames.SystemAdministrator,
    ];
}
