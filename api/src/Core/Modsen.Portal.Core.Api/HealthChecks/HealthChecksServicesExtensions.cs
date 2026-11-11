using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Modsen.Portal.Core.Api.HealthChecks;

public static class HealthChecksServicesExtensions
{
    private static readonly Dictionary<HealthStatus, int> _statusCodesMap = new()
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    };

    public static void UseApiHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = hc => hc.Tags.Contains(PortalHealthCheckTagNames.Ready),
            AllowCachingResponses = false,
            ResultStatusCodes = _statusCodesMap
        });

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = hc => hc.Tags.Contains(PortalHealthCheckTagNames.Live),
            AllowCachingResponses = false,
            ResultStatusCodes = _statusCodesMap
        });
    }
}
