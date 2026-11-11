namespace Modsen.Portal.Core.Services;

public interface ICloudStorageManager
{
    Task<string> GenerateUrl(string virtualDirectory, string fileKey, CancellationToken cancellationToken);
    Task<string> StoreFileAsync(string virtualDirectory, string fileKey, string? contentType, byte[] content, CancellationToken cancellationToken);
    Task<string> StoreFileAsync(string virtualDirectory, string fileKey, string? contentType, Stream streamContent, CancellationToken cancellationToken);
    Task<bool> FileExistsByListingAsync(string virtualDirectory, string fileKey, CancellationToken cancellationToken);
    Task<bool> FileExistsByMetadataAsync(string virtualDirectory, string fileKey, CancellationToken cancellationToken);

    Task<Stream> GetFileContentAsync(string virtualDirectory, string fileKey, CancellationToken cancellationToken);
    Task RemoveFileAsync(string virtualDirectory, string fileKey, CancellationToken cancellationToken);

    string GetBucketName();
    string GetVirtualDirectory();
    Stream ScaleImage(Stream inputStream, int maxWidth, int maxHeight);
    Stream ScaleImageWithoutUpscale(Stream inputStream, int maxWidth, int maxHeight, string? contentType = null);

    Task CopyFileAsync(string sourceVirtualDirectory, string sourceFileKey, string destinationVirtualDirectory,
        string destinationFileKey, CancellationToken cancellationToken);
    Task MarkFileAsDeprecatedAsync(string virtualDirectory, string fileKey, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<string>> GetAllFileNamesAsync(string virtualDirectory, CancellationToken cancellationToken);
}
