using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Core.PolicyNames;

public static class ChiefOnlyPolicyName
{
    public const string Name = "ChiefOnlyPolicy";
    public static readonly IReadOnlyCollection<string> Values =
    [
        PortalRoleNames.Ceo,
        PortalRoleNames.Cto
    ];
}
