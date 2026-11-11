using Microsoft.AspNetCore.Http;

namespace Modsen.Portal.Core.Authentication.RegexApiKey;

internal sealed class RegexApiKeyValidator : IRegexApiKeyValidator
{
    private readonly string? _apiKey;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RegexApiKeyValidator(string? apiKey, IHttpContextAccessor httpContextAccessor)
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

        return httpContext.Request.Headers.TryGetValue("X-Regex-Api-Key", out var values) &&
            values.Count > 0 &&
            values.Contains(_apiKey);
    }
}
