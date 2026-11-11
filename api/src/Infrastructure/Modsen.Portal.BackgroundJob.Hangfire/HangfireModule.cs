using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.InMemory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Modules;
using Newtonsoft.Json;

namespace Modsen.Portal.BackgroundJob.Hangfire;

public sealed class HangfireModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddHangfire((serviceProvider, hangfireConfiguration) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            hangfireConfiguration
                .UseInMemoryStorage()
                .UseConsole()
                .UseSerilogLogProvider()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSerializerSettings(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
        });

        services.AddHangfireServer(options =>
        {
            options.Queues =
            [
                "default",
                Configuration["ReportProcessingOptions:QueueName"],
                Configuration["AppSettings:PeopleForceSettings:AgreementsQueue"],
                Configuration["AppSettings:PeopleForceSettings:OneAgreementQueue"],
                Configuration["AppSettings:PeopleForceSettings:DisposeTransactionsQueue"],
                Configuration["AppSettings:PeopleForceSettings:OneDisposeTransactionsQueue"],
                Configuration["AppSettings:JiraSettings:TimesheetsImportQueue"],
                Configuration["AppSettings:JiraSettings:OneTimesheetImportQueue"],
            ];
        });

        services.AddHangfireConsoleExtensions();
        services.AddScoped<IBackgroundJobManager, BackgroundJobManager>();
    }
}
