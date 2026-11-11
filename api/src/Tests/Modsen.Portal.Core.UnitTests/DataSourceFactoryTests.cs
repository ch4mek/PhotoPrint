using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core.Database;
using Npgsql;

namespace Modsen.Portal.Core.UnitTests;

public class DataSourceFactoryTests
{
    [Fact]
    public void CreateShouldUseTrackDbConnectionStringWhenIamIsDisabled()
    {
        // Arrange
        const string connectionString =
            "Host=localhost;Port=5433;Database=portal;Username=portal-user;Password=portal-password;SSL Mode=Disable";
        var configuration = CreateConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:TrackDb"] = connectionString,
            ["AWS:RDS:UseAwsIam"] = "false"
        });

        // Act
        using var dataSource = new DataSourceFactory(configuration).Create();
        using var connection = dataSource.CreateConnection();
        var actual = new NpgsqlConnectionStringBuilder(connection.ConnectionString);

        // Assert
        actual.Host.Should().Be("localhost");
        actual.Port.Should().Be(5433);
        actual.Database.Should().Be("portal");
        actual.Username.Should().Be("portal-user");
        actual.SslMode.Should().Be(SslMode.Disable);
    }

    [Fact]
    public void CreateShouldUseIamDataSourceWhenIamIsEnabled()
    {
        // Arrange
        var configuration = CreateConfiguration(new Dictionary<string, string?>
        {
            ["AWS:RDS:UseAwsIam"] = "true",
            ["AWS:RDS:Host"] = "database.example.com",
            ["AWS:RDS:Port"] = "5433",
            ["AWS:RDS:Database"] = "portal",
            ["AWS:RDS:Username"] = "portal-user",
            ["AWS:RDS:CertificatePath"] = "/etc/ssl/certs/rds/rds-ca.pem"
        });

        // Act
        using var dataSource = new DataSourceFactory(configuration).Create();
        var actual = new NpgsqlConnectionStringBuilder(dataSource.ConnectionString);

        // Assert
        actual.Host.Should().Be("database.example.com");
        actual.Port.Should().Be(5433);
        actual.Database.Should().Be("portal");
        actual.Username.Should().Be("portal-user");
        actual.Password.Should().BeNullOrEmpty();
        actual.SslMode.Should().Be(SslMode.VerifyFull);
        actual.RootCertificate.Should().Be("/etc/ssl/certs/rds/rds-ca.pem");
    }

    private static IConfiguration CreateConfiguration(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
