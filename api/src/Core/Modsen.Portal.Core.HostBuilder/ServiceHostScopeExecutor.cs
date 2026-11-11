using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Modsen.Portal.Core.HostBuilder;

public static class ServiceHostScopeExecutor
{
    public static WebApplication ExecuteInScope(this WebApplication webApplication,
        Action<IServiceProvider> executeAction)
    {
        ArgumentNullException.ThrowIfNull(webApplication);
        ArgumentNullException.ThrowIfNull(executeAction);

        using var scope = webApplication.Services.CreateScope();

        executeAction.Invoke(scope.ServiceProvider);

        return webApplication;
    }
}
