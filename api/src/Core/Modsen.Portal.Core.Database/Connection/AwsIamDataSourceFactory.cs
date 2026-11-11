using Amazon.RDS.Util;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Modsen.Portal.Core.Database;

internal sealed class AwsIamDataSourceFactory
{
    private readonly IConfiguration _configuration;

    public AwsIamDataSourceFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public NpgsqlDataSource Create()
    {
        var host = GetRequired("AWS:RDS:Host");
        var database = GetRequired("AWS:RDS:Database");
        var username = GetRequired("AWS:RDS:Username");
        var port = GetPort();
        var certificatePath = _configuration["AWS:RDS:CertificatePath"];

        ArgumentException.ThrowIfNullOrWhiteSpace(certificatePath);

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = port,
            Database = database,
            Username = username,
            Password = null,
            MinPoolSize = 0,
            MaxPoolSize = 20,
            Timeout = 15,
            SslMode = SslMode.VerifyFull,
            RootCertificate = certificatePath
        };
        var builder = new NpgsqlDataSourceBuilder(connectionStringBuilder.ConnectionString);

        builder.UsePeriodicPasswordProvider(
            (_, ct) => GetIamToken(host, port, username, ct),
            successRefreshInterval: TimeSpan.FromMinutes(10),
            failureRefreshInterval: TimeSpan.FromSeconds(5));

        return builder.Build();
    }

    private static ValueTask<string> GetIamToken(string host, int port, string username, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var token = RDSAuthTokenGenerator.GenerateAuthToken(host, port, username);

        return ValueTask.FromResult(token);
    }

    private string GetRequired(string key)
    {
        return _configuration[key] ?? throw new InvalidOperationException($"Missing {key} in configuration.");
    }

    private int GetPort()
    {
        var value = _configuration["AWS:RDS:Port"];

        if (string.IsNullOrWhiteSpace(value))
        {
            return 5432;
        }

        if (!int.TryParse(value, out var port))
        {
            throw new InvalidOperationException($"Invalid AWS:RDS:Port value: '{value}'.");
        }

        return port;
    }
}
