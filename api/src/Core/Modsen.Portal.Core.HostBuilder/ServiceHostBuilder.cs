using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using Modsen.Portal.Core.Api.Cors;
using Modsen.Portal.Core.Api.HealthChecks;
using Modsen.Portal.Core.Api.Middleware;
using Modsen.Portal.Core.HostBuilder.HealthChecks;
using Modsen.Portal.Core.HostBuilder.HostModules;
using Modsen.Portal.Core.HostBuilder.Logger;
using Modsen.Portal.Core.UseCases;
using Modsen.Portal.Core.Utils.Modules;
using Scalar.AspNetCore;

namespace Modsen.Portal.Core.HostBuilder;

public sealed class ServiceHostBuilder
{
    private readonly string _version;
    private Type? _errorHandlerMiddlewareType;
    private Action<WebApplication>? _config;
    private readonly List<IRegisterPortalHealthCheck> _registerPortalHealthChecks = [];

    private ServiceHostBuilder(WebApplicationBuilder webHostBuilder, string serviceName, string version)
    {
        _version = version;
        WebHostBuilder = webHostBuilder;
        ServiceName = serviceName;
        RootModule = new RegistrationModuleAggregate(WebHostBuilder.Configuration);
    }

    private WebApplicationBuilder WebHostBuilder { get; }
    private RegistrationModuleAggregate RootModule { get; }
    private string ServiceName { get; }
    private IWebHostEnvironment WebHostEnvironment => WebHostBuilder.Environment;

    public static ServiceHostBuilder Create(string[] args, string serviceName, string version, string envVariablesPrefix)
    {
        return Create(args, serviceName, version, envVariablesPrefix, new Dictionary<string, string?>());
    }

    public static ServiceHostBuilder Create(string[] args, string serviceName, string version, string envVariablesPrefix,
        Dictionary<string, string?> inMemoryConfiguration, bool enableLogging = true)
    {
        ArgumentException.ThrowIfNullOrEmpty(serviceName);
        ArgumentNullException.ThrowIfNull(inMemoryConfiguration);

        var hostBuilder = new ServiceHostBuilder(WebApplication.CreateBuilder(args), serviceName, version);

        hostBuilder.WebHostBuilder.WebHost.ConfigureKestrel(options =>
        {
            options.AddServerHeader = false;
        });

        hostBuilder.WebHostBuilder.Host.UseDefaultServiceProvider(options =>
        {
            options.ValidateOnBuild = true;
            options.ValidateScopes = true;
        });

        hostBuilder.WebHostBuilder.Configuration.AddEnvironmentVariables(envVariablesPrefix);
        hostBuilder.WebHostBuilder.Configuration.AddInMemoryCollection(inMemoryConfiguration);
        hostBuilder.WebHostBuilder.Services.AddFeatureManagement();

        if (enableLogging)
        {
            hostBuilder.WebHostBuilder.ConfigureApplicationLogger();
        }

        return hostBuilder
            .WithModule<ConnectionModule>()
            .WithModule<ContextModule>()
            .WithModule<ControllersModule>()
            .WithModule<CorsModule>()
            .WithModule<MiddlewareModule>()
            .WithModule<RateLimiterModule>()
            .WithModule(new ApplicationCacheModule(serviceName))
            .WithModule(new OpenApiModule(serviceName))
            .WithHealthCheck(new RegisterPostgreSqlHealthCheck(serviceName))
            .WithHealthCheck(new RegisterRedisHealthCheck(serviceName, hostBuilder.WebHostBuilder.Configuration));
    }

    private ServiceHostBuilder WithModule<TModule>() where TModule : RegistrationModule, new()
    {
        RootModule.AddModule<TModule>();
        return this;
    }

    public ServiceHostBuilder WithModule<TModule>(Func<IWebHostEnvironment, RegistrationModuleAggregate, TModule> factory)
        where TModule : RegistrationModule
    {
        ArgumentNullException.ThrowIfNull(factory);
        RootModule.AddModule(factory(WebHostEnvironment, RootModule));
        return this;
    }

    private ServiceHostBuilder WithModule(RegistrationModule module)
    {
        RootModule.AddModule(module);
        return this;
    }

