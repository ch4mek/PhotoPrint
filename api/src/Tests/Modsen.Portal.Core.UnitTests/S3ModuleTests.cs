using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Modsen.Portal.CloudStorage.AwsS3;

namespace Modsen.Portal.Core.UnitTests;

public class S3ModuleTests
{
    [Fact]
    public async Task GetAwsOptionsShouldUseBasicCredentialsWhenBothKeysAreConfigured()
    {
        // Arrange
        var configuration = CreateConfiguration("access-key", "secret-key");

        // Act
        var options = S3Module.GetAwsOptions(configuration.GetSection("S3"));
        var credentials = await options.Credentials.GetCredentialsAsync();

        // Assert
        options.Credentials.Should().BeOfType<BasicAWSCredentials>();
        credentials.AccessKey.Should().Be("access-key");
        credentials.SecretKey.Should().Be("secret-key");
    }

    [Fact]
    public void GetAwsOptionsShouldUseDefaultCredentialChainWhenBothKeysAreOmitted()
    {
        // Arrange
        var configuration = CreateConfiguration(null, null);

        // Act
        var options = S3Module.GetAwsOptions(configuration.GetSection("S3"));

        // Assert
        options.Credentials.Should().BeNull();
    }

    [Theory]
    [InlineData("access-key", null)]
    [InlineData(null, "secret-key")]
    public void GetAwsOptionsShouldRejectPartiallyConfiguredCredentials(string? accessKey, string? secretKey)
    {
        // Arrange
        var configuration = CreateConfiguration(accessKey, secretKey);

        // Act
        var action = () => S3Module.GetAwsOptions(configuration.GetSection("S3"));

        // Assert
        action.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Invalid AWS S3 configuration: S3:AccessKey and S3:SecretKey must either both be set or both be omitted.");
    }

    private static IConfiguration CreateConfiguration(string? accessKey, string? secretKey)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["S3:Endpoint"] = "https://s3.example.com",
                ["S3:AccessKey"] = accessKey,
                ["S3:SecretKey"] = secretKey
            })
            .Build();
    }
}
