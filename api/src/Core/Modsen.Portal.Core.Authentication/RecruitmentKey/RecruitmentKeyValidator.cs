using Microsoft.AspNetCore.Http;

namespace Modsen.Portal.Core.Authentication.RecruitmentKey;

internal sealed class RecruitmentKeyValidator : IRecruitmentKeyValidator
{
    private readonly string? _recruitmentKey;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RecruitmentKeyValidator(string? recruitmentKey, IHttpContextAccessor httpContextAccessor)
    {
        ArgumentException.ThrowIfNullOrEmpty(recruitmentKey);
        _recruitmentKey = recruitmentKey;
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsValidProvidedKey()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return false;
        }

        return httpContext.Request.Headers.TryGetValue("X-RECRUITMENT-SERVICE-KEY", out var values) &&
            values.Count > 0 &&
            values.Contains(_recruitmentKey);
    }
}
