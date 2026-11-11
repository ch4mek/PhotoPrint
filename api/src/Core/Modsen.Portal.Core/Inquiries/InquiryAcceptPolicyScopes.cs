namespace Modsen.Portal.Core.Inquiries;

[Flags]
public enum InquiryAcceptPolicyScopes
{
    CEO = 1,
    CTO = 2,
    HeadOfDepartment = 4,
    GroupManager = 8,
    ProjectManager = 16,
    User = 32,
}