    private ServiceHostBuilder WithHealthCheck(IRegisterPortalHealthCheck registerPortalHealthCheck)
    {
        _registerPortalHealthChecks.Add(registerPortalHealthCheck);
        return this;
    }

    public ServiceHostBuilder WithSignalR()
    {
        return WithModule<SignalRModule>();
    }

    public ServiceHostBuilder WithErrorHandlerMiddleware<TMiddleware>() where TMiddleware : IMiddleware
    {
        var type = typeof(TMiddleware);

        WebHostBuilder.Services.AddSingleton(type);
        _errorHandlerMiddlewareType = type;

        return this;
    }

    public ServiceHostBuilder ConfigureMiddleware(Action<WebApplication> config)
    {
        _config = config;
        return this;
    }

    public WebApplication Build()
    {
        WithModule(new FrameworkModule(RootModule));
        RegisterHealthChecks();

        RootModule.LoadRoot(WebHostBuilder.Services);

        var app = WebHostBuilder.Build();

        app = UseApiMiddleware(app);

        _config?.Invoke(app);

        return app;
    }

    private void RegisterHealthChecks()
    {
        var healthChecksBuilder = WebHostBuilder.Services
            .AddHealthChecks()
            .AddCheck(
                name: $"{ServiceName} Self Check",
                check: () => HealthCheckResult.Healthy(),
                tags: [PortalHealthCheckTagNames.Live]);

        foreach (var registerPortalHealthCheck in _registerPortalHealthChecks)
        {
            registerPortalHealthCheck.Register(healthChecksBuilder);
        }
    }

    private WebApplication UseApiMiddleware(WebApplication app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
        app.UseHttpsRedirection();
        app.Use(async (context, next) => {
            if (context.Request.IsHttps)
            {
                context.Response.Headers.StrictTransportSecurity = "max-age=31536000; includeSubDomains; preload";
            }

            context.Response.Headers.XContentTypeOptions = "nosniff";
            context.Response.Headers.XFrameOptions= "SAMEORIGIN";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            context.Response.Headers.ContentSecurityPolicy =
                context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase) ||
                context.Request.Path.StartsWithSegments("/hangfire-dashboard", StringComparison.OrdinalIgnoreCase)
                    ? "default-src 'self'; img-src 'self' data:; style-src 'self' 'unsafe-inline'; script-src 'self' 'unsafe-inline'; connect-src 'self' https:; font-src 'self' data:;"
                    : "default-src 'none'; object-src 'none'; connect-src 'self' wss:; frame-ancestors 'none'; base-uri 'none'; form-action 'none';";

            await next();
        });

        app.UseMiddleware<CorrelationIdMiddleware>();

        if (_errorHandlerMiddlewareType != null)
        {
            app.UseMiddleware(_errorHandlerMiddlewareType);
        }

        app.UseODataQueryRequest();
        app.UseRouting();

        app.UseOpenApi();

        if (!app.Environment.IsProduction())
        {
            app.MapScalarApiReference("/swagger", opt =>
            {
                opt.WithTitle($"Modsen Management System API {_version}")
                    .WithDefaultHttpClient(ScalarTarget.Http, ScalarClient.Http11)
                    .WithOpenApiRoutePattern("/swagger/v1/swagger.json");
            });
        }

        app.UseApiCorsPolicy(PortalCorsPolicyNames.Default);

        app.UseAuthentication();

        app.UseMiddleware<CheckTokenRevokedMiddleware>();
        app.UseAuthorization();

        app.MapControllers().Add(endpointBuilder =>
        {
            var controllerDescriptor = endpointBuilder.Metadata
                .OfType<ControllerActionDescriptor>()
                .FirstOrDefault();

            var controllerName = controllerDescriptor?.ControllerTypeInfo.Name;

            if (controllerName != "RecruitmentServiceController")
            {
                endpointBuilder.Metadata.Add(new EnableRateLimitingAttribute("GlobalLimiter"));
            }
        });

        app.MapGet("/", () => Results.Ok($"Modsen Management System API {_version}: {ServiceName}"));
        app.UseApiHealthChecks();

        if (!app.Environment.IsEnvironment(PortalEnvironmentNames.IntegrationTest) &&
            !app.Environment.IsEnvironment(PortalEnvironmentNames.FunctionalTest))
        {
            app.UseRateLimiter();
        }

        return app;
    }
}
