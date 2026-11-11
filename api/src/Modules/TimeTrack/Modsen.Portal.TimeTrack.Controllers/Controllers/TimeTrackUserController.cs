using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Queries;
using Modsen.Portal.TimeTrack.UseCases.Queries.GetUsersLookupWithFilters;

namespace Modsen.Portal.TimeTrack.Controllers.Controllers;

[Area("[TimeTrack] Users")]
[Route("api/users")]
internal class TimeTrackUserController : BaseApiController
{
    private readonly ISender _sender;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public TimeTrackUserController(
        ISender sender,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _sender = sender;
        _authenticatedUserContext = authenticatedUserContext;
    }

    /// <summary>
    /// Get User Lookup with filters
    /// Allowed Roles: CEO, CTO, Head Of Department, Group Manager
    /// </summary>
    /// <param name="keyWord">Substring that should contain in FirstName or LastName of user </param>
    /// <param name="clientId">ID of client that should have any projects with user</param>
    /// <param name="projectId">ID of project that should have this user</param>
    /// <param name="departmentId">User Department</param>
    /// <returns>Filtered User</returns>
    [ProducesResponseType(typeof(UserLookupWithFiltersOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("lookup/filters")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<UserLookupWithFiltersOutput>>> LookupUsersWithFilterAsync(
        [FromQuery] string? keyWord,
        [FromQuery] int? clientId,
        [FromQuery] int? projectId,
        [FromQuery] string? departmentId)
    {
        var query = new GetUsersLookupWithFiltersQuery(clientId, projectId, departmentId, keyWord);

        var users = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(users.Select(UserLookupWithFiltersOutput.Of).ToList());
    }

    /// <summary>
    /// Get User Lookup with filters for report
    /// Allowed Roles: CEO, CTO, Head Of Department, Group Manager, Employee
    /// </summary>
    /// <param name="keyWord">Substring that should contain in FirstName or LastName of user </param>
    /// <param name="clientId">ID of client that should have any projects with user</param>
    /// <param name="projectId">ID of project that should have this user</param>
    /// <param name="departmentId">User Department</param>
    /// <param name="resourceGroupId">User resource group</param>
    /// <param name="onlyManagedUsers">boolean value that filters managed users</param>
    /// <param name="reportForDateFrom">from</param>
    /// <param name="reportForDateTo">to</param>
    /// <returns>Filtered User for report</returns>
    [HttpGet]
    [Route("lookup/filters/reports")]
    [Authorize(Policy = ChiefHeadGmEmployeeOnlyPolicyName.Name)]
    public async Task<ActionResult<List<UserLookupWithFiltersOutput>>> UserLookupWithFiltersForReportsAsync(
        [FromQuery] string? keyWord,
        [FromQuery] int? clientId,
        [FromQuery] int? projectId,
        [FromQuery] string? departmentId,
        [FromQuery] int? resourceGroupId,
        [FromQuery] bool? onlyManagedUsers,
        [FromQuery] DateOnly? reportForDateFrom,
        [FromQuery] DateOnly? reportForDateTo)
    {
        departmentId = _authenticatedUserContext.GetRoles().IsHeadOfDepartment()
            ? _authenticatedUserContext.DepartmentId
            : departmentId;

        resourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager()
            ? _authenticatedUserContext.ResourceGroupId
            : resourceGroupId;

        var query = new GetUsersLookupWithFiltersForReportsUseCase.Query(
            clientId,
            projectId,
            departmentId,
            resourceGroupId,
            keyWord,
            onlyManagedUsers,
            reportForDateFrom,
            reportForDateTo);

        var users = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(users.Select(UserLookupWithFiltersOutput.Of).ToList());
    }
}
