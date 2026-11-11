using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Requests.UserScores.Requests;
using Modsen.Portal.TeamBoard.Requests.UserScores.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Users Score")]
[Route("api/user-score")]
internal class UsersScoreController : BaseApiController
{
    private readonly ISender _sender;

    public UsersScoreController(
        ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Update user score
    /// Allowed Roles : Authorized
    /// </summary>
    /// <param name="input"></param>
    /// <returns>Score id</returns>
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPut]
    [Authorize]
    public async Task<ActionResult<int>> UpdateUserScoreAsync([FromBody] UserScoreInput input)
    {
        var id = await _sender.Send(new UpdateUserScoreRequest.Command(
            input.Score,
            input.Timestamp,
            input.Signature
            ));

        return id;
    }

    /// <summary>
    /// Get leaderboard
    /// Allowed Roles : Authorized
    /// </summary>
    /// <returns>Top user scores</returns>
    [ProducesResponseType(typeof(List<UserScoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("leaderboard")]
    [Authorize]
    public async Task<ActionResult<List<UserScoreDto>>> GetLeaderBoardAsync()
    {
        var leaderboard = await _sender.Send(new GetLeaderBoardRequest.Query());

        return leaderboard;
    }
}
