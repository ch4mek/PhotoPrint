using Microsoft.Extensions.Configuration;

namespace Modsen.Portal.Core.Database;

public class DataSourceFactory
{
    private readonly IConfiguration _configuration;

    public DataSourceFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public PortalDbDataSource Create()
    {
        var connectionString = _configuration.GetConnectionString("TrackDb");

        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        return new PortalDbDataSource(connectionString);
    }
}
