using Modsen.Portal.Core;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.Requests.Files.Core.ImagePolicies;

namespace Modsen.Portal.Storage.Requests.Jobs;

internal class CreateAvatarSmallSquaresJob(
    ICloudStorageManager cloudStorageManager,
    IStorageDbContext dbContext,
    IApplicationImageResizeStorageService resizeService,
    IIdentityContract identityContract)
{
    const string SmallSquareIdentifyer = "_150_square";

    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var virtualDirectory = cloudStorageManager.GetVirtualDirectory();

        var fileNames = await cloudStorageManager.GetAllFileNamesAsync(virtualDirectory, cancellationToken);

        var fileWithSmallSquareIds = fileNames
            .Select(Path.GetFileNameWithoutExtension)
            .Where(fileName => !fileName.IsNullOrEmpty() && fileName.EndsWith(SmallSquareIdentifyer, StringComparison.Ordinal))
            .Select(x => x![..^SmallSquareIdentifyer.Length])
            .ToList();

        var originalIds = fileNames
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => name!.Length == 32)
            .ToHashSet(StringComparer.Ordinal);

        var needSmallSquareFileIds = (await identityContract.GetUserAvatarFileIds(cancellationToken)).Except(fileWithSmallSquareIds);
        var needSmallSquareFiles = dbContext.ApplicationFiles
            .Where(x => needSmallSquareFileIds.Contains(x.FileId));

        foreach(var file in needSmallSquareFiles)
        {
            if (!originalIds.Contains(file.FileId))
            {
                continue;
            }

            using var originalStream = await cloudStorageManager.GetFileContentAsync(
                file.VirtualDirectory,
                file.FilePath,
                cancellationToken);

            var resizedFilePath = ImageCompressionPolicy.BuildResizedPath(file.FilePath, ApplicationFileSize.SmallSquare);

            await resizeService.GenerateAndStoreResized(originalStream, resizedFilePath,
                file.VirtualDirectory, file.ContentType, ApplicationFileSize.SmallSquare, cancellationToken);
        }
    }
}
