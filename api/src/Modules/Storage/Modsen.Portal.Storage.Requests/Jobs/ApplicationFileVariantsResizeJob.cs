using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Storage.Contract.Events;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.Requests.Files.Core.ImagePolicies;

namespace Modsen.Portal.Storage.Requests.Jobs;

internal sealed class ApplicationFileVariantsResizeJob(
    ICloudStorageManager cloudStorageManager,
    IStorageDbContext dbContext,
    IApplicationImageResizeStorageService resizeService,
    ILogger<ApplicationFileVariantsResizeJob> logger)
{
    public async Task Execute(ApplicationFileUploadedEvent @event, CancellationToken cancellationToken)
    {
        var file = await dbContext.ApplicationFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.FileId == @event.FileId, cancellationToken);

        if (file is null || !ImageCompressionPolicy.IsResizableImage(file.ContentType))
        {
            return;
        }

        var targetSizes = ImageCompressionPolicy.GetTargetSizes(@event.FilePurpose);

        if (targetSizes.Count == 0)
        {
            return;
        }

        var originalExists = await cloudStorageManager.FileExistsByMetadataAsync(
            file.VirtualDirectory,
            file.FilePath,
            cancellationToken);

        if (!originalExists)
        {
            logger.LogWarning("Resize skipped missing original for FileId={FileId}, Path={FilePath}",
                file.FileId, file.FilePath);

            return;
        }

        logger.LogInformation("Downloading original file bytes");

        using var originalStream = await cloudStorageManager.GetFileContentAsync(
            file.VirtualDirectory,
            file.FilePath,
            cancellationToken);

        logger.LogInformation("Original file downloaded. Bytes={Bytes}. SizesToProcess={SizesCount}",
            originalStream.Length, targetSizes.Count);

        var targets = targetSizes
            .Select(size => (
                MaxSize: size,
                ResizedFilePath: ImageCompressionPolicy.BuildResizedPath(file.FilePath, size)))
            .ToArray();

        var sizeSw = Stopwatch.StartNew();

        try
        {
            await resizeService.GenerateAndStoreManyResized(
                originalStream, file.VirtualDirectory, file.ContentType, targets, cancellationToken);

            logger.LogInformation(
                "Variants generated. FileId={FileId}, Sizes={SizesCount}, ElapsedMs={ElapsedMs}",
                file.FileId, targets.Length, sizeSw.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Variant generation failed. FileId={FileId}, ElapsedMs={ElapsedMs}",
                file.FileId, sizeSw.ElapsedMilliseconds);
            throw;
        }
    }
}
