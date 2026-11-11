using Modsen.Portal.Core.Identity;

namespace Modsen.Portal.Core.Inquiries;

public static class AcceptPolicyScopesForPosition
{
    private static readonly Dictionary<string, InquiryAcceptPolicyScopes[]> _availabilityMap =
        new()
        {
            [PortalRoleNames.Employee] = [InquiryAcceptPolicyScopes.GroupManager, InquiryAcceptPolicyScopes.ProjectManager,
                InquiryAcceptPolicyScopes.HeadOfDepartment, InquiryAcceptPolicyScopes.User],

            [PortalRoleNames.SystemAdministrator] = [InquiryAcceptPolicyScopes.GroupManager, InquiryAcceptPolicyScopes.ProjectManager,
                InquiryAcceptPolicyScopes.HeadOfDepartment, InquiryAcceptPolicyScopes.User],

            [PortalRoleNames.GroupManager] = [InquiryAcceptPolicyScopes.ProjectManager,
                InquiryAcceptPolicyScopes.HeadOfDepartment, InquiryAcceptPolicyScopes.User],

            [PortalRoleNames.HeadOfDepartment] = [InquiryAcceptPolicyScopes.CTO | InquiryAcceptPolicyScopes.CEO,
                InquiryAcceptPolicyScopes.User],

            [PortalRoleNames.Cto] = [InquiryAcceptPolicyScopes.CTO | InquiryAcceptPolicyScopes.CEO,
                InquiryAcceptPolicyScopes.User],

            [PortalRoleNames.Ceo] = [InquiryAcceptPolicyScopes.User]
        };

    public static InquiryAcceptPolicyScopes[] GetAvailable(string? position)
    {
        ArgumentNullException.ThrowIfNull(position);

        if (_availabilityMap.TryGetValue(position, out var availableScopes))
        {
            return availableScopes;
        }

        return [];
    }
}
