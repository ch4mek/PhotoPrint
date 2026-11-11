using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace Modsen.Portal.IntegrationTests;

internal static class InfrastructureForTesting
{
    public static PostgreSqlContainer CreateDbContainer()
    {
        var databaseName = $"TrackDbTest_{Guid.CreateVersion7():N}";
        var waitStrategy = Wait.ForUnixContainer()
            .UntilCommandIsCompleted($"psql -U postgres -d {databaseName} -c SELECT 1;",
                strategy => strategy.WithRetries(5)
                    .WithTimeout(TimeSpan.FromSeconds(60))
                    .WithInterval(TimeSpan.FromSeconds(10)));

        return new PostgreSqlBuilder()
            .WithImage("postgres:15.3")
            .WithPortBinding(5432, assignRandomHostPort: true)
            .WithHostname("db_for_integration_tests")
            .WithDatabase(databaseName)
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithWaitStrategy(waitStrategy)
            .WithReuse(true)
            .WithTmpfsMount("/tmp")
            // .WithAutoRemove(true)
            .Build();
    }
}
