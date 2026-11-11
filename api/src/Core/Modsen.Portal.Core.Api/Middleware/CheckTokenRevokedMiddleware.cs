using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Context;

namespace Modsen.Portal.Core.Api.Middleware;

public sealed class CheckTokenRevokedMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var authUserContext = context.RequestServices.GetRequiredService<IAuthenticatedUserContext>();
        if (authUserContext.GetBearerTokens().Count == 0 ||
            context.Request.Path.StartsWithSegments(new PathString("/api/Account/logout"),
                StringComparison.InvariantCultureIgnoreCase))
        {
            await next(context);
            return;
        }

        var authenticationTokenContext = context.RequestServices.GetRequiredService<IAuthenticationTokenContext>();

        foreach (var token in authUserContext.GetBearerTokens())
        {
            if (authUserContext.IsAuthenticated)
            {
                var reason = await authenticationTokenContext.GetRevokeReason(token, context.RequestAborted);
                if (reason == RevokeReasons.RegisteredNewToken)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";
                    var json = JsonSerializer.Serialize(new { message = PortalMessages.LoginFromAnotherIp });
                    await context.Response.WriteAsync(json, context.RequestAborted);
                    return;
                }
            }

            if (await authenticationTokenContext.IsTokenRevoked(token, context.RequestAborted))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
        }

        await next(context);
    }
}
