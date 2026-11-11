using System.Globalization;
using System.Security.Claims;
using Hangfire.Dashboard;
using Modsen.Portal.Core.Authentication.Jwt;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract;

namespace Modsen.Portal.Api.Hangfire;

internal sealed class HangfireDashboardJwtAuthorizationFilter : IDashboardAsyncAuthorizationFilter
{
    public async Task<bool> AuthorizeAsync(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        var tokenService = httpContext.RequestServices.GetRequiredService<ITokenService>();
        var authenticationTokenContext = httpContext.RequestServices.GetRequiredService<IAuthenticationTokenContext>();
        var identityContract = httpContext.RequestServices.GetRequiredService<IIdentityContract>();
        var authenticationOptions = httpContext.RequestServices.GetRequiredService<JwtAuthenticationOptions>();

        string? jwtToken;

        if (httpContext.Request.Query.ContainsKey("access_token"))
        {
            jwtToken = httpContext.Request.Query["access_token"][0] ?? string.Empty;
            httpContext.Response.Cookies.Append(
                "HangfireAccessToken",
                jwtToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddMinutes(authenticationOptions.TokenLifetimeInMinutes),
                    SameSite = SameSiteMode.Strict,
                    Secure = true,
                    HttpOnly = true
                });
        }
        else
        {
            jwtToken = httpContext.Request.Cookies["HangfireAccessToken"];
        }

        if (jwtToken.IsNullOrWhiteSpace())
        {
            return false;
        }

        var (isTokenValid, claims) = tokenService.GetClaimsFromToken(jwtToken);

        if (!isTokenValid)
        {
            return false;
        }

        var isTokenRevoked = await authenticationTokenContext.IsTokenRevoked(jwtToken, httpContext.RequestAborted);

        if (isTokenRevoked)
        {
            return false;
        }

        var userClaims = await identityContract.GetUserClaimsByGroup(
            HangfireClaimTypeNames.GroupId,
            int.Parse(claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture),
            httpContext.RequestAborted);

        return userClaims.Any(claim => claim.ClaimType == HangfireClaimTypeNames.HangfireDashboardAccess.Name);
    }
}
