using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Authentication.UserEventsKey;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;

namespace Modsen.Portal.Inquiries.Controllers.Controllers;

[Area("[Inquiries] Inquiries assigns")]
[Route("api/inquiries/assigns")]
internal class InquiryAssignController : BaseApiController
{
    private readonly ISender _sender;

    public InquiryAssignController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create assigns
    /// Allowed Roles : CEO, CTO, users with HR module claim (only unassign)
    /// </summary>
    /// <returns>Assigns Ids</returns>
    [HttpPost]
    [ProducesResponseType(typeof(InquiriesAssignsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAssignsPolicy)]
    public async Task<ActionResult<InquiriesAssignsDto>> CreateInquiryAssigns(
        [FromBody] CreateInquiryAssignInput input)
    {
        var result = await _sender.Send(
            new CreateInquiriesAssignsRequest.Command(
                input.AssigneUserIds,
                input.UnassigneUserIds,
                input.InquiryTemplateId,
                input.AgreementId,
                input.EffectiveFrom),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get user current inquiry assigns
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="userId">ID of user</param>
    /// <returns>List of UserAssignRequestDto</returns>
    [HttpGet("~/api/users/{userId:int}/inquiries/assigns")]
    [ProducesResponseType(typeof(List<UserAssignRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<UserAssignRequestDto>>> GetInquiryAssignsByUserIdAsync(
        [FromRoute] int userId)
    {
        var currentAssigns = await _sender.Send(
            new GetUserAssignsRequest.Query(userId),
            HttpContext.RequestAborted);

        return Ok(currentAssigns);
    }

    /// <summary>
    /// Get first active users' agreement assigns
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="id">ID of agreement</param>
    /// <param name="queryOptions">OData query options</param>
    /// <returns></returns>
    [HttpGet("~/api/agreements/{id:int}/assigns/search")]
    [ProducesResponseType(typeof(List<UserAssignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAbsenceSettingsPolicy)]
    public async Task<ActionResult<PageableListOfItems<UserAgreementAssignCoreDto>>> GetAssignsByAgreementIdAsync(
        [FromRoute] int id,
        ODataQueryOptions<UserAgreementAssignCoreDto> queryOptions)
    {
        var pageableUsers = await _sender.Send(new GetAssignsByAgreementRequest.Query(id, queryOptions),
            HttpContext.RequestAborted);

        return Ok(pageableUsers);
    }

    /// <summary>
    /// Get templates assigns
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="id">ID of template</param>
    /// <param name="queryOptions">OData query options</param>
    /// <returns></returns>
    [HttpGet("~/api/templates/{id:int}/assigns/search")]
    [ProducesResponseType(typeof(List<UserAssignDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAbsenceSettingsPolicy)]
    public async Task<ActionResult<PageableListOfItems<UserAssignDto>>> GetAssignsByTemplateIdAsync(
        [FromRoute] int id,
        ODataQueryOptions<AssignedUserDto> queryOptions)
    {
        var pageableUsers = await _sender.Send(new GetAssignsByTemplateRequest.Query(id, queryOptions),
            HttpContext.RequestAborted);

        return Ok(pageableUsers);
    }

    /// <summary>
    /// Get users templates lookup
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <returns></returns>
    [HttpPost("~/api/users/templates/lookup")]
    [ProducesResponseType(typeof(TemplateUsersLookupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageAbsenceSettingsPolicy)]
    public async Task<ActionResult<TemplateUsersLookupDto>> GetUserByTemplateLookup(
        [FromBody] UserAssignmentLookupInput input)
    {
        var pageableUsers = await _sender.Send(
            new GetUsersByTemplateLookupRequest.Query(
                input.TemplateId,
                input.AgreementId,
                input.FullName),
            HttpContext.RequestAborted);

        return Ok(pageableUsers);
    }

    /// <summary>
    /// Get users inquiry balances
    /// Access via API key 
    /// </summary>
    /// <returns>List of user inquiry template balances</returns>
    [HttpGet("~/api/users/{userId:int}/inquiries/balances")]
    [ServiceFilter(typeof(UserEventsApiKeyHeaderAttribute))]
    [ProducesResponseType(typeof(List<UserInquiryBalance>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<ActionResult<List<UserInquiryBalance>>> GetUserInquiryBalancesAsync(
        [FromRoute] int userId)
    {
        var currentAssigns = await _sender.Send(
            new GetUserInquiryBalancesRequest.Query(userId),
            HttpContext.RequestAborted);

        return Ok(currentAssigns);
    }
}
