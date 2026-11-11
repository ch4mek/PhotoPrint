using Microsoft.Extensions.Configuration;

namespace Modsen.Portal.Core.Database;

public abstract class DesignTimeBaseDbContext
{
    protected static string GetConnectionString()
    {
        var pathToDataAccess = Directory.GetCurrentDirectory();
        var pathToApi = Path.Combine(pathToDataAccess, "../../../../src/Modsen.Portal.Api/");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(pathToApi)
            .AddJsonFile("appsettings.Development.json")
            .Build();

        return configuration.GetConnectionString("TrackDb") ?? string.Empty;
    }
}
