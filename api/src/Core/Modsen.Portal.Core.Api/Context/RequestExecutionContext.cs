using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Modsen.Portal.Core.Context;

namespace Modsen.Portal.Core.Api.Context;

public sealed class RequestExecutionContext(IHttpContextAccessor httpContextAccessor) : IRequestExecutionContext
{
    public IPAddress? RemoteIpAddress()
    {
        return httpContextAccessor.HttpContext?.Connection.RemoteIpAddress;
    }

    public string? GetRealIpHeader()
    {
        return httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("x-real-ip", out var values) == true
            ? values.ToString()
            : null;
    }

    public string? GetForwardedForHeader()
    {
        return httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("x-forwarded-for", out var values) == true
            ? values.ToString()
            : null;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(string scheme)
    {
        var context = httpContextAccessor.HttpContext;
        if (context != null)
        {
            var authResult = await context.AuthenticateAsync(scheme);
            var authenticateResult = new AuthenticationResult
            {
                ReturnUrl = authResult.Properties?.Items["returnUrl"],
                GivenName = authResult.Principal?.FindFirstValue(ClaimTypes.GivenName),
                Surname = authResult.Principal?.FindFirstValue(ClaimTypes.Surname),
                Email = authResult.Principal?.FindFirstValue(ClaimTypes.Email)
                    ?? authResult.Principal?.FindFirstValue(ClaimTypes.Name),
                Succeeded = authResult.Succeeded
            };

            return authenticateResult;
        }

        return new AuthenticationResult { Succeeded = false };
    }
}
