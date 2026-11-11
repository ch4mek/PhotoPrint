using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Storage.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Storage.Requests.Files.Core.Composers;

internal interface IUploadFileStreamComposer
{
    Task<ApplicationFileEntity> ComposeAsync(
        Stream stream,
        string fileName,
        string contentType,
        FileSource fileSource,
        ApplicationFilePurpose filePurpose,
        CancellationToken cancellationToken);
}

internal sealed class UploadFileStreamComposer : IUploadFileStreamComposer
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICloudStorageManager _cloudStorageManager;
    private readonly IGuidGenerator _guidGenerator;

    public UploadFileStreamComposer(IAuthenticatedUserContext authenticatedUserContext,
        ICloudStorageManager cloudStorageManager,
        IGuidGenerator guidGenerator)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _cloudStorageManager = cloudStorageManager;
        _guidGenerator = guidGenerator;
    }

    public async Task<ApplicationFileEntity> ComposeAsync(
        Stream stream,
        string fileName,
        string contentType,
        FileSource fileSource,
        ApplicationFilePurpose filePurpose,
        CancellationToken cancellationToken)
    {
        var fileId = $"{_guidGenerator.Create():N}";
        var filePath = $"{fileId}{Path.GetExtension(fileName)}";
        var virtualDirectory = _cloudStorageManager.GetVirtualDirectory();

        var applicationFile = new ApplicationFileEntity
        {
            FileId = fileId,
            BucketName = _cloudStorageManager.GetBucketName(),
            VirtualDirectory = virtualDirectory,
            FilePath = filePath,
            OriginalFileName = fileName,
            UploadedByUserId = _authenticatedUserContext.UserId,
            ContentType = contentType,
            AvailableTill = null,
            FileSource = fileSource,
            FilePurpose = filePurpose
        };

        await _cloudStorageManager.StoreFileAsync(virtualDirectory, filePath, contentType, stream, cancellationToken);

        return applicationFile;
    }
}
