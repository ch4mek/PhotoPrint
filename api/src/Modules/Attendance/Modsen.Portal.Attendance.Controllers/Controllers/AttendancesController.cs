using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Attendance.DataAccess.Npgsql.OData;
using Modsen.Portal.Attendance.Requests.Attendances;
using Modsen.Portal.Attendance.Requests.Attendances.Requests;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.Attendance.Controllers.Controllers;

[Area("[Attendance] Attendance")]
[Route("api/attendances")]
internal sealed class AttendancesController : BaseApiController
{
    private readonly ISender _sender;

    public AttendancesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get user attendances by user id.
    /// Allowed roles: CEO, CTO, HeadOfDepartment, GroupManager and Employee for self-access
    /// </summary>
    [HttpGet("/api/users/{userId:int}/attendances")]
    [ProducesResponseType(typeof(PageableListOfItems<UserAttendanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadGmEmployeeOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<UserAttendanceDto>>> GetUserAttendancesAsync(
        int userId,
        ODataQueryOptions<OdpUserAttendanceRow> queryOptions,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUserAttendancesRequest.Query(userId, queryOptions), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get detailed attendances. Allowed roles: CEO, CTO, HeadOfDepartment and GroupManager
    /// </summary>
    /// <param name="queryOptions">Parameters for filtering, searching, ordering</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Pageable list of attendances</returns>
    [HttpGet("detailed")]
    [ProducesResponseType(typeof(PageableListOfItems<OdpAttendance>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpAttendance>>> GetDetailedAttendancesAsync(
        ODataQueryOptions<OdpAttendance> queryOptions,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetDetailedAttendancesRequest.Query(queryOptions), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get total attendances by user and date. Allowed roles: CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="queryOptions"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Pageable list of total attendances</returns>
    [HttpGet("total")]
    [ProducesResponseType(typeof(PageableListOfItems<OdpAttendanceTotal>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpAttendanceTotal>>> GetTotalAttendancesAsync(
        ODataQueryOptions<OdpAttendanceTotal> queryOptions, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetOdpAttendancesTotalRequest.Query(queryOptions), cancellationToken);

        return Ok(result);
    }
}
