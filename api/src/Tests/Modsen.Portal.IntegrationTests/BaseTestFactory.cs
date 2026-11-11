using System.Text.Json;
using ClosedXML.Excel;
using ClosedXML.Graphics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Api;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Core.Authentication.ApiKey;
using Modsen.Portal.Core.Authentication.External;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.HostBuilder.Jobs;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.IntegrationTests.Mocks;
using Modsen.Portal.IntegrationTests.Seeds;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Testcontainers.PostgreSql;

namespace Modsen.Portal.IntegrationTests;

public abstract class BaseTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer? _container;
    private bool _containerDisposed = false;
    private readonly bool _testViaDockerTestContainers = true;

    protected BaseTestFactory()
    {
        if (bool.TryParse(Environment.GetEnvironmentVariable("TestViaDockerTestContainers"),
                out var testViaDockerTestContainers))
        {
            _testViaDockerTestContainers = testViaDockerTestContainers;
        }

        if (_testViaDockerTestContainers)
        {
            _container = InfrastructureForTesting.CreateDbContainer();
        }
    }

    protected abstract string GetEnvironmentName();

    public JsonSerializerOptions JsonOptions => IntegrationTestsConstants.DefaultSerializerOptions;

    public bool IsCertificateTestsEnabled()
    {
        return DateTime.Now > new DateTime(2026, 03, 30);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(GetEnvironmentName());
        builder.ConfigureAppConfiguration(options =>
        {
            var connectionString = _testViaDockerTestContainers
                ? _container!.GetConnectionString()
                : $"{Environment.GetEnvironmentVariable("API_IntergrationTest_Db")};Database=TestDb_{Guid.CreateVersion7():N}";

            ArgumentException.ThrowIfNullOrEmpty(connectionString);

            if (_testViaDockerTestContainers)
            {
                connectionString = _container!.GetConnectionString();
            }

            var appSettings = new Dictionary<string, string>
            {
                { "ConnectionStrings:TrackDb", connectionString },
                { "ConnectionStrings:HangfireStorage", connectionString },
                { "CacheStore", "InMemory" },
                { "S3:DefaultVirtualDirectory", "application-files-tests" }
            };

            options.AddInMemoryCollection(appSettings!);
        });
        builder.ConfigureTestServices(services =>
        {
            // services.RemoveAll(typeof(IHostedService));
            // services.RemoveAll(typeof(BackgroundService));
            services.RemoveAll(typeof(ICommunicationBus));
            services.AddTransient<ICommunicationBus, OutboxEventPublisherForTests>();

            services.RemoveAll(typeof(IRequestExecutionContext));
            services.AddSingleton<IRequestExecutionContext, RequestExecutionContextMock>();

            services.RemoveAll(typeof(IExternalAuthenticationService));
            services.AddSingleton<IExternalAuthenticationService, MicrosoftAuthenticationServiceMock>();

            services.RemoveAll(typeof(IDistributedCache));
            services.AddDistributedMemoryCache();

            services.RemoveAll(typeof(IApiKeyValidator));
            services.AddSingleton<IApiKeyValidator, ApiKeyValidatorMock>();

            services.RemoveAll(typeof(IRecurringJobScheduler));

            services.RemoveAll(typeof(IBackgroundJobManager));
            services.AddScoped<IBackgroundJobManager, BackgroundJobManagerMock>();

            services.AddSingleton(serviceProvider => (RequestExecutionContextMock)serviceProvider.GetRequiredService<IRequestExecutionContext>());
            services.AddControllers().AddApplicationPart(typeof(TestAccountController).Assembly);

            services.AddTransient<ISeedEntitiesProvider<IdentityModuleDbContext>, IntegrationTestUserSeeds>();
            services.AddTransient<ISeedEntitiesProvider<BookingDbContext>, IntegrationTestWorkplacesSeed>();
            services.AddTransient<ISeedEntitiesProvider<IdentityModuleDbContext>, IntegrationTestResourceGroupsSeed>();
            services.AddTransient<ISeedEntitiesProvider<TeamBoardDbContext>, IntegrationTestUserSpecializationSeed>();
            services.AddTransient<ISeedEntitiesProvider<MerchDbContext>, IntegrationTestMerchSeed>();
            services.AddTransient<ISeedEntitiesProvider<AttendanceDbContext>, IntegrationTestAttendanceSeed>();
            services.AddTransient<ISeedEntitiesProvider<FinanceDbContext>, IntegrationTestExchangeRatesSeed>();

            LoadOptions.DefaultGraphicEngine = new DefaultGraphicEngine("Fallback font name");
        });
        builder.ConfigureLogging(options => options.SetMinimumLevel(LogLevel.Warning));
    }

    public async Task InitializeAsync()
    {
        if (_container != null)
        {
            await _container.StartAsync();
        }
    }

    private async Task DisposeContainerAsync()
    {
        if (_container != null && !_containerDisposed)
        {
            await _container.DisposeAsync();
            _containerDisposed = true;
        }
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DisposeContainerAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await DisposeContainerAsync();
        await base.DisposeAsync();
    }
}
