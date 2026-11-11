using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.DataAccess.Npgsql.Entities;
using Modsen.Portal.Storage.Requests.Files.Core.ImagePolicies;

namespace Modsen.Portal.Storage.Requests.Jobs;

internal sealed class BackfillApplicationImageVariantsJob(
    IStorageDbContext dbContext,
    ICloudStorageManager cloudStorageManager,
    IApplicationImageResizeStorageService resizeService,
    ILogger<BackfillApplicationImageVariantsJob> logger,
    IIdentityContract identityContract)
{
    private const int DbBatchSize = 1;
    private static readonly ApplicationFileSize[] targetSizes = [
        ApplicationFileSize.Medium,
        ApplicationFileSize.Small,
        ApplicationFileSize.SmallSquare];

    public async Task<BackfillChunkResult> Execute(CancellationToken cancellationToken = default)
    {
        var avatarFileIds = await identityContract.GetUserAvatarFileIds(cancellationToken);
        var files = await GetBatchAsync(avatarFileIds, cancellationToken);

        if (files.Count == 0)
        {
            return new BackfillChunkResult(HasMore: false);
        }

        logger.LogInformation("Backfill chunk started. BatchSize={BatchSize}", DbBatchSize);

        var stats = new BackfillStats();

        foreach (var file in files)
        {
            stats.Processed++;

            try
            {
                await ProcessFileAsync(file, stats, cancellationToken);
                file.ImageStatus = ImageResizingStatus.Processed;
                file.ImageVariantsBackfilledAtUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                stats.Failed++;
                logger.LogError(ex, "Backfill failed for FileId={FileId}, Path={FilePath}", file.FileId,
                    file.FilePath);
                file.ImageStatus = ImageResizingStatus.Failed;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var hasMore = await dbContext.ApplicationFiles
            .AnyAsync(x => x.ImageStatus != ImageResizingStatus.Processed && EF.Parameter(avatarFileIds).Contains(x.FileId), cancellationToken);

        LogCompleted(stats, hasMore);

        return new BackfillChunkResult(HasMore: hasMore);
    }

    private async Task<List<ApplicationFileEntity>> GetBatchAsync(ICollection<string> avatarFileIds, CancellationToken cancellationToken)
    {
        return await dbContext.ApplicationFiles
            .Where(x => x.ImageStatus != ImageResizingStatus.Processed && EF.Parameter(avatarFileIds).Contains(x.FileId))
            .OrderBy(x => x.FileId)
            .Take(DbBatchSize)
            .ToListAsync(cancellationToken);
    }

    private async Task ProcessFileAsync(
        ApplicationFileEntity file,
        BackfillStats stats,
        CancellationToken cancellationToken)
    {
        if (!ImageCompressionPolicy.IsResizableImage(file.ContentType))
        {
            stats.Skipped++;
            return;
        }

        var migratedLegacy = await TryMigrateLegacyThumbnailAsync(file, cancellationToken);

        if (migratedLegacy)
        {
            stats.RenamedLegacy++;
            return;
        }

        await EnsureResizedVariantsAsync(file, stats, cancellationToken);
    }

    private async Task<bool> TryMigrateLegacyThumbnailAsync(
        ApplicationFileEntity file,
        CancellationToken cancellationToken)
    {
        var legacyThumbnailPath = BuildLegacyThumbnailPath(file.FilePath);

        var legacyExists = await cloudStorageManager.FileExistsByMetadataAsync(
            file.VirtualDirectory,
            legacyThumbnailPath,
            cancellationToken);

        if (!legacyExists)
        {
            return false;
        }

        var smallPath = ImageCompressionPolicy.BuildResizedPath(file.FilePath, ApplicationFileSize.Thumbnail);

        var smallExists = await cloudStorageManager.FileExistsByMetadataAsync(
            file.VirtualDirectory,
            smallPath,
            cancellationToken);

        if (!smallExists)
        {
            await cloudStorageManager.CopyFileAsync(
                file.VirtualDirectory,
                legacyThumbnailPath,
                file.VirtualDirectory,
                smallPath,
                cancellationToken);
        }

        await cloudStorageManager.MarkFileAsDeprecatedAsync(
            file.VirtualDirectory,
            legacyThumbnailPath,
            cancellationToken);

        return true;
    }

    private async Task EnsureResizedVariantsAsync(
        ApplicationFileEntity file,
        BackfillStats stats,
        CancellationToken cancellationToken)
    {
        var originalExists = await cloudStorageManager.FileExistsByMetadataAsync(
            file.VirtualDirectory,
            file.FilePath,
            cancellationToken);

        if (!originalExists)
        {
            logger.LogWarning("Backfill skipped missing original for FileId={FileId}, Path={FilePath}",
                file.FileId, file.FilePath);
            stats.Skipped++;

            return;
        }

        await using var originalStream = await cloudStorageManager.GetFileContentAsync(
            file.VirtualDirectory,
            file.FilePath,
            cancellationToken);

        var targets = targetSizes
            .Select(size => (
                MaxSize: size,
                ResizedFilePath: ImageCompressionPolicy.BuildResizedPath(file.FilePath, size)))
            .ToArray();

        await resizeService.GenerateAndStoreManyResized(
            originalStream, file.VirtualDirectory, file.ContentType, targets, cancellationToken);

        stats.Created += targets.Length;
    }

    private static string BuildLegacyThumbnailPath(string filePath)
    {
        var ext = Path.GetExtension(filePath);
        var baseName = Path.GetFileNameWithoutExtension(filePath);

        return $"{baseName}_thumbnail{ext}";
    }

    private void LogCompleted(BackfillStats stats, bool hasMore)
    {
        logger.LogInformation(
            "Backfill completed: processed={Processed}, created={Created}, skipped={Skipped}, renamed={RenamedLegacy}, failed={Failed}, hasMore={HasMore}",
            stats.Processed,
            stats.Created,
            stats.Skipped,
            stats.RenamedLegacy,
            stats.Failed,
            hasMore);
    }

    internal sealed record BackfillChunkResult(bool HasMore);

    private sealed class BackfillStats
    {
        public int Processed { get; set; }
        public int Created { get; set; }
        public int Skipped { get; set; }
        public int Failed { get; set; }
        public int RenamedLegacy { get; set; }
    }
}
