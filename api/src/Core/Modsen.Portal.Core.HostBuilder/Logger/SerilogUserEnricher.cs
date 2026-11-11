using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Context;
using Serilog.Core;
using Serilog.Events;

namespace Modsen.Portal.Core.HostBuilder.Logger;

public sealed class SerilogUserEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string UserIdPropertyName = "XUserId";
    private const string UserNamePropertyName = "XUserName";

    public SerilogUserEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent? logEvent, ILogEventPropertyFactory? propertyFactory)
    {
        if (logEvent == null || propertyFactory == null)
        {
            return;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.RequestServices != null)
        {
            var context = httpContext.RequestServices.GetRequiredService<IAuthenticatedUserContext>();
            if (context.IsAuthenticated)
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(UserIdPropertyName, context.UserId));
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(UserNamePropertyName, context.UserEmail));
            }
        }
    }
}
