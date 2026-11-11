using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Requests;

namespace Modsen.Portal.Identity.Controllers.Controllers;

[Area("[Identity] Permission assignments")]
[Route("api/permissions/assignments")]
internal class PermissionAssignmentController : BaseApiController
{
    private readonly ISender _sender;

    public PermissionAssignmentController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets permission assignments of project resource type.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="queryOptions">OData query options.</param>
    /// <returns>Permission assignments with user and project info.</returns>
    [HttpGet]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    [ProducesResponseType(typeof(PageableListOfItems<ProjectPermissionAssignmentOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PageableListOfItems<ProjectPermissionAssignmentOutput>>> GetProjectPermissionAssignments(
        ODataQueryOptions<ProjectPermissionAssignmentCoreDto> queryOptions)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var query = new GetProjectPermissionAssignmentsRequest.Query(queryOptions);

        var assignments = await _sender.Send(query, cancellationToken);

        return Ok(assignments.Items.Select(ProjectPermissionAssignmentOutput.Of)
            .ToPageable()
            .WithPaging(assignments.Page, assignments.PageSize)
            .WithTotal(assignments.Total));
    }

    /// <summary>
    /// Updates permission assignments for user.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="input">Permissions to update information.</param>
    /// <returns>Collection of permission assignments.</returns>
    [HttpPut]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    [ProducesResponseType(typeof(List<PermissionAssignmentOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<PermissionAssignmentOutput>>> UpdateUserPermissionsAsync(
        [FromBody] UpdateUserPermissionsInput input)
    {
        var cancellationToken = HttpContext.RequestAborted;

        var command = new UpdateUserPermissionsRequest.Command(
            input.UserId,
            input.PermissionType,
            input.Resources,
            input.ResourceType);

        var assignment = await _sender.Send(command, cancellationToken);

        return Ok(assignment.Select(PermissionAssignmentOutput.Of));
    }

    /// <summary>
    /// Removes permission assignment.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="assignmentId">ID of permission assignment.</param>
    /// <returns>Removed permission assignment.</returns>
    [HttpDelete]
    [Route("{assignmentId:int}")]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    [ProducesResponseType(typeof(PermissionAssignmentOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PermissionAssignmentOutput>> RevokeUserPermissionAsync([FromRoute] int assignmentId)
    {
        var cancellationToken = HttpContext.RequestAborted;

        var command = new RevokePermissionRequest.Command(assignmentId);

        var assignment = await _sender.Send(command, cancellationToken);

        return Ok(PermissionAssignmentOutput.Of(assignment));
    }

    /// <summary>
    /// Gets information about permission resources of user.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="userId">ID of user.</param>
    /// <param name="permissionType">Type of permission.</param>
    /// <param name="resourceType">Type of permission resource.</param>
    /// <returns>User permission resources.</returns>
    [HttpGet]
    [Route("resources")]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    [ProducesResponseType(typeof(UserPermissionResourcesOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserPermissionResourcesOutput>> GetUserPermissionResources(
        [FromQuery] int userId,
        [FromQuery] PermissionType permissionType,
        [FromQuery] PermissionResourceType resourceType)
    {
        var query = new GetUserPermissionResourcesRequest.Query(userId, permissionType, resourceType);

        var resources = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(new UserPermissionResourcesOutput
        {
            UserId = userId,
            Resources = resources.Select(x => new ResourceOutput
            {
                ResourceId = x.ResourceId,
                ResourceName = x.ResourceName
            }).ToList()
        });
    }
}
