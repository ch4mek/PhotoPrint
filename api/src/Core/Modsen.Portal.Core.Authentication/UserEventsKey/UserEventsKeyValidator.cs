using Microsoft.AspNetCore.Http;

namespace Modsen.Portal.Core.Authentication.UserEventsKey;

internal sealed class UserEventsKeyValidator : IUserEventsKeyValidator
{
    private readonly string? _userEventsApiKey;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserEventsKeyValidator(string? userEventsApiKey, IHttpContextAccessor httpContextAccessor)
    {
        ArgumentException.ThrowIfNullOrEmpty(userEventsApiKey);
        _userEventsApiKey = userEventsApiKey;
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsValidProvidedKey()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return false;
        }

        return httpContext.Request.Headers.TryGetValue("X-User-Events-Api-Key", out var values) &&
            values.Count != 0 &&
            values.Contains(_userEventsApiKey);
    }
}
