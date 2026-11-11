using Modsen.Portal.Storage.Contract.Models;

namespace Modsen.Portal.Storage.Contract;

public interface IStorageCrossWriterContract
{
    Task<string> UploadFileInternal(UploadFileDto uploadFileDto, CancellationToken cancellationToken);
}
