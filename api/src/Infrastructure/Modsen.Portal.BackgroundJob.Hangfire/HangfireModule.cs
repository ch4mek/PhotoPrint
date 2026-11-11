using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Modules;
using Newtonsoft.Json;
using Npgsql;

namespace Modsen.Portal.BackgroundJob.Hangfire;

public sealed class HangfireModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        services.AddHangfire((serviceProvider, hangfireConfiguration) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var npgsqlDataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();

            hangfireConfiguration
                .UsePostgreSqlStorage(options =>
                {
                    if (configuration.GetValue<bool>("AWS:UseConnectionStringForHangfire"))
                    {
                        options.UseNpgsqlConnection(configuration.GetConnectionString("HangfireStorage"));
                    }
                    else
                    {
                        options.UseConnectionFactory(new HangfireConnectionFactory(npgsqlDataSource));
                    }
                }, new PostgreSqlStorageOptions
                {
                    PrepareSchemaIfNecessary = true,
                    SchemaName = "hangfire",
                    QueuePollInterval = TimeSpan.FromSeconds(5)
                })
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
                Configuration["BookingSettings:QueueName"],
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
