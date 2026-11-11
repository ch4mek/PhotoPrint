using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Jira.Contract;
using Moq;

namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.JiraController;

public sealed class JiraIntegrationTestFactory : IntegrationTestFactory
{
    public Mock<IJiraService> JiraServiceMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(options =>
        {
            var appSettings = new Dictionary<string, string>
            {
                { "Encryption:JiraSetting:Key", "MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY=" },
                { "Encryption:JiraSetting:Vector", "YWJjZGVmMDEyMzQ1Njc4OQ==" }
            };

            options.AddInMemoryCollection(appSettings!);
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(JiraServiceMock.Object);
        });

        base.ConfigureWebHost(builder);
    }
}
