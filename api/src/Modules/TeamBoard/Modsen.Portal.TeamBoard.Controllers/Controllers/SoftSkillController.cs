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
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Soft skills")]
[Route("api/soft-skills")]
internal class SoftSkillsController : BaseApiController
{
    private readonly ISender _sender;

    public SoftSkillsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of soft skills
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering, paging</param>
    /// <returns>A list of soft skills</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableListOfItems<OdpSoftSkill>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpSoftSkill>>> GetPageableSoftSkillsAsync(
        ODataQueryOptions<OdpSoftSkill> queryOptions)
    {
        var pageableSoftSkillsResponse = await _sender.Send(
            new GetOdpSoftSkillsRequest.Query(queryOptions),
            HttpContext.RequestAborted);

        return Ok(pageableSoftSkillsResponse);
    }

    /// <summary>
    /// Create soft skill
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="model">Model with soft skill properties</param>
    /// <returns>Created soft skill</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SoftSkillOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<SoftSkillOutput>> CreateSoftSkillAsync([FromBody] CreateSoftSkillInput model)
    {
        var createSoftSkillCommand = new CreateSoftSkillRequest.Command(model.Name);

        var createdSkill = await _sender.Send(createSoftSkillCommand, HttpContext.RequestAborted);

        return CreatedAtAction(nameof(GetSoftSkillByIdAsync), new { id = createdSkill.Id }, createdSkill);
    }

    /// <summary>
    /// Get a soft skill
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">Soft skill id</param>
    /// <returns>The soft skill</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PageableListOfItems<SoftSkillOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<SoftSkillOutput>> GetSoftSkillByIdAsync(
        [FromRoute] int id)
    {
        var softSkill = await _sender.Send(
            new GetSoftSkillByIdRequest.Query(id),
            HttpContext.RequestAborted);

        return Ok(new SoftSkillOutput(softSkill.Id, softSkill.Name));
    }

    /// <summary>
    /// Update the soft skill
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="input">Model of a soft skill update</param>
    /// <returns>Updated soft skill</returns>
    [HttpPut]
    [ProducesResponseType(typeof(SoftSkillOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<SoftSkillOutput>> UpdateSoftSkillAsync(
        [FromBody] UpdateSoftSkillInput input)
    {
        var command = new UpdateSoftSkillRequest.Command(input.Id, input.Name);

        var softSkill = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(softSkill);
    }

    /// <summary>
    /// Delete the soft skill
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="id">The soft skill id</param>
    /// <returns>Deleted soft skill</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(SoftSkillOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<SoftSkillOutput>> DeleteSoftSkillAsync(
        [FromRoute] int id)
    {
        var command = new DeleteSoftSkillRequest.Command(id);

        var softSkill = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(new SoftSkillOutput(softSkill.Id, softSkill.Name));
    }

    /// <summary>
    /// Get a list of user's soft skills and its seniority
    /// Allowed Roles : Authenticated User
    /// </summary>
    /// <param name="userId">User id to find soft skills for</param>
    /// <param name="queryOptions">OData query options</param>
    /// <returns>A list of users</returns>
    [HttpGet("~/api/users/{userId:int}/soft-skills")]
    [ProducesResponseType(typeof(PageableListOfItems<OdpUserSoftSkill>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpUserSoftSkill>>> GetPageableUserSoftSkillsAsync([FromRoute] int userId,
        ODataQueryOptions<OdpUserSoftSkill> queryOptions)
    {
        var userSoftSkillsResponse = await _sender.Send(
            new GetUserSoftSkillsRequest.Query(userId, queryOptions),
            HttpContext.RequestAborted);

        return Ok(userSoftSkillsResponse);
    }

    /// <summary>
    /// Get a list of assigned user's soft skills and its seniority
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">User id to find soft skills for</param>
    /// <param name="snapshotId">snapshot id</param>
    /// <returns>A list of users</returns>
    [HttpGet("~/api/users/{userId:int}/soft-skills/assigned")]
    [ProducesResponseType(typeof(List<AssignedSoftSkillOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<AssignedSoftSkillOutput>>> GetAssignedUserSoftSkillsAsync([FromRoute] int userId, int? snapshotId)
    {
        var userSoftSkillsResponse = await _sender.Send(
            new GetAssignedUserSoftSkillsRequest.Query(userId, snapshotId),
            HttpContext.RequestAborted);

        var result = userSoftSkillsResponse.Select(x => new AssignedSoftSkillOutput
        {
            SeniorityLevelId = x.SeniorityLevelId,
            SeniorityLevelName = x.SeniorityLevelName,
            SoftSkillId = x.SoftSkillId,
            SoftSkillName = x.SoftSkillName,
            SoftSkillSeniorityOrderWeight = x.SoftSkillSeniorityOrderWeight,
            SnapshotDate = x.SnapshotDate,
        }).ToList();

        return Ok(result);
    }

    /// <summary>
    /// Create user's soft skill snapshot
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">Model to Create user's soft skill snapshot</param>
    /// <param name="createUserSoftSkillsSnapshotInput">Model to Create user's soft skill snapshot</param>
    /// <returns>Updated user's soft skill id</returns>
    [HttpPost("~/api/users/{userId:int}/soft-skills")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<int>> CreateUserSoftSkillsAsync([FromRoute] int userId,
       [FromBody] CreateUserSoftSkillsSnapshotInput createUserSoftSkillsSnapshotInput)
    {
        var snapshotId = await _sender.Send(
            new CreateUserSoftSkillsSnapshotRequest.Command(createUserSoftSkillsSnapshotInput.ToRequestDto(userId)),
            HttpContext.RequestAborted);

        return Ok(snapshotId);
    }

    /// <summary>
    /// Update user's soft skill spashot
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="snapshotId">Model to update user's soft skill snapshot</param>
    /// <param name="createUserSoftSkillsSnapshotInput">Model to update user's soft skill snapshot</param>
    /// <returns>Updated user's soft skill id</returns>
    [HttpPut("~/api/users/soft-skills/{snapshotId:int}/")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<int>> UpdateUserSoftSkillsAsync([FromRoute] int snapshotId,
       [FromBody] UpdateUserSoftSkillsSnapshotInput createUserSoftSkillsSnapshotInput)
    {
        var result = await _sender.Send(new UpdateUserSoftSkillsSnapshotRequest.Command(
                createUserSoftSkillsSnapshotInput.ToRequestDto(snapshotId)),
            HttpContext.RequestAborted);

        return Ok(result);
    }
}
