using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Storage.Controllers.Helpers;
using Modsen.Portal.Storage.Controllers.Output;
using Modsen.Portal.Storage.Requests.Files.Requests;

namespace Modsen.Portal.Storage.Controllers;

[Area("[Storage]")]
[Route("api/[controller]")]
public class ApplicationFileController : BaseApiController
{
    private readonly ISender _sender;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public ApplicationFileController(ISender sender, IAuthenticatedUserContext authenticatedUserContext)
    {
        _sender = sender;
        _authenticatedUserContext = authenticatedUserContext;
    }

    /// <summary>
    /// Get Server file by id
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <param name="size">Size</param>
    /// <returns>File</returns>
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [HttpGet("{fileId}")]
    [AllowAnonymous]
    public async Task<ActionResult> GetFile([FromRoute] string fileId,
        [FromQuery] ApplicationFileSize size = ApplicationFileSize.Original)
    {
        var file = await _sender.Send(new GetFileRequest.Query(FileId: fileId, Size: size), HttpContext.RequestAborted);

        if (file.ContentStream is null)
        {
            return NotFound(new { fileId });
        }

        if (size != ApplicationFileSize.Original && file.IsFallbackToOriginal.HasValue && file.IsFallbackToOriginal.Value)
        {
            Response.Headers.CacheControl = "no-store, no-cache, must-revalidate, max-age=0";
            Response.Headers.Pragma = "no-cache";
            Response.Headers.Expires = "0";
            Response.Headers["X-Image-Fallback"] = "original";
        }
        else
        {
            Response.Headers.CacheControl = "public,max-age=31536000,immutable";
        }

        return File(file.ContentStream, file.ContentType, file.OriginalFileName);
    }

    /// <summary>
    /// Get payslip file by id.
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="fileId">File ID</param>
    /// <returns>File</returns>
    [HttpGet("{fileId}/payslip")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult> GetPayslipFile([FromRoute] string fileId)
    {
        var file = await _sender.Send(new GetPayslipFileRequest.Query(fileId), HttpContext.RequestAborted);

        return File(file.Content, file.ContentType);
    }

    /// <summary>
    /// Upload file to server
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="file">File form to upload</param>
    /// <param name="purpose">Purpose of the uploaded file (defines how the file will be processed and used</param>
    /// <returns>Uploaded file info</returns>
    [HttpPost]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    public async Task<ActionResult<ApplicationFileOutput>> UploadFile(IFormFile file,
        [FromQuery] ApplicationFilePurpose purpose = ApplicationFilePurpose.General)
    {
        ArgumentNullException.ThrowIfNull(file);

        var cancellationToken = HttpContext.RequestAborted;

        await using var memoryStream = new MemoryStream();
        await using var readStream = file.OpenReadStream();
        await readStream.CopyToAsync(memoryStream, cancellationToken);

        var fileContent = memoryStream.ToArray();

        var command = new UploadFileRequest.Command
        {
            FileContent = fileContent.ToArray(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = fileContent.Length,
            UserId = _authenticatedUserContext.GetRequiredUserId(),
            AvailableTill = null,
            FileSource = FileSource.User,
            Purpose = purpose
        };
        var response = await _sender.Send(command, cancellationToken);

        return Ok(new ApplicationFileOutput(response.FileId, response.FileSize, command.FileName));
    }

    /// <summary>
    /// Upload files to server.
    /// Allowed Roles: Authenticated user.
    /// Max total body size: 320 MB.
    /// Multipart form data supported only
    /// </summary>
    /// <returns>Uploaded file info</returns>
    [RequestSizeLimit(1024 * 1024 * 20 * 16)]
    [AllowAnonymous]
    [DisableFormValueModelBinding]
    [HttpPost("multiply")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ApplicationFileOutput>>> UploadFilesAsStream()
    {
        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        {
            throw new InvalidDataAppException("Invalid content type");
        }

        var boundary = MultipartRequestHelper.GetBoundary(Request.GetTypedHeaders().ContentType);

        var files = await _sender.Send(new UploadFilesRequest.Command(boundary, Request.Body, FileSource.User, ApplicationFilePurpose.General),
            HttpContext.RequestAborted);

        var output = files
            .Select(fileResult => new ApplicationFileOutput(
                fileResult.FileId,
                fileResult.FileSize,
                fileResult.FileName))
            .ToList();

        return Ok(output);
    }
}
