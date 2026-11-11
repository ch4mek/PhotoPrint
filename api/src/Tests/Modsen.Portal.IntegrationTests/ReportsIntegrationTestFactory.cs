using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.IntegrationTests.Mocks;

namespace Modsen.Portal.IntegrationTests;

public class ReportsIntegrationTestFactory : IntegrationTestFactory, IAsyncLifetime
{
    public GuidGeneratorMock GuidGeneratorMock { get; set; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IGuidGenerator));

            services.AddSingleton(GuidGeneratorMock.Object);

        });

        base.ConfigureWebHost(builder);
    }
}
