using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Modsen.Portal.Core.Api.Cors;

public static class CorsServicesExtensions
{
    public static IServiceCollection AddCorsPolicies(this IServiceCollection services, CorsSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        if (!settings.UseCors)
        {
            return services;
        }

        ArgumentNullException.ThrowIfNull(settings.Policies);
        services.AddCors(options =>
        {
            foreach (var policy in settings.Policies)
            {
                ArgumentNullException.ThrowIfNull(policy.PolicyName);
                options.AddPolicy(
                    policy.PolicyName,
                    builder => ConfigurePolicy(builder, policy));
            }
        });

        return services;
    }

    public static IApplicationBuilder UseApiCorsPolicy(this IApplicationBuilder app, string policyName)
    {
        ArgumentNullException.ThrowIfNull(policyName);

        app.UseCors(policyName);

        return app;
    }

    private static void ConfigurePolicy(CorsPolicyBuilder builder, CorsPolicySettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings.PolicyName);

        if (settings.AllowAnyHeader)
        {
            builder.AllowAnyHeader();
        }
        else
        {
            builder.WithHeaders(settings.AllowedHeaders.ToArray());
        }

        if (settings.AllowAnyMethod)
        {
            builder.AllowAnyMethod();
        }
        else
        {
            builder.WithMethods(settings.AllowedMethods.ToArray());
        }

        builder.WithExposedHeaders("Content-Disposition");
        builder.AllowAnyOrigin();
    }
}
