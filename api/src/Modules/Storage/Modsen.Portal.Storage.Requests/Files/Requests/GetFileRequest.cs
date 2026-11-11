using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.Requests.Files.Core.CoreDto;
using Modsen.Portal.Storage.Requests.Files.Core.ImagePolicies;
using Modsen.Portal.Storage.Requests.Files.Core.Loaders;

namespace Modsen.Portal.Storage.Requests.Files.Requests;

internal sealed class GetFileRequest : IQueryHandler<GetFileRequest.Query, GetFileRequest.Response>
{
    private readonly ICloudStorageManager _cloudStorageManager;
    private readonly IStorageDbContext _dbContext;
    private readonly IClockService _clockService;
    private readonly IFileStreamLoader _fileStreamLoader;

    public GetFileRequest(
        ICloudStorageManager cloudStorageManager,
        IStorageDbContext dbContext,
        IClockService clockService,
        IFileStreamLoader fileStreamLoader)
    {
        _cloudStorageManager = cloudStorageManager;
        _dbContext = dbContext;
        _clockService = clockService;
        _fileStreamLoader = fileStreamLoader;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var file = await _dbContext.ApplicationFiles.AsNoTracking()
            .Where(file => file.FileId == request.FileId &&
                (!file.AvailableTill.HasValue || file.AvailableTill.Value > _clockService.UtcNow))
            .Select(file => new FileCoreDto
            {
                FilePath = file.FilePath,
                VirtualDirectory = file.VirtualDirectory,
                FileId = file.FileId,
                ContentType = file.ContentType,
                OriginalFileName = file.OriginalFileName,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (file == null)
        {
            return new Response(ContentType: string.Empty, ContentStream: null, IsFallbackToOriginal: false);
        }

        if (ImageCompressionPolicy.RequiresCompression(request.Size, file.ContentType))
        {
            return await GetResizedOrFallbackToOriginal(file, request.Size, cancellationToken);
        }

        return await GetOriginal(file, cancellationToken);
    }

    private async Task<Response> GetResizedOrFallbackToOriginal(FileCoreDto file, ApplicationFileSize targetSize,
        CancellationToken cancellationToken)
    {
        var resizedFilePath = ImageCompressionPolicy.BuildResizedPath(file.FilePath, targetSize);

        var resizedExists = await _cloudStorageManager.FileExistsByMetadataAsync(
            file.VirtualDirectory,
            resizedFilePath,
            cancellationToken);

        if (resizedExists)
        {
            return await GetFile(file with { FilePath = resizedFilePath }, cancellationToken,
                isFallbackToOriginal: false);
        }

        return await GetOriginal(file, cancellationToken, isFallbackToOriginal: true);
    }

    private async Task<Response> GetOriginal(FileCoreDto file, CancellationToken cancellationToken,
        bool? isFallbackToOriginal = null)
    {
        return await GetFile(file, cancellationToken, isFallbackToOriginal);
    }

    private async Task<Response> GetFile(FileCoreDto file, CancellationToken cancellationToken,
        bool? isFallbackToOriginal = null)
    {
        var contentStream = await _fileStreamLoader.LoadAsync(file, cancellationToken);

        return new Response(file.ContentType, contentStream, isFallbackToOriginal, file.OriginalFileName);
    }

    internal sealed record Query(
        string FileId,
        ApplicationFileSize Size = ApplicationFileSize.Original) : IQuery<Response>;

    internal record Response(string ContentType, Stream? ContentStream, bool? IsFallbackToOriginal = null, string? OriginalFileName = null);
}
