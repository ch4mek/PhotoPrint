using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Controllers;

[Area("[TimeTrack] Projects")]
internal class ProjectsController : BaseApiController
{
    private readonly ISender _sender;

    public ProjectsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get Project with project users by id
    /// Allowed Roles: CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <returns>List of projects with project users</returns>
    [ProducesResponseType(typeof(ProjectWithManagerOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [HttpGet("{id:int}")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<ProjectWithManagerOutput>> GetByIdAsync([FromRoute] int id)
    {
        var projectWithUsers = await _sender.Send(new GetProjectByIdRequest.Query(id), HttpContext.RequestAborted);

        return Ok(ProjectWithManagerOutput.Of(projectWithUsers));
    }

    /// <summary>
    /// Get a list of projects
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering and paging</param>
    /// <returns>A list of projects</returns>
    [ProducesResponseType(typeof(PageableListOfItems<ProjectWithUserAndTimeInfoOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("search")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<ProjectWithUserAndTimeInfoOutput>>> GetProjectsAsync(
        ODataQueryOptions<OdpProject> queryOptions)
    {
        var pageableProjects = await _sender.Send(
            new GetProjectsRequest.Query(queryOptions),
            HttpContext.RequestAborted);

        var pageableProjectWithUserAndTimeInfoOutput =
            pageableProjects.Items.Select(ProjectWithUserAndTimeInfoOutput.Of)
                .ToPageable()
                .WithPaging((queryOptions.Skip.Value / queryOptions.Top.Value) + 1, queryOptions.Top.Value)
                .WithTotal(pageableProjects.Total);

        return Ok(pageableProjectWithUserAndTimeInfoOutput);
    }

    /// <summary>
    /// Create project
    /// Allowed Roles: CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="project">Input with project properties</param>
    /// <returns>Created Project</returns>
    [ProducesResponseType(typeof(ProjectOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
    [HttpPost]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<ProjectOutput>> CreateAsync([FromBody] CreateProjectInput project)
    {
        var createProjectCommand = new CreateProjectRequest.Command
        (
            Name: project.Name,
            Color: project.Color,
            ManagerId: project.ManagerId,
            ClientId: project.ClientId,
            IsPublic: project.IsPublic,
            IsBillable: project.IsBillable,
            TechStack: project.TechStack,
            Architecture: project.Architecture,
            CloudServices: project.CloudServices,
            Industry: project.Industry
        );

        var createdProject = await _sender.Send(createProjectCommand, HttpContext.RequestAborted);

        return Ok(ProjectOutput.Of(createdProject));
    }

    /// <summary>
    /// Update existing project.
    /// Allowed Roles: CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="project">Model with updated project properties</param>
    /// <returns>Updated Project</returns>
    [ProducesResponseType(typeof(UpdateProjectOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpPut]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<UpdateProjectOutput>> UpdateProjectAsync([FromBody] UpdateProjectInput project)
    {
        var updateProjectCommand = new UpdateProjectRequest.Command
        (
            Id: project.Id,
            Name: project.Name,
            Color: project.Color,
            ManagerId: project.ManagerId,
            ClientId: project.ClientId,
            IsPublic: project.IsPublic,
            IsBillable: project.IsBillable,
            Architecture: project.Architecture,
            CloudServices: project.CloudServices,
            Industry: project.Industry,
            TechStack: project.TechStack
        );
        var updatedProject = await _sender.Send(updateProjectCommand, HttpContext.RequestAborted);

        return Ok(UpdateProjectOutput.Of(updatedProject));
    }

    /// <summary>
    /// Get projects by user id
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="isPublic">Flag for public project</param>
    /// <param name="onlyManagedProjects">Flag for only managed projects</param>
    /// <param name="isActive">Flag for active project</param>
    /// <param name="trackIntervalFrom">Start of track interval</param>
    /// <param name="trackIntervalTo">End of track interval</param>
    /// <returns>List of user projects</returns>
    /// <exception cref="InvalidDataAppException">User dont have permission to get projects by user id</exception>
    /// <exception cref="UnreachableException">User role does not handled by code</exception>
    [ProducesResponseType(typeof(List<ProjectLookupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("~/api/users/{userId:int}/projects")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<ProjectLookupOutput>>> GetProjectsByUserIdAsync(
        [FromRoute] int userId, [FromQuery] bool? isPublic, [FromQuery] bool? onlyManagedProjects, [FromQuery] bool? isActive,
        [FromQuery] DateOnly? trackIntervalFrom, [FromQuery] DateOnly? trackIntervalTo)
    {
        var query = new GetProjectsLookupByUserIdRequest.Query(userId, isPublic, onlyManagedProjects, isActive,
            trackIntervalFrom, trackIntervalTo);

        var projects = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(projects.Select(ProjectLookupOutput.Of).ToList());
    }

    /// <summary>
    /// Get projects by key word
    /// Allowed Roles: Authenticated user
    /// </summary>
    /// <param name="keyWord">Substring that project name should contain</param>
    /// <param name="isPublic">Indicates whether to get public or private projects</param>
    /// <param name="isActive">Indicates whether to get active or inactive projects</param>
    /// <param name="onlyManagedProjects">Flag for only managed project</param>
    /// <returns>List of projects</returns>
    [ProducesResponseType(typeof(List<ProjectLookupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet("lookup")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<ProjectLookupOutput>>> GetProjectsLookupAsync(
        [FromQuery] string? keyWord,
        [FromQuery] bool? isPublic,
        [FromQuery] bool? isActive,
        [FromQuery] bool? onlyManagedProjects)
    {
        var projects = await _sender.Send(
            new GetProjectsLookupRequest.Query(keyWord, isPublic, isActive, onlyManagedProjects),
            HttpContext.RequestAborted);

        var result = projects.ConvertAll(p => new ProjectLookupOutput
        {
            Id = p.Id,
            ClientName = p.ClientName,
            IsBillable = p.IsBillable,
            IsCurrentUserAbleToSeeBillableTrackedTime = p.IsCurrentUserAbleToSeeBillableTrackedTime,
            Name = p.Name,
            Color = p.Color
        });

        return Ok(result);
    }

    /// <summary>
    /// Update project status (active/inactive)
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="id">Project ID</param>
    /// <param name="input">Input with isActive property</param>
    /// <returns>Updated Project</returns>
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpPut("{id:int}/IsActive")]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<bool>> UpdateProjectStatusAsync(
        [FromRoute] int id,
        [FromBody] ProjectStatusInput input)
    {
        var result = await _sender.Send(
            new UpdateProjectStatusRequest.Command(id, input.IsActive),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get projects by key word sorted by type and name
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="keyWord">Substring that project name should contain</param>
    /// <returns>List of sorted projects</returns>
    [ProducesResponseType(typeof(List<ProjectSortedLookupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet("sorted-lookup")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<ProjectSortedLookupOutput>>> SortedLookupProjectsAsync(
        [FromQuery] string? keyWord)
    {
        var projects = await _sender.Send(
            new GetProjectsSortedLookupRequest.Query(keyWord),
            HttpContext.RequestAborted);

        return Ok(projects.Select(ProjectSortedLookupOutput.Of).ToList());
    }

    /// <summary>
    ///  Get current user projects sorted by type and name
    ///  Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="keyWord">Substring that project name should contain</param>
    /// <returns>Sorted list of user projects </returns>
    [ProducesResponseType(typeof(List<ProjectSortedLookupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet("sorted-lookup/personal")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<ProjectSortedLookupOutput>>> SortedPersonalProjectLookupAsync([FromQuery] string? keyWord)
    {
        var projects = await _sender.Send(
            new GetSortedPersonalProjectLookupRequest.Query(keyWord),
            HttpContext.RequestAborted);

        return Ok(projects.Select(ProjectSortedLookupOutput.Of).ToList());
    }

    /// <summary>
    /// Gets all users who can be project managers.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <returns>Manager users</returns>
    [ProducesResponseType(typeof(List<ProjectManagersLookupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("managers/lookup")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<ProjectManagersLookupOutput>>> GetProjectManagersLookupAsync()
    {
        var projectManagers = await _sender.Send(
            new GetProjectManagersLookupRequest.Query(),
            HttpContext.RequestAborted);

        var output = projectManagers.ConvertAll(x =>
            new ProjectManagersLookupOutput(x.ManagerUserId, x.ManagerUserFullName));

        return Ok(output);
    }

    /// <summary>
    /// Get user's private projects with disabled days.
    /// Allowed Roles : AuthenticatedUser
    /// </summary>
    /// <param name="userId">User id to find projects for.</param>
    /// <param name="trackIntervalFrom">Start of track interval.</param>
    /// <param name="trackIntervalTo">End of track interval.</param>
    /// <param name="onlyManagedProjects">Flag for only managed projects</param>
    /// <returns>User's private projects</returns>
    [HttpGet]
    [Route("~/api/users/{userId:int}/projects/with-disabled-days")]
    [ProducesResponseType(typeof(List<ProjectWithDisabledDaysDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<ProjectWithDisabledDaysDto>>> GetUserProjectsWithDisabledDaysAsync(
        [FromRoute] int userId, [FromQuery] bool? onlyManagedProjects, [FromQuery] DateOnly trackIntervalFrom, [FromQuery] DateOnly trackIntervalTo)
    {
        var command = new GetUserProjectsWithDisabledDaysRequest.Query(userId, onlyManagedProjects, trackIntervalFrom, trackIntervalTo);

        var response = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(response);
    }
}
