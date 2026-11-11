using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.Comments.Requests;

namespace Modsen.Portal.Inquiries.Controllers.Controllers;

[Area("[Inquiries] Inquiry comments")]
[Route("api/inquiries/{inquiryId:int}/comments")]
internal class UserInquiryCommentsController : BaseApiController
{
    private readonly ISender _sender;

    public UserInquiryCommentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all comments for requested inquiry id
    /// Allowed Roles : all
    /// </summary>
    /// <param name="inquiryId">Inquiry id</param>
    /// <returns>List of UserInquiryCommentOutput</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserInquiryCommentOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<UserInquiryCommentOutput>>> GetAllCommentsByInquiryIdAsync(
        [FromRoute] int inquiryId)
    {
        var comments = await _sender.Send(new GetCommentsByInquiryIdRequest.Query(inquiryId), HttpContext.RequestAborted);

        var commentsOutput = comments.Select(UserInquiryCommentOutput.Of).ToList();

        return Ok(commentsOutput);
    }

    /// <summary>
    /// Create comment
    /// Allowed Roles : all
    /// </summary>
    /// <param name="inquiryId">Inquiry id in which the comment is created</param>
    /// <param name="commentInput">Comment input model for entity creating</param>
    /// <returns>UserInquiryCommentOutput</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserInquiryCommentOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<UserInquiryCommentOutput>> CreateCommentByInquiryIdAsync(
        [FromRoute] int inquiryId,
        [FromBody] UserInquiryCommentInput commentInput)
    {
        var comment = await _sender.Send(new CreateInquiryCommentRequest.Command(
            InquiryId: inquiryId,
            Comment: commentInput.Comment,
            NotifyApprovers: commentInput.NotifyApprovers ?? true,
            Files: commentInput.Files),
            HttpContext.RequestAborted);

        var commentOutput = UserInquiryCommentOutput.Of(comment);

        return Ok(commentOutput);
    }
}
