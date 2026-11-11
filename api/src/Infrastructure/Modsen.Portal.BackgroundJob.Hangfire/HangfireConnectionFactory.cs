using Npgsql;
using IConnectionFactory = Hangfire.PostgreSql.IConnectionFactory;

namespace Modsen.Portal.BackgroundJob.Hangfire;

internal class HangfireConnectionFactory : IConnectionFactory
{
    private readonly NpgsqlDataSource _npgsqlDataSource;

    public HangfireConnectionFactory(NpgsqlDataSource npgsqlDataSource)
    {
        _npgsqlDataSource = npgsqlDataSource;
    }

    public NpgsqlConnection GetOrCreateConnection()
    {
        return _npgsqlDataSource.OpenConnection();
    }
}
