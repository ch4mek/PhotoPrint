using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.Controllers.Controllers;

[Area("[TimeTrack] User Projects")]
[Route("api/user-projects")]
internal class UserProjectsController : BaseApiController
{
    private readonly ISender _sender;

    public UserProjectsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Lookup user's assignments to projects.
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <param name="userId">User id to find project for.</param>
    /// <returns>user's assignments to projects lookup</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<UserProjectLookupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<UserProjectLookupOutput>>> GetUserProjectsLookupAsync([FromQuery] int userId)
    {
        var userProjects = await _sender.Send(
            new GetUserProjectsLookupRequest.Query(userId),
            HttpContext.RequestAborted);

        return Ok(userProjects.Select(UserProjectLookupOutput.Of).ToList());
    }

    /// <summary>
    /// Get user's assignments to projects.
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <returns>user's assignment to projects</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableListOfItems<OdpProjectUser>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpProjectUser>>> GetPageableUserProjectsAsync(
        ODataQueryOptions<OdpProjectUser> queryOptions)
    {
        var response = await _sender.Send(
            new GetOdpUserProjectsRequest.Query(queryOptions),
            HttpContext.RequestAborted);

        return Ok(response);
    }

    /// <summary>
    /// Get user's assignment to project by id.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">ID of user's assignment to project.</param>
    /// <returns>user's assignment to project</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserProjectWithNamesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<UserProjectWithNamesDto>> GetUserProjectAssignmentByIdAsync(
        [FromRoute] int id)
    {
        var query = new GetUserProjectByIdRequest.Query(id);
        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Update user's assignment to project.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Parameters required for updating assignment.</param>
    /// <returns>user's assignment</returns>
    [HttpPut]
    [ProducesResponseType(typeof(UserProjectAssignmentOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<UserProjectAssignmentOutput>> UpdateUserProjectAssignmentAsync(
        [FromBody] UpdateUserProjectAssignmentInput input)

    {
        var command = new UpdateUserProjectAssignmentRequest.Command(input.Id,
                input.UserId,
                input.ProjectId,
                input.IsTimeOnProjectBillable,
                input.Fte,
                input.UserOnProjectSeniorityLevelId,
                input.UserOnProjectFrom,
                input.UserOnProjectTo);

        var response = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(new UserProjectAssignmentOutput
        {
            Id = response.Id,
            UserId = response.UserId,
            ProjectId = response.ProjectId,
            IsTimeOnProjectBillable = response.IsTimeOnProjectBillable,
            UserOnProjectFrom = response.UserOnProjectFrom,
            UserOnProjectTo = response.UserOnProjectTo,
            ClientName = response.ClientName,
            ProjectName = response.ProjectName,
            UserFullName = response.UserFullName,
            IsActive = response.IsActive,
        });
    }

    /// <summary>
    /// Create user's assignment to project.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="projectId">Project id</param>
    /// <param name="projectUser">Parameters required for creating assignment.</param>
    /// <returns>user's assignment</returns>
    [HttpPost]
    [Route("~/api/users/{userId:int}/projects/{projectId:int}")]
    [ProducesResponseType(typeof(UserProjectAssignmentOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<UserProjectAssignmentOutput>> CreateUserProjectAssignmentAsync(
        [FromRoute] int userId, [FromRoute] int projectId, [FromBody] CreateUserProjectAssignmentInput projectUser)
    {
        var command = new CreateUserProjectAssignmentRequest.Command(userId, projectId,
                projectUser.UserOnProjectFrom, projectUser.UserOnProjectTo, projectUser.IsTimeOnProjectBillable,
                projectUser.Fte, projectUser.UserOnProjectSeniorityLevelId);

        var response = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(new UserProjectAssignmentOutput
        {
            Id = response.Id,
            UserId = response.UserId,
            ProjectId = response.ProjectId,
            IsTimeOnProjectBillable = response.IsTimeOnProjectBillable,
            UserOnProjectFrom = response.UserOnProjectFrom,
            UserOnProjectTo = response.UserOnProjectTo,
            ClientName = response.ClientName,
            ProjectName = response.ProjectName,
            UserFullName = response.UserFullName,
            IsActive = response.IsActive,
        });
    }
}
