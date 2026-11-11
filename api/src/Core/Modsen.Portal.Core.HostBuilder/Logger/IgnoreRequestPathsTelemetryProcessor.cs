using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Modsen.Portal.Core.HostBuilder.Logger;

internal sealed class IgnoreRequestPathsTelemetryProcessor : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;

    public IgnoreRequestPathsTelemetryProcessor(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        if (item is RequestTelemetry request &&
            (request.Url.AbsolutePath.StartsWith("/health/ready", StringComparison.InvariantCulture) ||
                request.Url.AbsolutePath.StartsWith("/health/live", StringComparison.InvariantCulture) ||
                request.Url.AbsolutePath.StartsWith("/swagger/", StringComparison.InvariantCulture)))
        {
            return;
        }

        _next.Process(item);
    }
}
