using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output.Reports;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

namespace Modsen.Portal.TimeTrack.Controllers.Controllers;

[Area("[TimeTrack] Reports")]
internal sealed class ReportsController : BaseApiController
{
    private readonly ISender _sender;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimeTrackPortalSettingsAccessor _timeTrackPortalSettingsAccessor;

    public ReportsController(
        ISender sender,
        IAuthenticatedUserContext authenticatedUserContext,
        ITimeTrackPortalSettingsAccessor timeTrackPortalSettingsAccessor)
    {
        _sender = sender;
        _authenticatedUserContext = authenticatedUserContext;
        _timeTrackPortalSettingsAccessor = timeTrackPortalSettingsAccessor;
    }

    /// <summary>
    /// Get tracked time per user
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="page">number of pagination page</param>
    /// <param name="pageSize">size of elements of pagination page</param>
    /// <param name="year">year of tracked period</param>
    /// <param name="monthOfYear">month of tracked period</param>
    /// <param name="weekOfYear">week of tracked period</param>
    /// <param name="userId">User ID</param>
    /// <param name="departmentId">User Department</param>
    /// <param name="resourceGroupId">User resource group</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="projectId">Project ID</param>
    /// <param name="trackedTimeType">Tracked time type</param>
    /// <returns>Pageable list of tracked time reports</returns>
    [ProducesResponseType(typeof(PageableListOfItems<TrackedTimePerUserReportOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Authorize(Policy = DefaultPolicyName.Name)]
    [Route("users")]
    public async Task<ActionResult<PageableListOfItems<TrackedTimePerUserReportOutput>>> GetTrackedTimePerUserAsync(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] int? year,
        [FromQuery] int[] monthOfYear,
        [FromQuery] int? weekOfYear,
        [FromQuery] int? userId,
        [FromQuery] string? departmentId,
        [FromQuery] int? resourceGroupId,
        [FromQuery] int? clientId,
        [FromQuery] int? projectId,
        [FromQuery] TrackedTimeType trackedTimeType)
    {
        var query = new GetTrackedTimePerUsersRequest.Query
        {
            Page = page ?? 1,
            PageSize = pageSize ?? 50,
            Year = year,
            MonthOfYear = monthOfYear,
            WeekOfYear = weekOfYear,
            UserId = userId,
            DepartmentId = departmentId,
            ResourceGroupId = resourceGroupId,
            ClientId = clientId,
            ProjectId = projectId,
            TrackedTimeType = trackedTimeType
        };

        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(result.Items.Select(TrackedTimePerUserReportOutput.Of)
            .ToPageable()
            .WithPaging(result.Page, result.PageSize)
            .WithTotal(result.Total));
    }

    /// <summary>
    /// Get tracked time per period
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="year">year of tracked period</param>
    /// <param name="monthOfYear">month of tracked period</param>
    /// <param name="weekOfYear">week of tracked period</param>
    /// <param name="userId">User ID</param>
    /// <param name="departmentId">User Department</param>
    /// <param name="resourceGroupId">User resource group</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="projectId">Project ID</param>
    /// <param name="trackedTimeType">tracked Time type</param>
    /// <returns>Tracked time report per period</returns>
    [ProducesResponseType(typeof(TrackedTimePerPeriodReportOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<TrackedTimePerPeriodReportOutput>> GetTrackedTimePerPeriodReportAsync(
        [FromQuery] int? year,
        [FromQuery] int[] monthOfYear,
        [FromQuery] int? weekOfYear,
        [FromQuery] string? departmentId,
        [FromQuery] int? resourceGroupId,
        [FromQuery] int? clientId,
        [FromQuery] int? projectId,
        [FromQuery] int? userId,
        [FromQuery] TrackedTimeType trackedTimeType)
    {
        var query = new GetTrackedTimePerPeriodRequest.Query
        {
            UserId = userId,
            ClientId = clientId,
            ProjectId = projectId,
            DepartmentId = departmentId,
            ResourceGroupId = resourceGroupId,
            Year = year,
            MonthOfYear = monthOfYear,
            WeekOfYear = weekOfYear,
            TrackedTimeType = trackedTimeType
        };

        var report = await _sender.Send(query);

        return Ok(TrackedTimePerPeriodReportOutput.Of(report));
    }

    /// <summary>
    /// Get tracked time per projects
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="year">year of tracked period</param>
    /// <param name="monthOfYear">month of tracked period</param>
    /// <param name="weekOfYear">week of tracked period</param>
    /// <param name="userId">User ID</param>
    /// <param name="departmentId">User Department</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="projectId">Project ID</param>
    /// <param name="trackedTimeType">Tracked time type</param>
    /// <returns>List of Tracked time reports per projects</returns>
    [ProducesResponseType(typeof(List<TrackedTimePerProjectReportOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("{userId:int}")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<TrackedTimePerProjectReportOutput>>> GetTrackedTimePerProjectsAsync(
        [FromRoute] int userId,
        [FromQuery] int? year,
        [FromQuery] int[] monthOfYear,
        [FromQuery] int? weekOfYear,
        [FromQuery] string? departmentId,
        [FromQuery] int? clientId,
        [FromQuery] int? projectId,
        [FromQuery] TrackedTimeType trackedTimeType)
    {
        var query = new GetTrackedTimePerProjectsRequest.Query(
            userId,
            year,
            monthOfYear,
            weekOfYear,
            departmentId,
            clientId,
            projectId,
            trackedTimeType);

        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(result.Select(TrackedTimePerProjectReportOutput.Of).ToList());
    }

    /// <summary>
    /// Get tracked tasks by user and project
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="year">year of tracked period</param>
    /// <param name="monthOfYear">month of tracked period</param>
    /// <param name="weekOfYear">week of tracked period</param>
    /// <param name="userId">User ID</param>
    /// <param name="departmentId">User Department</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="projectId">Project ID</param>
    /// <param name="trackedTimeType">Tracked time type</param>
    /// <returns>List of tracked tasks with specified user and project</returns>
    [ProducesResponseType(typeof(List<TrackedTaskReportOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("{userId:int}/{projectId:int}")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<TrackedTaskReportOutput>>> GetTrackedTasksByUserAndProjectAsync(
        [FromRoute] int userId,
        [FromRoute] int projectId,
        [FromQuery] int? year,
        [FromQuery] int[] monthOfYear,
        [FromQuery] int? weekOfYear,
        [FromQuery] string? departmentId,
        [FromQuery] int? clientId,
        [FromQuery] TrackedTimeType trackedTimeType)
    {
        var query = new GetTrackedTasksByUserAndProjectRequest.Query(
            year,
            monthOfYear,
            weekOfYear,
            userId,
            projectId,
            departmentId,
            clientId,
            trackedTimeType);

        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(result.Select(TrackedTaskReportOutput.Of).ToList());
    }

    /// <summary>
    /// Get users with total tracked time reports
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="year">year of tracked period</param>
    /// <param name="monthOfYear">month of tracked period</param>
    /// <param name="weekOfYear">week of tracked period</param>
    /// <param name="userId">User ID</param>
    /// <param name="departmentId">User Department</param>
    /// <param name="resourceGroupId">User resource group</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="projectId">Project ID</param>
    /// <param name="trackedTimeType">Tracked time type</param>
    /// <returns>List of users with tracked time reports</returns>
    [ProducesResponseType(typeof(List<UserWithTrackedTimeReportOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Authorize(Policy = DefaultPolicyName.Name)]
    [Route("pieChart")]
    public async Task<ActionResult<List<UserWithTrackedTimeReportOutput>>> GetUsersWithTotalTrackedTimeAsync(
       [FromQuery] int? year,
       [FromQuery] int[] monthOfYear,
       [FromQuery] int? weekOfYear,
       [FromQuery] string? departmentId,
       [FromQuery] int? resourceGroupId,
       [FromQuery] int? clientId,
       [FromQuery] int? projectId,
       [FromQuery] int? userId,
       [FromQuery] TrackedTimeType trackedTimeType)
    {
        var query = new GetUsersWithTotalTrackedTimeRequest.Query(
            year,
            monthOfYear,
            weekOfYear,
            departmentId,
            resourceGroupId,
            clientId,
            projectId,
            userId,
            trackedTimeType);

        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(result.Select(UserWithTrackedTimeReportOutput.Of).ToList());
    }

    /// <summary>
    /// Get clients with total tracked time reports
    /// Allowed roles: CEO, CTO, HeadOfDepartment, Group Manager
    /// </summary>
    /// <param name="year">year of tracked period</param>
    /// <param name="monthOfYear">month of tracked period</param>
    /// <param name="weekOfYear">week of tracked period</param>
    /// <param name="userId">User ID</param>
    /// <param name="departmentId">User Department</param>
    /// <param name="resourceGroupId">User resource group</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="projectId">Project ID</param>
    /// <param name="trackedTimeType">Tracked time type</param>
    /// <returns>List of clients with tracked time reports</returns>
    [ProducesResponseType(typeof(List<ClientWithTrackedTimeReportOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    [Route("pieChart/clients")]
    public async Task<ActionResult<List<ClientWithTrackedTimeReportOutput>>> GetClientsWithTotalTrackedTimeAsync(
        [FromQuery] int? year,
        [FromQuery] int[] monthOfYear,
        [FromQuery] int? weekOfYear,
        [FromQuery] string? departmentId,
        [FromQuery] int? resourceGroupId,
        [FromQuery] int? clientId,
        [FromQuery] int? projectId,
        [FromQuery] int? userId,
        [FromQuery] TrackedTimeType trackedTimeType)
    {
        var query = new GetClientsWithTotalTrackedTimeRequest.Query(
            year,
            monthOfYear,
            weekOfYear,
            departmentId,
            resourceGroupId,
            clientId,
            projectId,
            userId,
            trackedTimeType);

        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(result.Select(ClientWithTrackedTimeReportOutput.Of).ToList());
    }

    /// <summary>
    /// Get projects with total tracked time reports
    /// Allowed roles: CEO, CTO, HeadOfDepartment, Group Manager
    /// </summary>
    /// <param name="year">year of tracked period</param>
    /// <param name="monthOfYear">month of tracked period</param>
    /// <param name="weekOfYear">week of tracked period</param>
    /// <param name="userId">User ID</param>
    /// <param name="departmentId">User Department</param>
    /// <param name="resourceGroupId">User resource group</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="projectId">Project ID</param>
    /// <param name="trackedTimeType">Tracked time type</param>
    /// <returns>List of projects with tracked time reports</returns>
    [ProducesResponseType(typeof(List<ProjectWithTrackedTimeReportOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    [Route("pieChart/projects")]
    public async Task<ActionResult<List<ProjectWithTrackedTimeReportOutput>>> GetProjectsWithTotalTrackedTimeAsync(
        [FromQuery] int? year,
        [FromQuery] int[] monthOfYear,
        [FromQuery] int? weekOfYear,
        [FromQuery] string? departmentId,
        [FromQuery] int? resourceGroupId,
        [FromQuery] int? clientId,
        [FromQuery] int? projectId,
        [FromQuery] int? userId,
        [FromQuery] TrackedTimeType trackedTimeType)
    {
        var query = new GetProjectsWithTotalTrackedTimeRequest.Query(
            year,
            monthOfYear,
            weekOfYear,
            departmentId,
            resourceGroupId,
            clientId,
            projectId,
            userId,
            trackedTimeType);

        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(result.Select(ProjectWithTrackedTimeReportOutput.Of).ToList());
    }

    /// <summary>
    /// Get projects, clients and users with total tracked time reports
    /// Allowed roles: CEO, CTO, HeadOfDepartment, Group Manager
    /// </summary>
    /// <param name="year">year of tracked period</param>
    /// <param name="monthOfYear">month of tracked period</param>
    /// <param name="weekOfYear">week of tracked period</param>
    /// <param name="userId">User ID</param>
    /// <param name="departmentId">User Department</param>
    /// <param name="resourceGroupId">User resource group</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="projectId">Project ID</param>
    /// <param name="trackedTimeType">Tracked time type</param>
    /// <returns>List of projects, clients and users with tracked time reports</returns>
    [ProducesResponseType(typeof(ClientsUsersAndProjectsWithTrackedTimeReportOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    [Route("pieCharts")]
    public async Task<ActionResult<ClientsUsersAndProjectsWithTrackedTimeReportOutput>> GetProjectsClientsAndUsersWithTotalTrackedTimeAsync(
        [FromQuery] int? year,
        [FromQuery] int[] monthOfYear,
        [FromQuery] int? weekOfYear,
        [FromQuery] string? departmentId,
        [FromQuery] int? resourceGroupId,
        [FromQuery] int? clientId,
        [FromQuery] int? projectId,
        [FromQuery] int? userId,
        [FromQuery] TrackedTimeType trackedTimeType)
    {
        var projectsQuery = new GetProjectsWithTotalTrackedTimeRequest.Query(
            year,
            monthOfYear,
            weekOfYear,
            departmentId,
            resourceGroupId,
            clientId,
            projectId,
            userId,
            trackedTimeType);

        var clientsQuery = new GetClientsWithTotalTrackedTimeRequest.Query(
            year,
            monthOfYear,
            weekOfYear,
            departmentId,
            resourceGroupId,
            clientId,
            projectId,
            userId,
            trackedTimeType);

        var usersQuery = new GetUsersWithTotalTrackedTimeRequest.Query(
            year,
            monthOfYear,
            weekOfYear,
            departmentId,
            resourceGroupId,
            clientId,
            projectId,
            userId,
            trackedTimeType);

        var projects = await _sender.Send(projectsQuery, HttpContext.RequestAborted);
        var clients = await _sender.Send(clientsQuery, HttpContext.RequestAborted);
        var users = await _sender.Send(usersQuery, HttpContext.RequestAborted);

        var result = new ClientsUsersAndProjectsWithTrackedTimeReportOutput
        {
            PerProjects = projects.Select(ProjectWithTrackedTimeReportOutput.Of).ToList(),
            PerClients = clients.Select(ClientWithTrackedTimeReportOutput.Of).ToList(),
            PerUsers = users.Select(UserWithTrackedTimeReportOutput.Of).ToList(),
        };

        return Ok(result);
    }

    /// <summary>
    /// Get tracked time by user, project or client in file
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="year">year of tracked period</param>
    /// <param name="monthOfYear">month of tracked period</param>
    /// <param name="weekOfYear">week of tracked period</param>
    /// <param name="userId">User ID</param>
    /// <param name="format">File format</param>
    /// <param name="departmentId">User Department</param>
    /// <param name="resourceGroupId">User resource group</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="projectId">Project ID</param>
    /// <param name="trackedTimeType">Type of tracked time</param>
    /// <param name="isPerUserExport">Is per user export enabled (false if not provided default)</param>
    /// <returns>Report file in specified format</returns>
    /// <response code="200">Requested report file</response>
    [ProducesResponseType(typeof(ProcessReportStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [HttpGet]
    [Authorize(Policy = DefaultPolicyName.Name)]
    [Route("{format}")]
    public async Task<IActionResult> GetTrackedTimeByFileAsync(
        [FromQuery] int? year,
        [FromQuery] int[] monthOfYear,
        [FromQuery] int? weekOfYear,
        [FromQuery] string? departmentId,
        [FromQuery] int? resourceGroupId,
        [FromQuery] int? clientId,
        [FromQuery] int? projectId,
        [FromQuery] int? userId,
        [FromRoute] DocumentType format,
        [FromQuery] TrackedTimeType trackedTimeType,
        [FromQuery] bool isPerUserExport = false)
    {
        switch (_authenticatedUserContext.GetRoles())
        {
            case var roles when roles.IsHeadOfDepartment():
                departmentId = _authenticatedUserContext.DepartmentId;
                break;

            case var roles when roles.IsGroupManager():
                resourceGroupId = _authenticatedUserContext.ResourceGroupId;
                break;

            case var roles when roles.IsEmployee() || roles.IsSystemAdministrator():
                userId = _authenticatedUserContext.UserId;
                break;
        }

        var command = new ProcessReportRequest.Command(
            Year: year,
            MonthOfYear: monthOfYear,
            WeekOfYear: weekOfYear,
            DepartmentId: departmentId,
            ClientId: clientId,
            UserId: userId,
            ProjectId: projectId,
            TrackedTimeType: trackedTimeType,
            ResourceGroupId: resourceGroupId,
            Format: format,
            IsPerUserExport: isPerUserExport);

        var response = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(response);
    }

    /// <summary>
    /// Get tracked time by clients or projects in csv
    /// Allowed roles: Authenticated user
    /// </summary>
    /// <param name="year">year of tracked period</param>
    /// <param name="monthOfYear">month of tracked period</param>
    /// <param name="weekOfYear">week of tracked period</param>
    /// <param name="userId">User ID</param>
    /// <param name="departmentId">User Department</param>
    /// <param name="resourceGroupId">User resource group</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="projectId">Project ID</param>
    /// <returns>Csv report file</returns>
    [ProducesResponseType(typeof(List<ProjectWithTrackedTimeReportOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [HttpGet]
    [Authorize(Policy = DefaultPolicyName.Name)]
    [Route("csv")]
    public async Task<IActionResult> GetToExportCsvAsync(
        [FromQuery] int? year,
        [FromQuery] int[] monthOfYear,
        [FromQuery] int? weekOfYear,
        [FromQuery] string? departmentId,
        [FromQuery] int? resourceGroupId,
        [FromQuery] int? clientId,
        [FromQuery] int? projectId,
        [FromQuery] int? userId)
    {
        switch (_authenticatedUserContext.GetRoles())
        {
            case var roles when roles.IsGroupManager():
                resourceGroupId = _authenticatedUserContext.ResourceGroupId;
                break;

            case var roles when roles.IsHeadOfDepartment():
                departmentId = _authenticatedUserContext.DepartmentId;
                break;

            case var roles when roles.IsEmployee() || roles.IsSystemAdministrator():
                userId = _authenticatedUserContext.UserId;
                break;
        }

        var query = new GetTrackedTimeByCsvRequest.Query(
            year,
            monthOfYear,
            weekOfYear,
            departmentId,
            resourceGroupId,
            clientId,
            projectId,
            userId);

        var cancellationToken = HttpContext.RequestAborted;
        var result = await _sender.Send(query, cancellationToken);

        var settings = await _timeTrackPortalSettingsAccessor.GetSettings(cancellationToken);

        return File(result, MediaTypeNames.Text.Csv, settings.DefaultTimesheetReportName + PortalFileExtensionNames.Csv);
    }
}
