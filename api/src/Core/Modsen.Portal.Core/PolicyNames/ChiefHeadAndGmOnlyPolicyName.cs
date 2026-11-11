using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Core.PolicyNames;

public static class ChiefHeadAndGmOnlyPolicyName
{
    public const string Name = "ChiefHeadAndGmOnlyPolicy";
    public static readonly IReadOnlyCollection<string> Values =
    [
        PortalRoleNames.Ceo,
        PortalRoleNames.Cto,
        PortalRoleNames.HeadOfDepartment,
        PortalRoleNames.GroupManager
    ];
}
