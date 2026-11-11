using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Storage.Contract;
using Modsen.Portal.Storage.Contract.Models;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.Requests.Files.Core.Loaders;
using Modsen.Portal.Storage.Requests.Files.Requests;

namespace Modsen.Portal.Storage.Requests;

internal class StorageContract : IStorageContract
{
    private readonly IStorageDbContext _storageDbContext;
    private readonly IClockService _clockService;
    private readonly ISender _sender;
    private readonly IFileStreamLoader _fileStreamLoader;

    public StorageContract(IStorageDbContext storageDbContext, IClockService clockService, ISender sender,
        IFileStreamLoader fileStreamLoader)
    {
        _storageDbContext = storageDbContext;
        _clockService = clockService;
        _sender = sender;
        _fileStreamLoader = fileStreamLoader;
    }

    public async Task<string?> GetFileBase64Async(string fileId, FileSource? fileSource, CancellationToken cancellationToken)
    {
        var now = _clockService.UtcNow;
        var file = await _storageDbContext.ApplicationFiles
            .AsNoTracking()
            .Where(x => x.FileId == fileId)
            .Where(x => x.AvailableTill == null || x.AvailableTill >= now)
            .FirstOrDefaultAsync(cancellationToken);

        if (file is null)
        {
            return null;
        }

        if (file.FilePurpose != ApplicationFilePurpose.UserAvatar &&
            fileSource is not null &&
            file.FileSource != fileSource)
        {
            throw new ActionNotAllowedException("You are not allowed to get file from other source");
        }

        using var stream = await _fileStreamLoader.LoadAsync(file, cancellationToken);
        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream, cancellationToken);
        var buffer = memoryStream.ToArray();

        var base64string = Convert.ToBase64String(buffer);
        return $"data:{file.ContentType};base64,{base64string}";
    }

    public async Task<string?> GetFileBase64Async(string fileId, ApplicationFileSize fileSize, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetFileRequest.Query(fileId, fileSize), cancellationToken);

        if (response?.ContentStream == null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream();

        await response.ContentStream.CopyToAsync(memoryStream, cancellationToken);
        var buffer = memoryStream.ToArray();

        var base64string = Convert.ToBase64String(buffer);
        return $"data:{response.ContentType};base64,{base64string}";
    }

    public Task<bool> IsFileExists(string fileId, CancellationToken cancellationToken)
    {
        var now = _clockService.UtcNow;
        return _storageDbContext.ApplicationFiles
            .Where(x => x.FileId == fileId)
            .Where(x => x.AvailableTill == null || x.AvailableTill >= now)
            .AnyAsync(cancellationToken);
    }

    public async Task<string> UploadFile(UploadFileDto uploadFileDto, CancellationToken cancellationToken)
    {
        var command = new UploadFileRequest.Command
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

    public async Task<List<ApplicationFileDto>> UploadFiles(UploadFilesDto uploadFilesDto, CancellationToken cancellationToken)
    {
        var command = new UploadFilesRequest.Command(
            uploadFilesDto.Boundary,
            uploadFilesDto.ContentStream,
            uploadFilesDto.FileSource,
            uploadFilesDto.FilePurpose);

        var response = await _sender.Send(command, cancellationToken);

        return response
            .Select(x => new ApplicationFileDto(x.FileId, x.FileSize, x.FileName))
            .ToList();
    }

    public Task DeleteFile(string fileId, FileSource? fileSource, CancellationToken cancellationToken)
    {
        var command = new DeleteFileRequest.Command(fileId, fileSource);

        return _sender.Send(command, cancellationToken);
    }
}
