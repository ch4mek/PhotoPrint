using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Personal info")]
[Route("api/personal-info")]
internal class PersonalInfoController : BaseApiController
{
    private readonly ISender _sender;

    public PersonalInfoController(ISender sender)
    {
        _sender = sender;
    }


    /// <summary>
    /// Update flag is user need project
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="isUserNeedProject">Is user need project</param>
    /// <returns></returns>
    [HttpPatch("need-project")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<bool>> UpdateIsUserNeedProject([FromQuery] int userId, [FromQuery] bool isUserNeedProject)
    {
        var result = await _sender.Send(new UpdateIsUserNeedProjectRequest.Command(userId, isUserNeedProject));

        return Ok(result);
    }

    /// <summary>
    /// Get user plan
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">User id</param>
    /// <returns></returns>
    [HttpGet("plan")]
    [ProducesResponseType(typeof(UserDetailsOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<string?>> GetUserPlan([FromQuery] int userId)
    {
        var result = await _sender.Send(new GetUserDetailsRequest.Query(userId));

        return Ok(new UserDetailsOutput(result.Plan));
    }


    /// <summary>
    /// Update user plan
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">User id</param>
    /// <param name="input">User plan</param>
    /// <returns></returns>
    [HttpPatch("plan")]
    [ProducesResponseType(typeof(UserPlanOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<string?>> UpdateUserPlan([FromQuery] int userId, UserPlanInput input)
    {
        var result = await _sender.Send(new UpdateUserPlanRequest.Command(userId, input.Plan));

        return Ok(new UserPlanOutput(result));
    }

    /// <summary>
    /// Get user's personal info
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="userId">ID of a user to find a personal info</param>
    /// <param name="overdueDaysCount">Count of overdue days</param>
    /// <returns>User's personal info</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PersonalInfoOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PersonalInfoOutput>> GetPersonalInfoAsync([FromQuery] int userId,
        [FromQuery] int? overdueDaysCount)
    {
        var query = new GetPersonalInfoRequest.Query(userId, overdueDaysCount);

        var result = await _sender.Send(query);

        return PersonalInfoOutput.Of(result);
    }

    /// <summary>
    /// Update user's personal data
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Data to update user's personal data</param>
    /// <returns>Updated personal data</returns>
    [HttpPatch]
    [ProducesResponseType(typeof(PersonalDataOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
       [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmHRPolicyName)]
    public async Task<ActionResult<PersonalDataOutput>> UpdatePersonalDataAsync(
        [FromBody] UpdatePersonalDataRequestDto input)
    {
        var command = new UpdatePersonalDataRequest.Command(input);

        var dataDto = await _sender.Send(command);

        return PersonalDataOutput.Of(dataDto);
    }

    /// <summary>
    /// Write comment about an employee
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Data to update a comment about user.</param>
    /// <returns>Written comment about an employee</returns>
    [HttpPatch("comment")]
    [ProducesResponseType(typeof(TeamBoardUserCommentOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<TeamBoardUserCommentOutput>> UpdateTeamBoardUserCommentAsync(
        [FromBody] UpdateTeamBoardUserCommentInput input)
    {
        var command = new UpdateTeamBoardUserCommentRequest.Command(input.UserId, input.Comment);

        var commentDto = await _sender.Send(command);

        return Ok(new TeamBoardUserCommentOutput(commentDto.Comment));
    }
}
