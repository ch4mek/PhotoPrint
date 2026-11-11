using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Core.PolicyNames;

public static class ChiefHeadGmEmployeeOnlyPolicyName
{
    public const string Name = "ChiefHeadGmEmployeeOnlyPolicy";
    public static readonly IReadOnlyCollection<string> Values =
    [
        PortalRoleNames.Ceo,
        PortalRoleNames.Cto,
        PortalRoleNames.HeadOfDepartment,
        PortalRoleNames.GroupManager,
        PortalRoleNames.Employee,
    ];
}
