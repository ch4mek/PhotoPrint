using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Context;
using Serilog.Core;
using Serilog.Events;

namespace Modsen.Portal.Core.HostBuilder.Logger;

internal sealed class SerilogCorrelationIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CorrelationIdPropertyName = "XCorrelationId";

    public SerilogCorrelationIdEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.RequestServices != null)
        {
            var executionContext = httpContext.RequestServices.GetRequiredService<ICorrelationContext>();
            if (!executionContext.IsCorrelationIdEmpty)
            {
                var property = propertyFactory.CreateProperty(CorrelationIdPropertyName, executionContext.CorrelationId);
                logEvent.AddOrUpdateProperty(property);
            }
        }
    }
}
