using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.Files.Requests;

namespace Modsen.Portal.Inquiries.Controllers.Controllers;

[Area("[Inquiries] Inquiry attachments")]
[Route("api/inquiries/{inquiryId:int}/attachments")]
internal class UserInquiryFilesController : BaseApiController
{
    private readonly ISender _sender;

    public UserInquiryFilesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all attachments for requested inquiry id
    /// Allowed Roles : all
    /// </summary>
    /// <param name="inquiryId">Inquiry id</param>
    /// <returns>List of UserInquiryFileOutput</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserInquiryFileOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<UserInquiryFileOutput>>> GetAllFilesByInquiryIdAsync(
      [FromRoute] int inquiryId)
    {
        var files = await _sender.Send(new GetFilesByInquiryIdRequest.Query(inquiryId), HttpContext.RequestAborted);

        var filesOutput = files.Select(UserInquiryFileOutput.Of).ToList();

        return Ok(filesOutput);
    }

    /// <summary>
    /// Create attachment 
    /// Allowed Roles : all
    /// </summary>
    /// <param name="inquiryId">Inquiry id in which the file is created</param>
    /// <param name="fileInput">File input model for entity creating</param>
    /// <returns>UserInquiryFileOutput</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserInquiryFileOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<UserInquiryFileOutput>> CreateFileByInquiryIdAsync(
        [FromRoute] int inquiryId,
        [FromBody] UserInquiryFileInput fileInput)
    {
        var file = await _sender.Send(new CreateInquiryFileRequest.Command(
            InquiryId: inquiryId,
            FileId: fileInput.FileId),
            HttpContext.RequestAborted);

        var fileOutput = UserInquiryFileOutput.Of(file);

        return Ok(fileOutput);
    }

    /// <summary>
    /// Delete attachment
    /// Allowed Roles : all
    /// </summary>
    /// <param name="inquiryId">Inquiry id in which the file is deleted</param>
    /// <param name="attachmentId">File id for deleting</param>
    [HttpDelete("{attachmentId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult> DeleteFileAsync(
        [FromRoute] int inquiryId,
        [FromRoute] int attachmentId)
    {
        await _sender.Send(new DeleteInquiryFileRequest.Command(
            InquiryId: inquiryId,
            Id: attachmentId),
            HttpContext.RequestAborted);

        return Ok();
    }
}
