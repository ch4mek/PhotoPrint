using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.Controllers.Controllers;

[Area("[TimeTrack] Timesheet")]
internal class TimesheetController : BaseApiController
{
    private readonly ISender _sender;

    public TimesheetController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get timesheet for user by id using Odata for filtering, sorting and pagination
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="queryOptions">Odata options class</param>
    /// <param name="userId">User ID</param>
    /// <param name="onlyManagedProjects">Flag for only managed projects</param>
    /// <param name="isPersonalMode">Personal mode</param>
    /// <returns>Response with list of timesheets , with count of returned items and total count of items after odata filtration</returns>
    [HttpGet("{userId:int}")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<UserTimesheetsWithBlockedDaysDto?>> GetTimesheetsByUserId(
        ODataQueryOptions<TimesheetOdpDto> queryOptions,
        [FromRoute] int userId,
        [FromQuery] bool? onlyManagedProjects,
        [FromQuery] bool isPersonalMode)
    {
        var response = await _sender.Send(
            new GetTimesheetByUserIdRequest.Query(queryOptions, userId, onlyManagedProjects, isPersonalMode),
            HttpContext.RequestAborted);

        return Ok(response);
    }

    /// <summary>
    /// Get timesheets using filters pagination.
    /// Allowed Roles: CEO, CTO, Head Of Department, Employee, Group Manager
    /// </summary>
    /// <param name="input">filters for searching and paging timesheets</param>
    /// <returns>Response with list of timesheets , with count of returned items and total count of items after odata filtration</returns>
    [HttpGet]
    [Authorize(Policy = ChiefHeadGmEmployeeOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableUserTimesheets>> GetTimesheets(
        [FromQuery] GetTimesheetsInput input)
    {
        var getTimesheetsDto = new GetTimesheetsDto
        {
            Top = input.Top.GetValueOrDefault(),
            Skip = input.Skip.GetValueOrDefault(),
            ReportForDateFrom = input.ReportForDateFrom,
            ReportForDateTo = input.ReportForDateTo,
            DepartmentId = input.DepartmentId,
            ResourceGroupId = input.ResourceGroupId,
            ClientId = input.ClientId,
            ProjectId = input.ProjectId,
            UserId = input.UserId,
            OnlyManagedProjects = input.OnlyManagedProjects,
            AssignedForSelectedDate = input.AssignedForSelectedDate,
        };

        var response = await _sender.Send(
            new GetTimesheetsRequest.Query(getTimesheetsDto),
            HttpContext.RequestAborted);

        return Ok(response);
    }

    /// <summary>
    /// Create timesheet.
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="timesheet">Timesheet model for creating</param>
    /// <returns>Created Timesheet</returns>
    [HttpPost("/api/Timesheet/fact/upsert")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<TimesheetCoreDto> UpsertFact(UpsertFactDto timesheet)
    {
        var result = await _sender.Send(
            new CreateOrUpdateTimesheet.Command(timesheet),
            HttpContext.RequestAborted);

        return result.Model;
    }

    /// <summary>
    /// Create timesheet.
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="timesheet">Timesheet model for creating</param>
    /// <returns>Created Timesheet</returns>
    [HttpPost("/api/Timesheet/billable/upsert")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<TimesheetCoreDto> UpsertBillable(UpsertBillableDto timesheet)
    {
        var result = await _sender.Send(
            new CreateOrUpdateTimesheet.Command(timesheet),
            HttpContext.RequestAborted);

        return result.Model;
    }

    /// <summary>
    /// Delete timesheet.
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="id">Timesheet ID</param>
    /// <returns>Removed Timesheet</returns>
    [HttpDelete("{id:int}/fact")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<TimesheetDto>> DeleteFactTimesheet(int id)
    {
        var response = await _sender.Send(new DeleteTimesheetRequest.Command(id, false), HttpContext.RequestAborted);
        return Ok(response);
    }

    /// <summary>
    /// Delete timesheet.
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="id">Timesheet ID</param>
    /// <returns>Removed Timesheet</returns>
    [HttpDelete("{id:int}/billable")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<TimesheetDto>> DeleteBillableTimesheet(int id)
    {
        var response = await _sender.Send(new DeleteTimesheetRequest.Command(id, true), HttpContext.RequestAborted);
        return Ok(response);
    }


    /// <summary>
    /// Update task name.
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="model">id of timesheet and new task name</param>
    /// <returns>New task name and count of updated timesheets</returns>
    [HttpPatch("taskName")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<UpdateTaskNameResultDto>> UpdateTaskName(UpdateTaskNameDto model)
    {
        var response = await _sender.Send(new UpdateTaskNameRequest.Command(model), HttpContext.RequestAborted);
        return Ok(response);
    }

    /// <summary>
    /// Get submit status.
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="userId">Get submit status for user with this userId</param>
    /// <param name="month">Get timesheets for this month</param>
    /// <param name="year">Update timesheets for this year</param>
    /// <returns>GetStatusOutput</returns>
    [HttpGet]
    [Route("~/api/users/{userId:int}/timesheets/status")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<StatusOutput>> GetUserSubmitStatusAsync(
        [FromRoute] int userId,
        [FromQuery] int month,
        [FromQuery] int year)
    {
        var response = await _sender.Send(
                new GetUserSubmitStatusRequest.Query(userId, month, year),
                HttpContext.RequestAborted);

        return Ok(new StatusOutput(userId, response));
    }

    /// <summary>
    /// Update submit status for timesheets.
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="userId">Update submit status for user with this userId</param>
    /// <param name="month">Update status for this month</param>
    /// <param name="year">Update status for this year</param>
    /// <param name="input">Object contains status to update</param>
    /// <returns>Updated submit status and user id</returns>
    [HttpPut]
    [Route("~/api/users/{userId:int}/timesheets/status")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<StatusOutput>> CreateOrUpdateUserSubmitStatusAsync(
        [FromRoute] int userId,
        [FromQuery] int month,
        [FromQuery] int year,
        [FromBody] UpdateStatusInput input)
    {
        var submitStatus = await _sender.Send(
                new UpdateUserSubmitStatusRequest.Command(new UpdateUserSubmitStatusDto(userId, input.Status, month, year)),
                HttpContext.RequestAborted);

        return Ok(new StatusOutput(userId, submitStatus));
    }

    /// <summary>
    /// Get timesheet notification status.
    /// Allowed Roles: User with claim ViewHome
    /// </summary>
    /// <param name="forDate">current date</param>
    /// <returns>UserTimesheetStatusDto</returns>
    [HttpGet]
    [Route("~/api/users/timesheets/calendar")]
    [Authorize(Policy = PortalPolicyNames.ViewHomePolicy)]
    public async Task<IActionResult> GetNotificationsAsync(
        [FromQuery] DateOnly forDate)
    {
        var response = await _sender.Send(
                new GetStatusWithPreviousTasks.Query(forDate),
                HttpContext.RequestAborted);

        return Ok(response);
    }
}
