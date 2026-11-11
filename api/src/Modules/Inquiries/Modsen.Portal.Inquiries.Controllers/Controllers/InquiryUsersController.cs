using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Inquiries.Requests.InquiryUsers.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.InquiryUsers.Requests;

namespace Modsen.Portal.Inquiries.Controllers.Controllers;

[Area("[Inquiries] Inquiry users")]
[Route("api/inquiry-users")]
internal class InquiryUsersController : BaseApiController
{
    private readonly ISender _sender;

    public InquiryUsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of all inquiry users
    /// Allowed Roles : All
    /// </summary>
    /// <returns>A list of all inquiry users</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<InquiryUserLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewOrManageRequestsPolicy)]
    public async Task<ActionResult<List<InquiryUserLookupDto>>> GetInquiryUsersLookup()
    {
        var inquiryUsers = await _sender.Send(
            new GetInquiryUsersLookupRequest.Query(IsApprovals: false),
            HttpContext.RequestAborted);

        return Ok(inquiryUsers);
    }

    /// <summary>
    /// Get a list of all inquiry approvers users
    /// Allowed Roles : All
    /// </summary>
    /// <returns>A list of all inquiry approvers users</returns>
    [HttpGet("approvers/lookup")]
    [ProducesResponseType(typeof(List<InquiryUserLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAbsenceSettingsPolicy)]
    public async Task<ActionResult<List<InquiryUserLookupDto>>> GetInquiryApproversUsersLookup()
    {
        var inquiryUsers = await _sender.Send(
            new GetInquiryUsersLookupRequest.Query(IsApprovals: true),
            HttpContext.RequestAborted);

        return Ok(inquiryUsers);
    }
}
