using MediatR;
using Modsen.Portal.Storage.Contract;
using Modsen.Portal.Storage.Contract.Models;
using Modsen.Portal.Storage.Requests.Files.Requests;

namespace Modsen.Portal.Storage.Requests;

internal class StorageCrossWriterContract : IStorageCrossWriterContract
{
    private readonly ISender _sender;

    public StorageCrossWriterContract(ISender sender)
    {
        _sender = sender;
    }

    public async Task<string> UploadFileInternal(UploadFileDto uploadFileDto, CancellationToken cancellationToken)
    {
        var command = new UploadFileRequest.InternalCommand
        {
            FileContent = uploadFileDto.FileContent.ToArray(),
            FileName = uploadFileDto.FileName,
            ContentType = uploadFileDto.ContentType,
            FileSize = uploadFileDto.FileSize,
            UserId = uploadFileDto.UserId,
            AvailableTill = uploadFileDto.AvailableTill,
            FileSource = uploadFileDto.FileSource,
        };
        var response = await _sender.Send(command, cancellationToken);

        return response.FileId;
    }
}
