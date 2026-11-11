using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Modsen.Portal.Core.Database;

public class DataSourceFactory
{
    private readonly IConfiguration _configuration;

    public DataSourceFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public NpgsqlDataSource Create()
    {
        if (_configuration.GetValue<bool>("AWS:RDS:UseAwsIam"))
        {
            return new AwsIamDataSourceFactory(_configuration).Create();
        }

        var connectionString = _configuration.GetConnectionString("TrackDb");

        ArgumentException.ThrowIfNullOrEmpty(connectionString);

        return NpgsqlDataSource.Create(connectionString);
    }
}
