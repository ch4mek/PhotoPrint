using Modsen.Portal.Core;
using Modsen.Portal.Storage.Contract.Models;

namespace Modsen.Portal.Storage.Contract;

public interface IStorageContract
{
    Task<bool> IsFileExists(string fileId, CancellationToken cancellationToken);

    Task<string?> GetFileBase64Async(string fileId, FileSource? fileSource, CancellationToken cancellationToken);

    Task<string?> GetFileBase64Async(string fileId, ApplicationFileSize fileSize, CancellationToken cancellationToken);

    Task<string> UploadFile(UploadFileDto uploadFileDto, CancellationToken cancellationToken);

    Task<List<ApplicationFileDto>> UploadFiles(UploadFilesDto uploadFilesDto, CancellationToken cancellationToken);

    Task DeleteFile(string fileId, FileSource? fileSource, CancellationToken cancellationToken);
}
