using System.Net.Mime;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.Storage.Requests.Files.Core.Composers;

namespace Modsen.Portal.Storage.Requests.Files.Requests;

internal sealed class UploadFilesRequest : ICommandHandler<UploadFilesRequest.Command, List<UploadFilesRequest.UploadedFileResult>>
{
    private readonly IUploadFileStreamComposer _uploadFileStreamComposer;
    private readonly IStorageDbContext _storageDbContext;

    public UploadFilesRequest(IUploadFileStreamComposer uploadFileStreamComposer,
        IStorageDbContext storageDbContext)
    {
        _uploadFileStreamComposer = uploadFileStreamComposer;
        _storageDbContext = storageDbContext;
    }

    public async Task<List<UploadedFileResult>> Handle(Command request, CancellationToken cancellationToken)
    {
        var uploadedFiles = new List<UploadedFileResult>();
        var reader = new MultipartReader(request.Boundary, request.ContentStream);
        MultipartSection? section;

        while ((section = await reader.ReadNextSectionAsync(cancellationToken)) != null)
        {
            var contentDisposition = section.GetContentDispositionHeader();

            if (contentDisposition is not null && contentDisposition.IsFileDisposition())
            {
                var fileName = contentDisposition.FileName.Value
                    ?? throw new InvalidDataAppException(PortalMessages.FileNameIsMissing);

                var contentType = GetNormalizedContentType(fileName, section.ContentType);

                ValidateFile(contentType);

                using var ms = new MemoryStream();
                await section.Body.CopyToAsync(ms, cancellationToken);
                ms.Position = 0;

                var applicationFile = await _uploadFileStreamComposer.ComposeAsync(
                    ms,
                    fileName!,
                    contentType!,
                    request.FileSource,
                    request.FilePurpose,
                    cancellationToken);

                _storageDbContext.ApplicationFiles.Add(applicationFile);

                uploadedFiles.Add(new UploadedFileResult(
                    FileId: applicationFile.FileId,
                    FileSize: ms.Length,
                    FileName: fileName));
            }
            else if (contentDisposition is not null && contentDisposition.IsFormDisposition())
            {
                using var streamReader = new StreamReader(section.Body);
                await streamReader.ReadToEndAsync(cancellationToken);
            }
        }

        await _storageDbContext.SaveChangesAsync(cancellationToken);

        return uploadedFiles;
    }

    private static void ValidateFile(string? contentType)
    {
        var allowedMediaTypes =
            new[] {
                MediaTypeNames.Image.Png,
                MediaTypeNames.Image.Jpeg,
                MediaTypeNames.Application.Pdf,
                PortalMediaTypeNames.Word,
                PortalMediaTypeNames.WordX,
                PortalMediaTypeNames.PowerPointX,
                PortalMediaTypeNames.Excel,
                PortalMediaTypeNames.ExcelX,
                MediaTypeNames.Text.Plain,
                MediaTypeNames.Text.Markdown,
            }
            .Concat(PortalMediaTypeNames.Rar)
            .Concat(PortalMediaTypeNames.Zip)
            .Concat(PortalMediaTypeNames.Md)
            .ToArray();

        var isSupportedContentType = allowedMediaTypes.Contains(contentType);

        if (!isSupportedContentType)
        {
            throw new InvalidDataAppException("You can upload files only in PDF, TXT, DOC, DOCX, JPG, JPEG, PNG, XLS, XLSX, PPTX, MD, ZIP, RAR formats");
        }
    }

    private static string GetNormalizedContentType(string fileName, string? contentType)
    {
        var extension = Path.GetExtension(fileName).ToUpperInvariant();

        return extension switch
        {
            ".RAR" => PortalMediaTypeNames.VndRar,
            _ => contentType ?? MediaTypeNames.Application.Octet
        };
    }

    internal sealed record Command(
        string Boundary,
        Stream ContentStream,
        FileSource FileSource,
        ApplicationFilePurpose FilePurpose) : ICommand<List<UploadedFileResult>>, ITransactionalRequest;

    public record UploadedFileResult(string FileId, long FileSize, string? FileName);
}
