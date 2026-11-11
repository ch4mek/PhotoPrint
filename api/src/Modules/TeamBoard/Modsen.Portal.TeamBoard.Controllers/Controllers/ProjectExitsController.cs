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
using Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Project Exits")]
[Route("api/project-exits")]
internal class ProjectExitsController(ISender sender) : BaseApiController
{
    /// <summary>
    /// Get a list of project exit records.
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering, paging.</param>
    /// <returns>A list of project exit records.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableListOfItems<OdpProjectExitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewOrManageProjectExitsPolicy)]
    public async Task<ActionResult<PageableListOfItems<OdpProjectExitDto>>> GetPageableProjectExitsAsync(
        ODataQueryOptions<OdpProjectExit> queryOptions)
    {
        var response = await sender.Send(new GetProjectExitsRequest.Query(queryOptions));

        return Ok(response);
    }

    /// <summary>
    /// Get all available closing reasons.
    /// </summary>
    /// <returns>A list of closing reasons.</returns>
    [HttpGet("closing-reasons")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProjectExitClosingReasonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewOrManageProjectExitsPolicy)]
    public async Task<ActionResult<IReadOnlyCollection<ProjectExitClosingReasonDto>>> GetProjectExitClosingReasonsAsync()
    {
        var response = await sender.Send(new GetProjectExitClosingReasonsRequest.Query());

        return Ok(response);
    }

    /// <summary>
    /// Create a new project exit record.
    /// </summary>
    /// <param name="input">Project exit details.</param>
    /// <returns>The created project exit record.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OdpProjectExitDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageProjectExitsPolicy)]
    public async Task<ActionResult<OdpProjectExitDto>> CreateProjectExitAsync([FromBody] CreateProjectExitInput input)
    {
        var response = await sender.Send(new CreateProjectExitRequest.Command(
            input.UserId,
            input.ProjectId,
            input.ClientId,
            input.ClosingDate,
            input.ClosingReasonName));

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Update an existing project exit record.
    /// </summary>
    /// <param name="id">Project exit identifier.</param>
    /// <param name="input">Updated project exit details.</param>
    /// <returns>The updated project exit record.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(OdpProjectExitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageProjectExitsPolicy)]
    public async Task<ActionResult<OdpProjectExitDto>> UpdateProjectExitAsync(
        [FromRoute] int id, [FromBody] UpdateProjectExitInput input)
    {
        var response = await sender.Send(new UpdateProjectExitRequest.Command(
            id,
            input.UserId,
            input.ProjectId,
            input.ClientId,
            input.ClosingDate,
            input.ClosingReasonName));

        return Ok(response);
    }

    /// <summary>
    /// Delete a project exit record (soft delete).
    /// </summary>
    /// <param name="id">Project exit identifier.</param>
    /// <returns>Identifier of the deleted record.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageProjectExitsPolicy)]
    public async Task<ActionResult<int>> DeleteProjectExitAsync([FromRoute] int id)
    {
        var response = await sender.Send(new DeleteProjectExitRequest.Command(id));

        return Ok(response);
    }

    /// <summary>
    /// Restore a deleted project exit record.
    /// </summary>
    /// <param name="id">Project exit identifier.</param>
    /// <returns>The restored project exit record.</returns>
    [HttpPatch("{id:int}/restore")]
    [ProducesResponseType(typeof(OdpProjectExitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageProjectExitsPolicy)]
    public async Task<ActionResult<OdpProjectExitDto>> RestoreProjectExitAsync([FromRoute] int id)
    {
        var response = await sender.Send(new RestoreProjectExitRequest.Command(id));

        return Ok(response);
    }
}
