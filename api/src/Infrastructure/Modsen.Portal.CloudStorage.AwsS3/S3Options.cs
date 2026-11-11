namespace Modsen.Portal.CloudStorage.AwsS3;

internal sealed class S3Options
{
    public string? Endpoint { get; set; }
    public string? BucketName { get; set; }
    public string? DefaultVirtualDirectory { get; set; }
}
