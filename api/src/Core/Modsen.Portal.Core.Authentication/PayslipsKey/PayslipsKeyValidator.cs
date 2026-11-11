using Microsoft.AspNetCore.Http;

namespace Modsen.Portal.Core.Authentication.PayslipsKey;

internal sealed class PayslipsKeyValidator : IPayslipsKeyValidator
{
    private readonly string? _payslipsApiKey;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PayslipsKeyValidator(string? payslipsApiKey, IHttpContextAccessor httpContextAccessor)
    {
        ArgumentException.ThrowIfNullOrEmpty(payslipsApiKey);
        _payslipsApiKey = payslipsApiKey;
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsValidProvidedKey()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return false;
        }

        return httpContext.Request.Headers.TryGetValue("X-Payslips-Api-Key", out var values) &&
            values.Count != 0 &&
            values.Contains(_payslipsApiKey);
    }
}
