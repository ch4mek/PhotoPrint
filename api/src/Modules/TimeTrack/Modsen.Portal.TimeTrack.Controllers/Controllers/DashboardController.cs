using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TimeTrack.Domain;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

namespace Modsen.Portal.TimeTrack.Controllers.Controllers;

[Area("[TimeTrack] Dashboard")]
public class DashboardController : BaseApiController
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get Dashboard for current user and current date
    /// Allowing roles: HeadOfDepartment, Employee, SystemAdministrator, GroupManager
    /// </summary>
    /// <param name="queryOptions">Filters</param>
    /// <returns>Dashboard</returns>
    [ProducesResponseType(typeof(DashboardCurrentUserCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Authorize(Policy = HeadGmSysAdminEmployeeOnlyPolicyName.Name)]
    public async Task<DashboardCurrentUserCoreDto> GetDashboardForCurrent(ODataQueryOptions<DashboardActivity> queryOptions)
    {
        return await _sender.Send(new GetDashboardCurrentUserRequest.Query(queryOptions), HttpContext.RequestAborted);
    }

    /// <summary>
    /// Get tracked time by current user without pagination
    /// Allowing roles: HeadOfDepartment, Employee, SystemAdministrator, GroupManager
    /// </summary>
    /// <returns>Tracked time without pagination</returns>
    [ProducesResponseType(typeof(DashboardCurrentUserAllTrackedHoursCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet("tracked-time")]
    [Authorize(Policy = HeadGmSysAdminEmployeeOnlyPolicyName.Name)]
    public async Task<DashboardCurrentUserAllTrackedHoursCoreDto> GetDashboardCurrentUserAllTrackedHours()
    {
        return await _sender.Send(new GetDashboardCurrentUserAllTrackedHoursRequest.Query(), HttpContext.RequestAborted);
    }

    /// <summary>
    /// Get dashboard info for specified period for manager ( for HeadOfDepartment - only for his department)
    /// Allowed Roles: CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="year">year of period</param>
    /// <param name="monthOfYear">month of period</param>
    /// <param name="weekOfYear">week of period</param>
    /// <param name="departmentId">Search Department</param>
    /// <returns>Dashboard Head Model for specified period</returns>
    [ProducesResponseType(typeof(DashboardOverviewCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("overview")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<DashboardOverviewCoreDto> GetOverviewAsync(int year, [FromQuery] int[] monthOfYear, int? weekOfYear, string? departmentId)
    {
        return await _sender.Send(new GetDashboardOverviewRequest.Query(year, monthOfYear, weekOfYear, departmentId), HttpContext.RequestAborted);
    }

    /// <summary>
    /// Get Users Info for specified period for manager ( for HeadOfDepartment - only for his department)
    /// Allowed Roles: CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="year">year of period</param>
    /// <param name="monthOfYear">month of period</param>
    /// <param name="page">number of pagination page </param>
    /// <param name="rowsCount">count of elements of pagination page</param>
    /// <param name="departmentId">Search Department</param>
    /// <returns>List of users info with pagination properties for specified period</returns>
    [ProducesResponseType(typeof(DashboardUsersCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("users")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<DashboardUsersCoreDto> GetUsersAsync(int year, [FromQuery] int[] monthOfYear, int page, int rowsCount, string? departmentId)
    {
        return await _sender.Send(new GetDashboardUsersRequest.Query(year, monthOfYear, page, rowsCount, departmentId), HttpContext.RequestAborted);
    }

    /// <summary>
    /// Get Projects Info for specified period and specific user for manager ( for HeadOfDepartment - only for his department)
    /// Allowed Roles: CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="year">Year of period</param>
    /// <param name="monthOfYear">Month of period</param>
    /// <param name="departmentId">Search Department</param>
    /// <returns>List of user projects for specified period</returns>
    [ProducesResponseType(typeof(List<DashboardOfUserProjectsCoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("users/{userId:int}")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<List<DashboardOfUserProjectsCoreDto>> GetUserProjectsAsync(int userId, int year, [FromQuery] int[] monthOfYear, string? departmentId)
    {
        return await _sender.Send(new GetDashboardOfUserProjectsRequest.Query(userId, year, monthOfYear, departmentId), HttpContext.RequestAborted);
    }

    /// <summary>
    /// Get Tasks Info for specified period and specific user and specific project for manager ( for HeadOfDepartment - only for his department)
    /// Allowed Roles: CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="projectId"></param>
    /// <param name="year">Year of period</param>
    /// <param name="monthOfYear">Month of period</param>
    /// <param name="departmentId">Search Department</param>
    /// <returns>List of user tasks for specified period</returns>
    [ProducesResponseType(typeof(List<DashboardTasksCoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("users/{userId:int}/{projectId:int}")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<List<DashboardTasksCoreDto>> GetUserProjectTasksAsync(int userId, int projectId, int year, [FromQuery] int[] monthOfYear, string? departmentId)
    {
        return await _sender.Send(new GetDashboardTasksRequest.Query(userId, projectId, year, monthOfYear, departmentId), HttpContext.RequestAborted);
    }

    /// <summary>
    /// Get Pie Chart for specified period for manager ( for HeadOfDepartment - only for his department)
    /// </summary>
    /// <param name="year">Year of period</param>
    /// <param name="monthOfYear">Month of period</param>
    /// <param name="departmentId">Search Department</param>
    /// <returns>List of pie charts for specified period </returns>
    [ProducesResponseType(typeof(List<PieChartUsersCoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [HttpGet("pieChart")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<List<PieChartUsersCoreDto>> GetPieChartUsers(int? year,
        [FromQuery] int[] monthOfYear, string? departmentId)
    {
        return await _sender.Send(new GetPieChartUsersRequest.Query(year, monthOfYear, departmentId), HttpContext.RequestAborted);
    }

    /// <summary>
    /// Get Total Time Stack for specified period for manager ( for HeadOfDepartment - only for his department)
    /// </summary>
    /// <param name="year">Year of period</param>
    /// <param name="monthOfYear">Month of period</param>
    /// <param name="departmentId">Search Department</param>
    /// <returns>List of stacks for specified period</returns>
    [ProducesResponseType(typeof(List<TimeStackCoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("time-stack")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<List<TimeStackCoreDto>> GetTimeStack(int year, [FromQuery] int[] monthOfYear, string? departmentId)
    {
        return await _sender.Send(new GetTimeStackRequest.Query(year, monthOfYear, departmentId), HttpContext.RequestAborted);
    }
}
