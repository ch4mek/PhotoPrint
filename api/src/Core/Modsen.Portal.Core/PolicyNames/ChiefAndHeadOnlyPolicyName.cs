using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Core.PolicyNames;

public static class ChiefAndHeadOnlyPolicyName
{
    public const string Name = "ChiefAndHeadOnlyPolicy";
    public static readonly IReadOnlyCollection<string> Values =
    [
        PortalRoleNames.Ceo,
        PortalRoleNames.Cto,
        PortalRoleNames.HeadOfDepartment
    ];
}
