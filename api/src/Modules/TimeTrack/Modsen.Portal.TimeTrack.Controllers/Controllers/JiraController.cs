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
using Modsen.Portal.TimeTrack.UseCases.Dtos;
using Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

namespace Modsen.Portal.TimeTrack.Controllers.Controllers;

[Area("[TimeTrack] Jira")]
[Route("api/jira-settings")]
public class JiraController : BaseApiController
{
    private readonly ISender _sender;

    public JiraController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get jira setting by id
    /// </summary>
    /// <param name="id">Jira setting id</param>
    /// <returns>Jira setting</returns>
    [ProducesResponseType(typeof(JiraSettingOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [HttpGet("{id:int}")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<JiraSettingOutput>> GetByIdAsync([FromRoute] int id)
    {
        var jiraSetting = await _sender.Send(new GetJiraSettingByIdRequest.Query(id), HttpContext.RequestAborted);

        return Ok(JiraSettingOutput.Of(jiraSetting));
    }

    /// <summary>
    /// Get a list of projects
    /// </summary>
    /// <param name="queryOptions">parameters for searching, ordering and paging</param>
    /// <returns>A list of jira settings</returns>
    [ProducesResponseType(typeof(PageableListOfItems<JiraSettingOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("search")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<JiraSettingOutput>>> GetJiraSettingsAsync(
        ODataQueryOptions<JiraSettingDto> queryOptions)
    {
        var jiraSettings = await _sender.Send(
            new GetJiraSettingsRequest.Query(queryOptions),
            HttpContext.RequestAborted);

        var jiraSettingsOutput = jiraSettings.Items.Select(JiraSettingOutput.Of)
            .ToPageable()
            .WithPaging(jiraSettings.Page, jiraSettings.PageSize)
            .WithTotal(jiraSettings.Total);

        return Ok(jiraSettingsOutput);
    }

    /// <summary>
    /// Create jira setting
    /// </summary>
    /// <param name="jiraSetting">Input with jira setting properties</param>
    /// <returns>Created jira setting</returns>
    [ProducesResponseType(typeof(JiraSettingOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
    [HttpPost]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<JiraSettingOutput>> CreateAsync([FromBody] CreateJiraSettingInput jiraSetting)
    {
        ArgumentNullException.ThrowIfNull(jiraSetting);

        var createJiraSettingCommand = new CreateJiraSettingRequest.Command
        (
            jiraSetting.Url,
            jiraSetting.Pat,
            jiraSetting.JiraProjectTag,
            jiraSetting.Email,
            jiraSetting.ProjectId,
            jiraSetting.UserId
        );

        var createdJiraSetting = await _sender.Send(createJiraSettingCommand, HttpContext.RequestAborted);

        return Ok(JiraSettingOutput.Of(createdJiraSetting));
    }

    /// <summary>
    /// Update jira setting
    /// </summary>
    /// <param name="id">Jira setting id</param>
    /// <param name="jiraSetting">Input with jira setting properties</param>
    /// <returns>Updated jira setting</returns>
    [ProducesResponseType(typeof(JiraSettingOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
    [HttpPut("{id:int}")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<JiraSettingOutput>> UpdateAsync([FromRoute] int id,
        [FromBody] UpdateJiraSettingInput jiraSetting)
    {
        ArgumentNullException.ThrowIfNull(jiraSetting);

        var updateJiraSettingCommand = new UpdateJiraSettingRequest.Command
        (
            id,
            jiraSetting.Url,
            jiraSetting.Pat,
            jiraSetting.JiraProjectTag,
            jiraSetting.Email,
            jiraSetting.ProjectId,
            jiraSetting.UserId
        );

        var updateJiraSetting = await _sender.Send(updateJiraSettingCommand, HttpContext.RequestAborted);

        return Ok(JiraSettingOutput.Of(updateJiraSetting));
    }

    /// <summary>
    /// Update jira setting
    /// </summary>
    /// <param name="id">Jira setting id</param>
    /// <returns>Deleted jira setting id</returns>
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
    [HttpDelete("{id:int}")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<int>> DeleteAsync([FromRoute] int id)
    {
        var deleteJiraSettingCommand = new DeleteJiraSettingRequest.Command(id);

        var deletedJiraSettingId = await _sender.Send(deleteJiraSettingCommand, HttpContext.RequestAborted);

        return Ok(deletedJiraSettingId);
    }
}
