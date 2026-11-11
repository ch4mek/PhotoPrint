using System.Globalization;
using Hangfire.Console.Extensions.Serilog;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modsen.Portal.Core.Utils.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Modsen.Portal.Core.HostBuilder.Logger;

internal static class SerilogServicesExtensions
{
    private const string s_defaultOutputTemplate = "[{Timestamp} | {Level}] {Message:lj} {NewLine}{Exception}{Properties}{NewLine}";

    internal static WebApplicationBuilder ConfigureApplicationLogger(this WebApplicationBuilder webAppBuilder)
    {
        var connectionString = webAppBuilder.Configuration.GetValue<string>("ApplicationInsights:ConnectionString");
        var environment = webAppBuilder.Environment;

        webAppBuilder.Services.AddSingleton<SerilogCorrelationIdEnricher>();
        webAppBuilder.Services.AddSingleton<SerilogUserEnricher>();

        webAppBuilder.Services
            .AddMetrics()
            .AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = connectionString;
                options.EnableAdaptiveSampling = false;
                options.EnablePerformanceCounterCollectionModule = true;
            })
            .AddApplicationInsightsTelemetryProcessor<IgnoreRequestPathsTelemetryProcessor>();

        webAppBuilder.Services
            .ConfigureTelemetryModule<EventCounterCollectionModule>(
                (module, _) =>
                {
                    module.Counters.Add(
                        new EventCounterCollectionRequest("System.Runtime", "working-set"));
                })
            .ConfigureTelemetryModule<PerformanceCollectorModule>(
                (module, _) =>
                {
                    module.Counters.Add(
                        new PerformanceCounterCollectionRequest(
                            "Memory\\Available Bytes", "AvailableMemoryBytes"));
                    module.Counters.Add(
                        new PerformanceCounterCollectionRequest(
                            "Memory\\Committed Bytes", "CommittedMemoryBytes"));
                });

        webAppBuilder.Host.UseSerilog((_, serviceProvider, config) =>
        {
            config = config
                .Enrich.WithHangfireContext()
                .Enrich.WithProperty(nameof(environment.EnvironmentName), environment.EnvironmentName)
                .Enrich.FromLogContext();

            var correlationIdEnricher = serviceProvider.GetService<SerilogCorrelationIdEnricher>();
            if (correlationIdEnricher != null)
            {
                config = config.Enrich.With(correlationIdEnricher);
            }

            var userEnricher = serviceProvider.GetService<SerilogUserEnricher>();
            if (userEnricher != null)
            {
                config = config.Enrich.With(userEnricher);
            }

            config = config.Filter.ByExcluding(logEvent =>
            {
                if (!logEvent.Properties.TryGetValue("Path", out var property))
                {
                    return false;
                }

                var path = property.ToString();

                return path.Contains("/swagger", StringComparison.InvariantCulture) || path.Contains("/health", StringComparison.InvariantCulture);
            });

            var innerConfig = new LoggerConfiguration()
                .MinimumLevel.Verbose();

            innerConfig = innerConfig.WriteTo.Console(
                restrictedToMinimumLevel: LogEventLevel.Information,
                outputTemplate: s_defaultOutputTemplate,
                theme: AnsiConsoleTheme.Code,
                formatProvider: CultureInfo.InvariantCulture);

            innerConfig = innerConfig.WriteTo.Hangfire(
                restrictedToMinimumLevel: LogEventLevel.Information,
                formatProvider: CultureInfo.InvariantCulture);

            if (!connectionString.IsNullOrWhiteSpace())
            {
                var telemetryConfiguration = serviceProvider.GetRequiredService<TelemetryConfiguration>();
                innerConfig = innerConfig.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Events);
            }

            var innerLogger = innerConfig.CreateLogger();

            config.WriteTo.Sink(new LevelDowngradingSink(innerLogger));
        });

        return webAppBuilder;
    }
}
