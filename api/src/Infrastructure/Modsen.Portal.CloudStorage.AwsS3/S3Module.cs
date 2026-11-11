using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Modules;

namespace Modsen.Portal.CloudStorage.AwsS3;

public sealed class S3Module : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        var configurationSection = Configuration.GetSection("S3");
        var awsS3Options = GetAwsOptions(configurationSection);

        services.AddDefaultAWSOptions(awsS3Options);
        services.AddAWSService<IAmazonS3>();

        services.Configure<S3Options>(configurationSection);
        services.AddScoped<ICloudStorageManager, S3StorageManager>();
        services.AddSingleton<IImageScaler, ImageSharpScaler>();
    }

    internal static AWSOptions GetAwsOptions(IConfigurationSection configurationSection)
    {
        var awsS3Options = new AWSOptions
        {
            Region = RegionEndpoint.EUCentral1,
            DefaultClientConfig = { ServiceURL = configurationSection["Endpoint"] }
        };

        var accessKey = configurationSection["AccessKey"];
        var secretKey = configurationSection["SecretKey"];

        if (!string.IsNullOrWhiteSpace(accessKey) != !string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("Invalid AWS S3 configuration: S3:AccessKey and S3:SecretKey must either both be set or both be omitted.");
        }

        if (!string.IsNullOrWhiteSpace(secretKey) && !string.IsNullOrWhiteSpace(accessKey))
        {
            awsS3Options.Credentials = new BasicAWSCredentials(accessKey, secretKey);
        }

        return awsS3Options;
    }
}
