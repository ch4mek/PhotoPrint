using System.Net.Mime;
using System.Runtime.CompilerServices;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Storage.Contract.Events;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.DataAccess.Npgsql.Entities;

[assembly: InternalsVisibleTo("Modsen.Portal.ArchitectureTests")]

namespace Modsen.Portal.Storage.Requests.Files.Requests;

internal sealed class UploadFileRequest :
    ICommandHandler<UploadFileRequest.Command, UploadFileRequest.Response>,
    ICommandHandler<UploadFileRequest.InternalCommand, UploadFileRequest.Response>
{
    private readonly ICloudStorageManager _cloudStorageManager;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IStorageDbContext _dbContext;
    private readonly ICommunicationBus _communicationBus;

    public UploadFileRequest(ICloudStorageManager cloudStorageManager,
        IGuidGenerator guidGenerator,
        IStorageDbContext dbContext,
        ICommunicationBus communicationBus)
    {
        _cloudStorageManager = cloudStorageManager;
        _guidGenerator = guidGenerator;
        _dbContext = dbContext;
        _communicationBus = communicationBus;
    }

    public Task<Response> Handle(Command command, CancellationToken cancellationToken)
    {
        return UploadFileAsync(command, cancellationToken);
    }

    public Task<Response> Handle(InternalCommand command, CancellationToken cancellationToken)
    {
        return UploadFileAsync(command, cancellationToken);
    }

    private async Task<Response> UploadFileAsync(InternalCommand command,
        CancellationToken cancellationToken)
    {
        var fileId = $"{_guidGenerator.Create():N}";
        var extension = Path.GetExtension(command.FileName);
        var fileName = $"{fileId}{extension}";

        var virtualDirectory = _cloudStorageManager.GetVirtualDirectory();

        var file = await _dbContext.ApplicationFiles
            .FirstOrDefaultAsync(file => file.FileId == fileId, cancellationToken);

        if (file == null)
        {
            file = new ApplicationFileEntity
            {
                FileId = fileId,
                BucketName = _cloudStorageManager.GetBucketName(),
                VirtualDirectory = virtualDirectory,
                FilePath = fileName,
                OriginalFileName = command.FileName,
                UploadedByUserId = command.UserId,
                ContentType = command.ContentType,
                AvailableTill = command.AvailableTill,
                FileSource = command.FileSource,
                FilePurpose = command.Purpose
            };

            _dbContext.ApplicationFiles.Add(file);
        }
        else
        {
            file.BucketName = _cloudStorageManager.GetBucketName();
            file.VirtualDirectory = virtualDirectory;
            file.FilePath = fileName;
            file.OriginalFileName = command.FileName;
            file.UploadedByUserId = command.UserId;
            file.ContentType = command.ContentType;
            file.AvailableTill = command.AvailableTill;

            _dbContext.ApplicationFiles.Update(file);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await _cloudStorageManager.StoreFileAsync(virtualDirectory, fileName,
            command.ContentType, command.FileContent, cancellationToken);

        var notification = new ApplicationFileUploadedEvent
        {
            EventId = Guid.CreateVersion7().ToString(),
            FileId = file.FileId,
            FilePurpose = command.Purpose,
            ContentType = command.ContentType
        };

        await _communicationBus.PublishAsync(notification, cancellationToken);

        return new Response(FileId: fileId, FileSize: command.FileSize, FileName: fileName);
    }

    internal sealed class Command : InternalCommand, ITransactionalRequest;

    internal class InternalCommand : ICommand<Response>
    {
        public required byte[] FileContent { get; init; }
        public required string FileName { get; init; }
        public required string ContentType { get; init; }
        public required long FileSize { get; init; }
        public required int? UserId { get; init; }
        public required DateTime? AvailableTill { get; set; }
        public required FileSource FileSource { get; init; }
        public ApplicationFilePurpose Purpose { get; init; } = ApplicationFilePurpose.General;
    }

    internal record Response(string FileId, long FileSize, string? FileName);

    internal sealed class Validator : AbstractValidator<Command>
    {
        private const long s_oneMb = 1024 * 1024;

        private readonly string[] _allowedMediaTypes =
            new[]
                {
                    MediaTypeNames.Image.Png, MediaTypeNames.Image.Jpeg, MediaTypeNames.Application.Pdf,
                    PortalMediaTypeNames.Word, PortalMediaTypeNames.WordX, PortalMediaTypeNames.PowerPointX,
                    PortalMediaTypeNames.Excel, PortalMediaTypeNames.ExcelX, MediaTypeNames.Text.Plain,
                }
                .Concat(PortalMediaTypeNames.Rar)
                .Concat(PortalMediaTypeNames.Zip)
                .Concat(PortalMediaTypeNames.Md)
                .ToArray();

        public Validator()
        {
            RuleFor(x => x.FileSize)
                .LessThanOrEqualTo(s_oneMb * 10)
                .When(x => x.ContentType is MediaTypeNames.Image.Jpeg or MediaTypeNames.Image.Png)
                .WithMessage("File max size should not exceed 10 MB")
                .GreaterThan(0)
                .WithMessage("File size should be greater than 0 MB");

            RuleFor(x => x.ContentType)
                .Must(_allowedMediaTypes.Contains)
                .WithMessage(
                    "You can upload files only in PDF, TXT, DOC, DOCX, JPG, JPEG, PNG, XLS, XLSX, PPTX, MD, ZIP, RAR formats");
        }
    }
}
