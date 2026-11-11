using Microsoft.AspNetCore.Http;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Core.Authentication.SecurityHash;

internal sealed class SecurityHashValidator : ISecurityHashValidator
{
    private readonly string? _securityHashString;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHashService _hashService;
    private readonly IClockService _clockService;

    public SecurityHashValidator(
        string? securityHashString,
        IHttpContextAccessor httpContextAccessor,
        IHashService hashService,
        IClockService clockService)
    {
        ArgumentException.ThrowIfNullOrEmpty(securityHashString);
        _securityHashString = securityHashString;
        _httpContextAccessor = httpContextAccessor;
        _hashService = hashService;
        _clockService = clockService;
    }

    public bool IsValidProvidedHash()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return false;
        }

        var dateTimeUtcNow = _clockService.UtcNow;

        dateTimeUtcNow = _clockService.RoundUpToHalfAnHour(dateTimeUtcNow);

        var expectedHash = _hashService.GetSha256Hash(dateTimeUtcNow.ToString("O") + _securityHashString);

        return httpContext.Request.Headers.TryGetValue("SecurityHash", out var values) &&
            values.Count > 0 &&
            values.Contains(expectedHash);
    }
}
