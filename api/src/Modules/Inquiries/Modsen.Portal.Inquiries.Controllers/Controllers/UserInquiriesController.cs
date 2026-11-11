using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

namespace Modsen.Portal.Inquiries.Controllers.Controllers;

[Area("[Inquiries] Inquiries")]
[Route("api/inquiries")]
internal class UserInquiriesController : BaseApiController
{
    private readonly ISender _sender;

    public UserInquiriesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get inquiries by user id and filters
    /// Allowed Roles : all except internt
    /// </summary>
    /// <param name="userId">User id for search</param>
    /// <param name="queryOptions">parameters for searching, ordering and paging</param>
    /// <returns>List of OdpUserInquiryOutput</returns>
    [HttpGet("~/api/users/{userId:int}/inquiries/search")]
    [ProducesResponseType(typeof(PageableListOfItems<UserInquiryCoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<UserInquiryCoreDto>>> SearchInquiriesAsync(
        [FromRoute] int userId,
        ODataQueryOptions<UserInquiryCoreDto> queryOptions)
    {
        var inquiries = await _sender.Send(
            new GetPageableInquiriesRequest.Query(userId, queryOptions),
            HttpContext.RequestAborted);

        return Ok(inquiries);
    }

    /// <summary>
    /// Get a list of inquiries
    /// Allowed Roles : all except internt
    /// Available by claim
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering and paging</param>
    /// <returns>List of UserInquiryOutput</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableListOfItems<InquiryCoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewOrManageRequestsPolicy)]
    public async Task<ActionResult<PageableListOfItems<InquiryCoreDto>>> GetInquiriesAsync(
        ODataQueryOptions<InquiryCoreDto> queryOptions)
    {
        var result = await _sender.Send(
            new GetInquiriesRequest.Query(queryOptions), HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get all inquiries for requested user id
    /// Allowed Roles : all except internt
    /// </summary>
    /// <param name="userId">ID for user</param>
    /// <returns>List of UserInquiryOutput</returns>
    [HttpGet("~/api/users/{userId:int}/inquiries")]
    [ProducesResponseType(typeof(List<UserInquiryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<UserInquiryOutput>>> GetInquiriesByUserIdAsync(
        [FromRoute] int userId)
    {
        var inquiries = await _sender.Send(new GetInquiriesByUserIdRequest.Query(userId), HttpContext.RequestAborted);

        var inquiriesOutput = inquiries.Select(UserInquiryOutput.Of).ToList();

        return Ok(inquiriesOutput);
    }

    /// <summary>
    /// Get inquiry by id
    /// Allowed Roles : all except internt
    /// </summary>
    /// <param name="inquiryId">Inquiry id</param>
    /// <returns>UserInquiryOutput</returns>
    [HttpGet("{inquiryId:int}")]
    [ProducesResponseType(typeof(List<UserInquiryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<UserInquiryOutput>>> GetInquiryByIdAsync(
        [FromRoute] int inquiryId)
    {
        var inquiry = await _sender.Send(new GetInquiryByIdRequest.Query(inquiryId), HttpContext.RequestAborted);

        var inquiryOutput = UserInquiryOutput.Of(inquiry);

        return Ok(inquiryOutput);
    }

    /// <summary>
    /// Get user leaves
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="top">Users count</param>
    /// <param name="skip">Skip users count</param>
    /// <param name="year">Year for search</param>
    /// <param name="month">Month for search</param>
    /// <param name="position">Position for search</param>
    /// <returns>List of LeaveOutput</returns>
    [HttpGet("~/api/users/leaves")]
    [ProducesResponseType(typeof(List<LeaveOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<LeaveOutput>>> GetLeavesAsync(
        [FromQuery] int top,
        [FromQuery] int skip,
        [FromQuery] int year,
        [FromQuery] int? month,
        [FromQuery] string? position)
    {
        var leaves = await _sender.Send(new GetUserLeavesRequest.Query
        (
            Top: top,
            Skip: skip,
            Year: year,
            Month: month,
            Position: position
        ), HttpContext.RequestAborted);

        var leavesOutput = leaves.Select(LeaveOutput.Of).ToList();

        return Ok(leavesOutput);
    }

    /// <summary>
    /// Get approved absences for current user's scope
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <returns>List of ApprovedAbsenceOutput</returns>
    [HttpGet("approved-absences")]
    [ProducesResponseType(typeof(List<ApprovedAbsenceOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<ApprovedAbsenceOutput>>> GetApprovedAbsencesAsync([FromQuery] string? departmentId)
    {
        var absences = await _sender.Send(new GetApprovedAbsencesRequest.Query(departmentId), HttpContext.RequestAborted);
        var output = absences.Select(ApprovedAbsenceOutput.Of).ToList();

        return Ok(output);
    }

    /// <summary>
    /// Create inquiry
    /// Allowed Roles : all except internt
    /// </summary>
    /// <param name="userId">User id for whom the request is being created</param>
    /// <param name="input">Inquiry input model for entity creating</param>
    /// <returns>UserInquiryOutput</returns>
    [HttpPost("~/api/users/{userId:int}/inquiries")]
    [ProducesResponseType(typeof(UserInquiryCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<UserInquiryCoreDto>> CreateUserInquiryAsync(
        [FromRoute] int userId,
        [FromBody] UserInquiryInput input)
    {
        var result = await _sender.Send(new CreateUserInquiryAgreementRequest.Command
            {
                UserId = userId,
                InquiryTemplateId = input.InquiryTemplateId,
                AgreementTemplateId = input.AgreementTemplateId,
                Hours = input.Hours,
                Notes = input.Notes,
                DateFrom = input.DateFrom,
                DateTo = input.DateTo,
                Files = input.Files
            },
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Update inquiry
    /// Allowed Roles : all except internt
    /// </summary>
    /// <param name="inquiryId">Inquiry id</param>
    /// <param name="updateUserInquiryInput">Inquiry input model for entity updating</param>
    /// <returns>UserInquiryOutput</returns>
    [HttpPut("{inquiryId:int}")]
    [ProducesResponseType(typeof(List<UserInquiryOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<UserInquiryOutput>> UpdateInquiryAsync(
        [FromRoute] int inquiryId,
        [FromBody] UpdateUserInquiryInput updateUserInquiryInput)
    {
        var inquiry = await _sender.Send(new UpdateUserInquiryRequest.Command
        {
            InquiryId = inquiryId,
            Notes = updateUserInquiryInput.Notes,
            DateFrom = updateUserInquiryInput.DateFrom,
            DateTo = updateUserInquiryInput.DateTo
        }, HttpContext.RequestAborted);

        var inquiryOutput = UserInquiryOutput.Of(inquiry);

        return Ok(inquiryOutput);
    }

    /// <summary>
    /// Makes the request status cancelled
    /// Allowed Roles : all except internt
    /// </summary>
    /// <param name="inquiryId">Inquiry id for deleting</param>
    /// <returns>UserInquiryOutput</returns>
    [HttpDelete("{inquiryId:int}")]
    [ProducesResponseType(typeof(UserInquiryOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<UserInquiryOutput>> DeleteInquiryAsync([FromRoute] int inquiryId)
    {
        var cancelledInquiry = await _sender.Send(new DeleteUserInquiryRequest.Command(inquiryId),
            HttpContext.RequestAborted);

        var inquiryOutput = UserInquiryOutput.Of(cancelledInquiry);

        return Ok(inquiryOutput);
    }

    /// <summary>
    /// Get selected days
    /// Allowed Roles : all except internt
    /// </summary>
    /// <param name="selectedDaysInput">Selected day filter input</param>
    /// <returns>Selected days</returns>
    [HttpGet("selected-days")]
    [ProducesResponseType(typeof(SelectedDaysOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<int>> GetAgreementRemainingDaysAsync(
        [FromQuery] SelectedDaysInput selectedDaysInput)
    {
        var response = await _sender.Send(
            new GetInquirySelectedDaysRequest.Query(
                selectedDaysInput.TemplateId,
                selectedDaysInput.UserId,
                selectedDaysInput.DateFrom,
                selectedDaysInput.DateTo),
            HttpContext.RequestAborted);

        return Ok(new SelectedDaysOutput(response));
    }

    /// <summary>
    /// Get inquiries grouped by users
    /// Allowed : Users with one of claims (ManageAllRequests, ManageRequests, ViewAllRequests)
    /// </summary>
    /// <param name="queryOptions"></param>
    /// <param name="year">in year</param>
    /// <param name="month">in month</param>
    /// <param name="templateId">filter</param>
    /// <returns>Grouped inquiries</returns>
    [HttpGet("calendar")]
    [ProducesResponseType(typeof(PageableListOfItems<OdpUserInquiryCalendarEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewOrManageRequestsPolicy)]
    public async Task<ActionResult<PageableListOfItems<OdpUserInquiryCalendarEntryDto>>> GetUserInquiriesCalendarAsync(
        ODataQueryOptions<OdpUserInquiryCalendarEntryDto> queryOptions,
        [FromQuery] int year, [FromQuery] int month, [FromQuery] int? templateId)
    {
        var response = await _sender.Send(new GetOdpInquiryCalendarEntriesRequest.Query(queryOptions, year, month, templateId),
            HttpContext.RequestAborted);

        return Ok(response);
    }
}
