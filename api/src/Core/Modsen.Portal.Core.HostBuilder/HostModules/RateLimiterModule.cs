using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Core.HostBuilder.HostModules;

internal sealed class RateLimiterModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("GlobalLimiter",
                context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.User.Claims
                            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ??
                        context.Request.Headers.Host.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 240,
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(1)
                        }));

            options.AddFixedWindowLimiter("RecruitmentLimiter", limiterOptions =>
            {
                limiterOptions.PermitLimit = 300;
                limiterOptions.Window = TimeSpan.FromSeconds(1);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0;
            });

            options.AddFixedWindowLimiter("PayslipsLimiter", limiterOptions =>
            {
                limiterOptions.PermitLimit = 1000;
                limiterOptions.Window = TimeSpan.FromMinutes(5);
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiterOptions.QueueLimit = 0;
            });
        });
    }
}
