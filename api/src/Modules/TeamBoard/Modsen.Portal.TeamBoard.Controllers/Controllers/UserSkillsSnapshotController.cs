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
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] User skills snapshot")]
[Route("api/user-skills-snapshot")]
internal class UserSkillsSnapshotController : BaseApiController
{
    private readonly ISender _sender;

    public UserSkillsSnapshotController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get user's skills snapshots.
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <param name="userId">User id to find snapshots for.</param>
    /// <param name="queryOptions">query options</param>
    /// <returns>User's skills snapshots</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<UserSpecializationTemplateWithSnapshotDto>>> GetUserSpecializationsSnapshotsByUserIdAsync(
        [FromQuery] int userId,
        ODataQueryOptions<UserSpecializationTemplateWithSnapshotDto> queryOptions
        )
    {
        var query = new GetUserSnapshotsByUserIdRequest.Query(userId, queryOptions);

        var result = await _sender.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Get user's skills snapshot by date.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">User id to find snapshots for.</param>
    /// <param name="date">Date to find user snapshots for.</param>
    /// <returns>User's skills snapshot</returns>
    [HttpGet("~/api/users/{userId:int}/user-skills-snapshot/{date}")]
    [ProducesResponseType(typeof(UserSnapshotCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<UserSnapshotCoreDto>> GetUserSpecializationsSnapshotByDateAsync(
        [FromRoute] int userId, [FromRoute] DateOnly date)
    {
        var query = new GetUserSnapshotByDateRequest.Query(userId, date);

        var result = await _sender.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Get user's skills snapshot by id.
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">Snapshot idr.</param>
    /// <returns>User's skills snapshot</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserSnapshotCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<UserSnapshotCoreDto>> GetUserSpecializationsSnapshotByIdAsync(
        [FromRoute] int id)
    {
        var query = new GetUserSnapshotByIdRequest.Query(id);

        var result = await _sender.Send(query);

        return Ok(result);
    }

    /// <summary>
    /// Add user skills snapshot
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="createUserSnapshotInput">Model with user skills snapshot properties</param>
    /// <returns>Created user skills snapshot</returns>
    [HttpPost]
    [ProducesResponseType(typeof(UserSnapshotCoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<UserSnapshotCoreDto>> CreateUserSnapshotAsync(
        [FromBody] CreateUserSnapshotInput createUserSnapshotInput)
    {
        var command = new CreateUserSnapshotRequest.Command(
            createUserSnapshotInput.UserId,
            createUserSnapshotInput.SnapshotDate,
            createUserSnapshotInput.UserSpecializations);

        var result = await _sender.Send(command);

        return Ok(result);
    }
}
