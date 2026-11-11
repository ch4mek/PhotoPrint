using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Core.PolicyNames;

public static class ChiefHeadSysAdminOnlyPolicyName
{
    public const string Name = "ChiefHeadSysAdminOnlyPolicy";
    public static readonly IReadOnlyCollection<string> Values =
    [
        PortalRoleNames.Ceo,
        PortalRoleNames.Cto,
        PortalRoleNames.HeadOfDepartment,
        PortalRoleNames.SystemAdministrator,
    ];
}
