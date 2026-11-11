using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests;
using Modsen.Portal.TeamBoard.Requests.Interviews.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Interviews")]
[Route("api/interviews")]
internal class InterviewsController(ISender sender) : BaseApiController
{
    [HttpGet("search")]
    [ProducesResponseType(typeof(PageableListOfItems<OdpInterviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageInterviewsPolicy)]
    public async Task<ActionResult<PageableListOfItems<OdpInterviewDto>>> GetPageableInterviewsAsync(
        ODataQueryOptions<OdpInterview> queryOptions)
    {
        var result = await sender.Send(new GetOdpInterviewsRequest.Query(queryOptions), HttpContext.RequestAborted);

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserInterviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageInterviewsPolicy)]
    public async Task<ActionResult<UserInterviewDto>> CreateInterviewAsync(
        [FromBody] CreateOrUpdateInterviewInput input)
    {
        var result = await sender.Send(new CreateInterviewRequest.Command(
            input.UserId,
            input.Specialization,
            input.Client,
            input.JiraTask,
            input.Type,
            input.Status,
            input.InterviewDate,
            input.InterviewTimeFromUtc,
            input.SaleManagerId,
            input.ReasonForRefusal,
            input.Notes
            ), HttpContext.RequestAborted);

        return Ok(result);
    }

    [HttpPost("groups/{groupId:Guid}")]
    [ProducesResponseType(typeof(UserInterviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageInterviewsPolicy)]
    public async Task<ActionResult<UserInterviewDto>> CreateInterviewInGroupAsync(
        [FromRoute] Guid groupId, [FromBody] CreateOrUpdateInterviewInput input)
    {
        var result = await sender.Send(new CreateInterviewInGroupRequest.Command(
            input.UserId,
            input.Specialization,
            input.Client,
            input.JiraTask,
            input.Type,
            input.Status,
            input.InterviewDate,
            input.InterviewTimeFromUtc,
            input.SaleManagerId,
            input.ReasonForRefusal,
            input.Notes,
            groupId
            ), HttpContext.RequestAborted);

        return Ok(result);
    }

    [HttpPut("{interviewId}")]
    [ProducesResponseType(typeof(UserInterviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageInterviewsPolicy)]
    public async Task<ActionResult<UserInterviewDto>> UpdateInterviewAsync(
        [FromRoute] int interviewId, [FromBody] CreateOrUpdateInterviewInput input)
    {
        var result = await sender.Send(new UpdateInterviewRequest.Command(
            interviewId,
            input.UserId,
            input.Specialization,
            input.Client,
            input.JiraTask,
            input.Type,
            input.Status,
            input.InterviewDate,
            input.InterviewTimeFromUtc,
            input.SaleManagerId,
            input.ReasonForRefusal,
            input.Notes
            ), HttpContext.RequestAborted);

        return Ok(result);
    }

    [HttpDelete("{interviewId}")]
    [ProducesResponseType(typeof(UserInterviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageInterviewsPolicy)]
    public async Task<ActionResult<UserInterviewDto>> DeleteInterviewStageAsync(
        [FromRoute] int interviewId)
    {
        var result = await sender.Send(new DeleteInterviewRequest.Command(interviewId), HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Access employees related interviews on dashboard for current day
    /// Allowed Roles : Managers
    /// </summary>
    /// <returns>List of employees related interviews</returns>
    [HttpGet("employees-interviews")]
    [ProducesResponseType(typeof(ICollection<EmployeeInterviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewDashboardPolicy)]
    public async Task<ActionResult<ICollection<EmployeeInterviewDto>>> GetEmployeesInterviews([FromQuery] string? departmentId)
    {
        var employeesInterviewDtos = await sender.Send(new GetEmployeesInterviewsRequest.Query
        {
            DepartmentId = departmentId
        });

        return Ok(employeesInterviewDtos);
    }
}
