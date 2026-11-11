using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests.RequestDtos;

namespace Modsen.Portal.Inquiries.Controllers.Controllers;

[Area("[Inquiries] Inquiry accepts")]
[Route("api/inquiries/{inquiryId:int}/accepts")]
internal class UserInquiryAcceptsController : BaseApiController
{
    private readonly ISender _sender;

    public UserInquiryAcceptsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all accepts for requested inquiry id
    /// Allowed Roles : all
    /// </summary>
    /// <param name="inquiryId">Inquiry id</param>
    /// <returns>List of UserInquiryAcceptOutput</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserInquiryAcceptOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<UserInquiryAcceptOutput>>> GetAllAcceptsByInquiryIdAsync(
        [FromRoute] int inquiryId)
    {
        var inquiryAccepts = await _sender.Send(new GetInquiryAcceptsByInquiryIdRequest.Query(inquiryId),
            HttpContext.RequestAborted);

        var inquiryAcceptsOutput = inquiryAccepts.Select(UserInquiryAcceptOutput.Of).ToList();

        return Ok(inquiryAcceptsOutput);
    }

    /// <summary>
    /// Accept inquiry
    /// Allowed Roles : all
    /// </summary>
    /// <param name="inquiryId">Inquiry id</param>
    /// <returns>UserInquiryAcceptOutput</returns>
    [HttpPost("~/api/inquiries/{inquiryId:int}/accept")]
    [ProducesResponseType(typeof(UserInquiryAcceptOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<UserInquiryAcceptOutput>>> AcceptInquiryAsync(
        [FromRoute] int inquiryId)
    {
        var inquiryAccepts = await _sender.Send(new AcceptInquiryRequest.Command(inquiryId),
            HttpContext.RequestAborted);

        var inquiryAcceptOutput = inquiryAccepts.Select(UserInquiryAcceptOutput.Of).ToList();

        return Ok(inquiryAcceptOutput);
    }

    /// <summary>
    /// Reject inquiry
    /// Allowed Roles : all
    /// </summary>
    /// <param name="inquiryId">Inquiry id</param>
    /// <returns>UserInquiryAcceptOutput</returns>
    [HttpPost("~/api/inquiries/{inquiryId:int}/reject")]
    [ProducesResponseType(typeof(UserInquiryAcceptOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<UserInquiryAcceptOutput>>> RejectInquiryAsync(
        [FromRoute] int inquiryId)
    {
        var inquiryAccepts = await _sender.Send(new RejectInquiryRequest.Command(inquiryId),
            HttpContext.RequestAborted);

        var inquiryAcceptOutput = inquiryAccepts.Select(UserInquiryAcceptOutput.Of).ToList();

        return Ok(inquiryAcceptOutput);
    }

    /// <summary>
    /// Get requests that current user should approve or reject now
    /// Allowed Roles : all except Employee and SystemAdministrator
    /// </summary>
    /// <returns>List of PendingInquiryApprovalDto</returns>
    [HttpGet("~/api/inquiries/accepts/pending")]
    [ProducesResponseType(typeof(List<PendingInquiryApprovalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadGmSysAdminOnlyPolicyName.Name)]
    public async Task<ActionResult<List<PendingInquiryApprovalDto>>> GetPendingApprovalsAsync(
        [FromQuery] string? departmentId)
    {
        var pendingApprovals = await _sender.Send(new GetPendingInquiryApprovalsRequest.Query(departmentId),
            HttpContext.RequestAborted);

        return Ok(pendingApprovals);
    }

    /// <summary>
    /// Send notification to inquiry approver
    /// Allowed Roles : all
    /// </summary>
    /// <param name="inquiryId">Inquiry id</param>
    /// <param name="inquiryAcceptId">Inquiry accept id</param>
    /// <returns>Nothing</returns>
    [HttpPost("{inquiryAcceptId:int}/notifications")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult> NotifyInquiryApprover(
        [FromRoute] int inquiryId,
        [FromRoute] int inquiryAcceptId)
    {
        await _sender.Send(new NotifyInquiryApproverRequest.Command(inquiryId, inquiryAcceptId),
            HttpContext.RequestAborted);

        return NoContent();
    }
}
