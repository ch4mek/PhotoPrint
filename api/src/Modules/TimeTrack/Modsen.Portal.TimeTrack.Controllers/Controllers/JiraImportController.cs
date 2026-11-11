using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

namespace Modsen.Portal.TimeTrack.Controllers.Controllers;

[Area("[TimeTrack] Jira Import")]
[Route("api/jira-import")]
public sealed class JiraImportController : BaseApiController
{
    private readonly ISender _sender;

    public JiraImportController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get Jira projects configured for the current user.
    /// Allowed Roles: User with claim ViewJiraImport.
    /// </summary>
    /// <returns>Jira projects available for import.</returns>
    [ProducesResponseType(typeof(IReadOnlyCollection<JiraImportProjectOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [HttpGet("projects")]
    [Authorize(Policy = TimeTrackPolicyNames.ViewJiraImportPolicy)]
    public async Task<ActionResult<IReadOnlyCollection<JiraImportProjectOutput>>> GetProjectsAsync()
    {
        var result = await _sender.Send(new GetJiraImportProjectsRequest.Query(), HttpContext.RequestAborted);

        return Ok(result.Select(JiraImportProjectOutput.Of).ToList());
    }

    /// <summary>
    /// Import current user's Jira worklogs to MMS timesheets.
    /// Allowed Roles: User with claim ViewJiraImport.
    /// </summary>
    /// <param name="input">Jira import parameters.</param>
    /// <returns>Import result counters.</returns>
    [ProducesResponseType(typeof(JiraImportResultOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [HttpPost("timesheets")]
    [Authorize(Policy = TimeTrackPolicyNames.ViewJiraImportPolicy)]
    public async Task<ActionResult<JiraImportResultOutput>> ImportTimesheetsAsync(
        [FromBody] ImportJiraTimesheetsInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var result = await _sender.Send(
            new ImportJiraTimesheetsRequest.Command(input.ProjectId, input.StartDate, input.EndDate),
            HttpContext.RequestAborted);

        return Ok(JiraImportResultOutput.Of(result));
    }
}
