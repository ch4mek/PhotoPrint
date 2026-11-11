namespace Modsen.Portal.Core.Context;

public interface IAuthenticatedUserContext
{
    int? UserId { get; }
    bool IsAuthenticated { get; }
    string? DepartmentId { get; }
    int? ResourceGroupId { get; }
    bool IsManager { get; }
    string? UserEmail { get; }

    int GetRequiredUserId();
    IReadOnlyCollection<string> GetRoles();
    IReadOnlyCollection<string> GetBearerTokens();
    IReadOnlyCollection<string> GetUserClaimsNames();

    bool HasClaim(string claimName);
    bool HasClaimWithValue(string claimName, string value);
    bool HasClaimWithAnyValue(string claimName, string[] values);

    bool IsCeo();
    bool IsCto();
    bool IsHeadOfDepartment();
    bool IsHeadOfDepartmentFrom(string? departmentId);
    bool IsGroupManager();
    bool IsGroupManagerFrom(int? resourceGroupId);
    bool IsSystemAdministrator();
    bool IsEmployee();
    bool IsFromDepartment(string? departmentId);
    bool IsFromResourceGroup(int? resourceGroupId);
}
