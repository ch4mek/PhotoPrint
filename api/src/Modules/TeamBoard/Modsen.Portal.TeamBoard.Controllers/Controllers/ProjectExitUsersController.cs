using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;
using Modsen.Portal.TeamBoard.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Project Exit Users")]
[Route("api/project-exit-users")]
public class ProjectExitUsersController : ControllerBase
{
    private readonly ISender _sender;

    public ProjectExitUsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of all project exit users if user has ManageAllProjectExits claim, empty list otherwise
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// Allowed Claims : ManageAllProjectExits, ViewProjectExits
    /// </summary>
    /// /// <param name="includeInactive">indicates if inactive users should be included</param>
    /// <returns>A list of all teamboard users</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<TeamBoardUserRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmSysAdminOrProjectExitsPolicyName)]
    public async Task<ActionResult<List<TeamBoardUserRequestDto>>> GetProjectExitUserLookup(
        [FromQuery] bool includeInactive = false)
    {
        var projectExitUsers = await _sender.Send(
            new GetProjectExitUsersRequest.Query(includeInactive),
            HttpContext.RequestAborted);

        return Ok(projectExitUsers);
    }
}
