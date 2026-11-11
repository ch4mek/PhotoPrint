using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Users.Requests;
using Modsen.Portal.TeamBoard.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Users")]
[Route("api/teamboard-users")]
public class TeamBoardUsersController : ControllerBase
{
    private readonly ISender _sender;

    public TeamBoardUsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of users
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering, paging</param>
    /// <returns>A list of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableOdpTeamboardUsersOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableOdpTeamboardUsersOutput>> GetPageableTeamBoardUsersAsync(
        ODataQueryOptions<OdpTeamBoardUser> queryOptions)
    {
        var pageableTeamBoardUsers = await _sender.Send(new GetOdpTeamBoardUsersRequest.Query(queryOptions),
            HttpContext.RequestAborted);

        return Ok(new PageableOdpTeamboardUsersOutput(pageableTeamBoardUsers));
    }

    /// <summary>
    /// Get a list of all teamboard users
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// /// <param name="includeInactive">indicates if inactive users should be included</param>
    /// <returns>A list of all teamboard users</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<TeamBoardUserRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<TeamBoardUserRequestDto>>> GetTeamBoardUserLookup(
        [FromQuery] bool includeInactive = false)
    {
        var teamBoardUsers = await _sender.Send(
            new GetTeamBoardUsersLookupRequest.Query(includeInactive),
            HttpContext.RequestAborted);

        return Ok(teamBoardUsers);
    }

    /// <summary>
    /// Get a list of all sale managers
    /// </summary>
    /// <returns>A list of users from 'Sales' department</returns>
    [HttpGet("~/api/sale-managers/lookup")]
    [Authorize(Policy = PortalPolicyNames.ManageInterviewsPolicy)]
    public async Task<ActionResult<List<TeamBoardUserRequestDto>>> GetSaleManagersLookupAsync(
        [FromQuery] bool? hasInterviews, [FromQuery] bool? onlyActive, [FromQuery] bool includeTopManagement = false)
    {
        var saleManagers = await _sender.Send(
            new GetSaleManagersLookupRequest.Query
            {
                HasInterviews = hasInterviews, OnlyActive = onlyActive, IncludeTopManagement = includeTopManagement
            },
            HttpContext.RequestAborted);

        return Ok(saleManagers);
    }

    /// <summary>
    /// Get a list of users joint with interviews
    /// </summary>
    /// <param name="onlyActive"></param>
    /// <returns>A list of users joint with interviews</returns>
    [HttpGet("~/api/interview-users/lookup")]
    [ProducesResponseType(typeof(List<TeamBoardUserRequestDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageInterviewsPolicy)]
    public async Task<ActionResult<List<TeamBoardUserRequestDto>>> GetInterviewUsersLookupAsync(
        [FromQuery] bool? onlyActive)
    {
        var users = await _sender.Send(new GetInterviewUsersLookupRequest.Query(onlyActive),
            HttpContext.RequestAborted);

        return Ok(users);
    }
}
