using Modsen.Portal.Core;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.Storage.Requests.Files.Core.ImagePolicies;

internal interface IApplicationImageResizeStorageService
{
    Task GenerateAndStoreResized(Stream originalStream, string resizedFilePath, string virtualDirectory,
        string contentType, ApplicationFileSize maxSize, CancellationToken cancellationToken);

    Task GenerateAndStoreManyResized(
        Stream originalStream,
        string virtualDirectory,
        string contentType,
        IReadOnlyList<(ApplicationFileSize MaxSize, string ResizedFilePath)> targets,
        CancellationToken cancellationToken);
}

internal sealed class ApplicationImageResizeStorageService(
    ICloudStorageManager cloudStorageManager,
    IImageScaler imageScaler)
    : IApplicationImageResizeStorageService
{
    public Task GenerateAndStoreResized(Stream originalStream, string resizedFilePath, string virtualDirectory,
        string contentType, ApplicationFileSize maxSize, CancellationToken cancellationToken)
    {
        return GenerateAndStoreManyResized(
            originalStream, virtualDirectory, contentType,
            [(MaxSize: maxSize, ResizedFilePath: resizedFilePath)],
            cancellationToken);
    }

    public async Task GenerateAndStoreManyResized(
        Stream originalStream,
        string virtualDirectory,
        string contentType,
        IReadOnlyList<(ApplicationFileSize MaxSize, string ResizedFilePath)> targets,
        CancellationToken cancellationToken)
    {
        if (targets.Count == 0)
        {
            return;
        }

        var maxSizes = targets.ToDictionary(
            x => x.MaxSize,
            x => ImageCompressionPolicy.GetMaxSizeForCompression(x.MaxSize));

        var streams = imageScaler.ScaleWithoutUpscale(originalStream, contentType, maxSizes);

        try
        {
            for (var i = 0; i < targets.Count; i++)
            {
                await cloudStorageManager.StoreFileAsync(
                    virtualDirectory,
                    targets[i].ResizedFilePath,
                    contentType,
                    streams[i],
                    cancellationToken);
            }
        }
        finally
        {
            foreach (var s in streams)
            {
                await s.DisposeAsync();
            }
        }
    }
}
