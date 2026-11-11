using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.Requests.Files.Core.ImagePolicies;

namespace Modsen.Portal.Storage.Requests.Files.Requests;

internal sealed class DeleteFileRequest : ICommandHandler<DeleteFileRequest.Command, Unit>
{
    private readonly ICloudStorageManager _cloudStorageManager;
    private readonly IStorageDbContext _dbContext;

    public DeleteFileRequest(ICloudStorageManager cloudStorageManager, IStorageDbContext dbContext)
    {
        _cloudStorageManager = cloudStorageManager;
        _dbContext = dbContext;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var file = await _dbContext.ApplicationFiles
            .Where(f => f.FileId == request.FileId)
            .FirstOrDefaultAsync(cancellationToken);

        if (file == null)
        {
            throw new EntityNotFoundException("File does not exist");
        }

        if (request.FileSource != null && request.FileSource != file.FileSource)
        {
            throw new ActionNotAllowedException("You are not allowed to delete file from other source");
        }

        _dbContext.ApplicationFiles.Remove(file);

        await _dbContext.SaveChangesAsync(cancellationToken);

        foreach (var filePath in BuildFilePathsForDeletion(file.FilePath))
        {
            await _cloudStorageManager.RemoveFileAsync(
                file.VirtualDirectory,
                filePath,
                cancellationToken);
        }

        return Unit.Value;
    }

    private static IEnumerable<string> BuildFilePathsForDeletion(string originalFilePath)
    {
        yield return originalFilePath;

        foreach (var size in ApplicationImageSizes.AllImageSizes)
        {
            yield return ImageCompressionPolicy.BuildResizedPath(
                originalFilePath,
                size);
        }
    }

    internal sealed record Command(string FileId, FileSource? FileSource) : ICommand<Unit>, ITransactionalRequest;
}
