using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Context;

namespace Modsen.Portal.Core.Api.Middleware;

public class CorrelationIdMiddleware : IMiddleware
{
    private const string s_correlationId = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var correlationContext = context.RequestServices.GetRequiredService<ICorrelationContext>();
        Guid correlationId;

        if (context.Request.Headers.TryGetValue(s_correlationId, out var header))
        {
            if (Guid.TryParse(header, out var id))
            {
                correlationId = id;
                correlationContext.UseCorrelationId(correlationId);
            }
            else
            {
                correlationId = correlationContext.CorrelationId;
            }
        }
        else
        {
            correlationId = correlationContext.CorrelationId;
        }

        context.Response.Headers[s_correlationId] = correlationId.ToString(PortalSharedConstants.DefaultGuidFormat);

        await next(context);
    }
}
