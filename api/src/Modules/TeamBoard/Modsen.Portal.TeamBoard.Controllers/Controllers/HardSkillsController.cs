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
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;

namespace Modsen.Portal.TeamBoard.Controllers.Controllers;

[Area("[TeamBoard] Hard skills")]
[Route("api/hard-skills")]
internal class HardSkillsController : BaseApiController
{
    private readonly ISender _sender;

    public HardSkillsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of hard skills
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering, paging</param>
    /// <param name="departmentId">parameters for filtering by department</param>
    /// <param name="specializationTemplateId">parameters for filtering by specialization template</param>
    /// <returns>A list of hard skills</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PageableListOfItems<OdpHardSkill>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<OdpHardSkill>>> GetPageableHardSkillsAsync(
        ODataQueryOptions<OdpHardSkill> queryOptions,
        [FromQuery] string? departmentId,
        [FromQuery] int? specializationTemplateId)
    {
        var pageableHardSkillsResponse = await _sender.Send(
            new GetOdpHardSkillsRequest.Query(queryOptions, departmentId, specializationTemplateId),
            HttpContext.RequestAborted);

        return Ok(pageableHardSkillsResponse);
    }

    /// <summary>
    /// Get a hard skill
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">Hard skill id</param>
    /// <returns>The hard skill</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PageableListOfItems<OdpHardSkill>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<HardSkillByIdOutput>> GetHardSkillByIdAsync(
        [FromRoute] int id)
    {
        var hardSkill = await _sender.Send(
            new GetHardSkillByIdRequest.Query(id),
            HttpContext.RequestAborted);

        return Ok(new HardSkillByIdOutput(hardSkill.Id, hardSkill.Name, hardSkill.HardSkillGroupId, hardSkill.HardSkillGroupName ?? string.Empty));
    }

    /// <summary>
    /// Create the hard skill
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Model of a new hard skill with group id</param>
    /// <returns>Created hard skill</returns>
    [HttpPost]
    [ProducesResponseType(typeof(HardSkillOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<HardSkillOutput>> CreateHardSkillAsync(
        [FromBody] CreateHardSkillInput input)
    {
        var command = new CreateHardSkillRequest.Command(input.Name, input.HardSkillGroupId);

        var hardSkill = await _sender.Send(command, HttpContext.RequestAborted);

        return CreatedAtAction(
            nameof(GetHardSkillByIdAsync),
            new { hardSkill.Id },
            new HardSkillOutput(hardSkill.Id, hardSkill.Name, hardSkill.HardSkillGroupId));
    }

    /// <summary>
    /// Update the hard skill
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="input">Model of a hard skill update</param>
    /// <returns>Updated hard skill</returns>
    [HttpPut]
    [ProducesResponseType(typeof(HardSkillOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<HardSkillOutput>> UpdateHardSkillAsync(
        [FromBody] UpdateHardSkillInput input)
    {
        var command = new UpdateHardSkillRequest.Command(input.Id, input.Name, input.HardSkillGroupId);

        var hardSkill = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(new HardSkillOutput(hardSkill.Id, hardSkill.Name, hardSkill.HardSkillGroupId));
    }

    /// <summary>
    /// Delete the hard skill
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">The hard skill id</param>
    /// <returns>Deleted hard skill</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(HardSkillOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefAndHeadOnlyPolicyName.Name)]
    public async Task<ActionResult<HardSkillOutput>> DeleteHardSkillAsync(
        [FromRoute] int id)
    {
        var command = new DeleteHardSkillRequest.Command(id);

        var hardSkill = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(new HardSkillOutput(hardSkill.Id, hardSkill.Name, hardSkill.HardSkillGroupId));
    }

    /// <summary>
    /// Get hard skills lookup
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <returns>List of hard skills</returns>
    [HttpGet("lookup")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<List<HardSkillLookupOutput>>> GetHardSkillsLookupAsync()
    {
        var result = await _sender.Send(
            new GetHardSkillsLookupRequest.Query(),
            HttpContext.RequestAborted);

        var hardSkills = result
            .Select(x => new HardSkillLookupOutput(x.Id, x.Name))
            .ToList();

        return Ok(hardSkills);
    }
}
