using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;

namespace Modsen.Portal.Core.Api.Context;

public class AuthenticatedUserContext : IAuthenticatedUserContext
{
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private IReadOnlyCollection<Claim>? _claims;

    public AuthenticatedUserContext() { }

    public AuthenticatedUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var id = GetClaims().FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(id, out var parsedId)
                ? parsedId
                : null;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor?.HttpContext?.User.Identity?.IsAuthenticated == true;

    public string? DepartmentId => GetClaims().FirstOrDefault(x => x.Type == PortalClaimTypeNames.DepartmentId)?.Value;

    public int? ResourceGroupId
    {
        get
        {
            var resourceGroupId = GetClaims().FirstOrDefault(x => x.Type == PortalClaimTypeNames.ResourceGroupId)?.Value;
            return int.TryParse(resourceGroupId, out var id)
                ? id
                : null;
        }
    }

    public bool IsManager => HasClaim(PortalClaimTypeNames.IsManager);

    public string? UserEmail => GetClaims().FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

    public IReadOnlyCollection<string> GetRoles()
    {
        return GetClaims().Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToArray();
    }

    public IReadOnlyCollection<string> GetBearerTokens()
    {
        var httpContext = _httpContextAccessor?.HttpContext;
        if (httpContext == null || !IsAuthenticated)
        {
            return [];
        }

        return httpContext.Request.Headers.ContainsKey(HeaderNames.Authorization)
            ? httpContext.Request.Headers.Authorization
                .Where(x => !x.IsNullOrEmpty())
                .Select(x => x!.Replace("Bearer", string.Empty, StringComparison.InvariantCulture).Trim())
                .ToArray()
            : [];
    }

    public int GetRequiredUserId()
    {
        return UserId ?? throw new ActionNotAllowedException("UserId is required");
    }

    public IReadOnlyCollection<string> GetUserClaimsNames()
    {
        return GetClaims().Select(claim => claim.Type).ToArray();
    }

    private IEnumerable<Claim> GetClaims()
    {
        return _claims ??= _httpContextAccessor?.HttpContext?.User.Claims.ToArray() ?? [];
    }

    public bool HasClaim(string claimName)
    {
        return GetClaims().Any(x => x.Type == claimName);
    }

    public bool HasClaimWithValue(string claimName, string value)
    {
        return GetClaims().Any(x => x.Type == claimName && x.Value == value);
    }

    public bool HasClaimWithAnyValue(string claimName, string[] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return values.Length != 0 && GetClaims().Any(x => x.Type == claimName && values.Contains(x.Value));
    }

    public bool IsCeo()
    {
        return GetRoles().IsCeo();
    }

    public bool IsCto()
    {
        return GetRoles().IsCto();
    }

    public bool IsHeadOfDepartment()
    {
        return GetRoles().IsHeadOfDepartment();
    }

    public bool IsHeadOfDepartmentFrom(string? departmentId)
    {
        return GetRoles().IsHeadOfDepartment() && IsFromDepartment(departmentId);
    }

    public bool IsGroupManager()
    {
        return GetRoles().IsGroupManager();
    }

    public bool IsGroupManagerFrom(int? resourceGroupId)
    {
        return GetRoles().IsGroupManager() && IsFromResourceGroup(resourceGroupId);
    }

    public bool IsSystemAdministrator()
    {
        return GetRoles().IsSystemAdministrator();
    }

    public bool IsEmployee()
    {
        return GetRoles().IsEmployee();
    }

    public bool IsFromDepartment(string? departmentId)
    {
        var currentUserDepartmentId = DepartmentId;
        return !string.IsNullOrEmpty(currentUserDepartmentId) && currentUserDepartmentId == departmentId;
    }

    public bool IsFromResourceGroup(int? resourceGroupId)
    {
        var currentUserResourceGroupId = ResourceGroupId;
        return currentUserResourceGroupId.HasValue && currentUserResourceGroupId == resourceGroupId;
    }
}
