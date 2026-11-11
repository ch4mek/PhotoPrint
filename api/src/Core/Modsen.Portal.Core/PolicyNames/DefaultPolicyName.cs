using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Core.PolicyNames;

public static class DefaultPolicyName
{
    public const string Name = "DefaultPolicy";
    public static readonly IReadOnlyCollection<string> Values =
    [
        PortalRoleNames.Ceo,
        PortalRoleNames.Cto,
        PortalRoleNames.HeadOfDepartment,
        PortalRoleNames.GroupManager,
        PortalRoleNames.Employee,
        PortalRoleNames.SystemAdministrator,
    ];
}
