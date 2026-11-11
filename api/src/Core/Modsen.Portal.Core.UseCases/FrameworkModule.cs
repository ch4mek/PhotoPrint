using System.Reflection;
using FluentValidation;
using MassTransit;
using MediatR.NotificationPublishers;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Database.Interceptors;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.Core.UseCases.Pipeline;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.Core.UseCases;

public sealed class FrameworkModule(RegistrationModuleAggregate rootModule) : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(Configuration);

        services.AddScoped<ISaveChangesInterceptor, DatabaseAuditInterceptor>();
        services.AddScoped<IDateCalculator, DateCalculator>();

        services.AddSingleton<IClockService, ClockService>();
        services.AddSingleton<IHashService, HashService>();
        services.AddSingleton<IHexColorGenerator, HexColorGenerator>();
        services.AddSingleton<IUrlEncoder, UrlEncoder>();
        services.AddSingleton<IGuidGenerator, GuidGenerator>();
        services.AddTransient<ICommunicationBus, CommunicationBus>();
        services.AddScoped<IDataProtectionFactory, DataProtectionFactory>();
        services.AddScoped<IFileProtectionFactory, FileProtectionFactory>();

        services.AddHttpContextAccessor();
        services.AddHttpClient();

        var useCasesAssemblies = Assembly.GetExecutingAssembly()
            .GetReferencedAssemblies("Modsen.Portal*UseCases.dll")
            .ToArray();
        var requestsAssemblies = Assembly.GetExecutingAssembly()
            .GetReferencedAssemblies("Modsen.Portal*Requests.dll")
            .ToArray();
        var assemblies = useCasesAssemblies.Concat(requestsAssemblies).ToArray();

        AddMediatrServices(services, assemblies);
        AddFluentValidationServices(services, assemblies);
        AddMassTransitServices(services, Configuration);
    }

    private static void AddMediatrServices(IServiceCollection services, Assembly[] assemblies)
    {
        services.AddMediatR(config =>
        {
            config
                .RegisterServicesFromAssemblies(assemblies)
                .AddOpenBehavior(typeof(FluentValidationPipe<,>))
                .AddOpenBehavior(typeof(DbTransactionPipe<,>));

            config.NotificationPublisher = new ForeachAwaitPublisher();
            config.NotificationPublisherType = typeof(ForeachAwaitPublisher);
        });
    }

    private static void AddFluentValidationServices(
        IServiceCollection services,
        Assembly[] assemblies)
    {
        services.AddValidatorsFromAssemblies(
            assemblies,
            includeInternalTypes: true);
    }

    private void AddMassTransitServices(IServiceCollection services, IConfiguration configuration)
    {
        var transportType = configuration.GetValue<string>("Messaging:TransportType");

        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter();
            busConfig.SetInMemorySagaRepositoryProvider();

            var exchangeDefinitions = rootModule.Modules.SelectMany(x => x.GetExchangeDefinitions()).ToArray();

            if (transportType == "RabbitMQ")
            {
                foreach (var registration in rootModule.Modules.SelectMany(x => x.GetConsumerRegistrationConfigurations()))
                {
                    registration.Register(busConfig);
                }

                var rabbitMqConfig = configuration.GetSection("Messaging:RabbitMq");
                var username = rabbitMqConfig["Username"];
                var password = rabbitMqConfig["Password"];

                ArgumentException.ThrowIfNullOrEmpty(username);
                ArgumentException.ThrowIfNullOrEmpty(password);

                busConfig.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqConfig["Host"], rabbitMqConfig.GetValue<ushort>("Port", 5672), rabbitMqConfig["VirtualHost"] ?? "/", h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    foreach (var exchangeDefinition in exchangeDefinitions)
                    {
                        exchangeDefinition.Configure(cfg);
                    }

                    cfg.ConfigureEndpoints(context);
                });
            }
            else
            {
                var consumerTypes = rootModule.Modules
                    .SelectMany(x => x.GetConsumerRegistrationConfigurations())
                    .Select(registration => registration.GetConsumerType())
                    .ToArray();

                busConfig.AddConsumers(consumerTypes);
                busConfig.UsingInMemory((context, config) =>
                {
                    config.ConfigureEndpoints(context);
                });
            }
        });
    }
}
