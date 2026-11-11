using Microsoft.AspNetCore.Http;

namespace Modsen.Portal.Core.Authentication.ApiKey;

internal sealed class ApiKeyValidator : IApiKeyValidator
{
    private readonly string? _apiKey;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiKeyValidator(string? apiKey, IHttpContextAccessor httpContextAccessor)
    {
        ArgumentException.ThrowIfNullOrEmpty(apiKey);
        _apiKey = apiKey;
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsValidProvidedKey()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return false;
        }

        return httpContext.Request.Headers.TryGetValue("X-Api-Key", out var values) &&
            values.Count > 0 &&
            values.Contains(_apiKey);
    }
}
