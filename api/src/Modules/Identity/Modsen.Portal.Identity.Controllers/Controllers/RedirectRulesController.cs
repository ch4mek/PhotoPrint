using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Requests.RedirectRules.Requests;
using Modsen.Portal.Identity.Requests.RedirectRules.Requests.RequestDto;

namespace Modsen.Portal.Identity.Controllers.Controllers;

[Area("[Identity] Redirect Rules")]
[Route("api/redirect-rules")]
internal class RedirectRulesController : BaseApiController
{
    private readonly ISender _sender;

    public RedirectRulesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get redirect rule
    /// Allowed Roles : Anonymous
    /// </summary>
    /// <param name="key">Key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Redirect rule</returns>
    [HttpGet("{key}")]
    [ProducesResponseType(typeof(RedirectRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [AllowAnonymous]
    public async Task<ActionResult<RedirectRuleDto>> GetRedirectRuleAsync(string key,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetRedirectRuleRequest.Query(key), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get list of redirect rules
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of redirect rule</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<RedirectRuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<List<RedirectRuleDto>>> GetRedirectRulesListAsync(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetRedirectRulesRequest.Query(), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Create redirect rule
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="input">Redirect rule input</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created redirect rule</returns>
    [HttpPost]
    [ProducesResponseType(typeof(RedirectRuleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult> CreateRedirectRuleAsync(CreateRedirectRuleInput input,
        CancellationToken cancellationToken)
    {
        var result =  await _sender.Send(new CreateRedirectRuleRequest.Command(input.RedirectKey, input.RedirectTo),
            cancellationToken);

        return CreatedAtAction(nameof(GetRedirectRuleAsync), new { key = result.RedirectKey } ,result);
    }

    /// <summary>
    /// Update redirect rule
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="input">Redirect rule input</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated redirect rule</returns>
    [HttpPut]
    [ProducesResponseType(typeof(RedirectRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult> UpdateRedirectRuleAsync(UpdateRedirectRuleInput input,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new UpdateRedirectRuleRequest.Command(input.Id, input.RedirectTo),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Delete redirect rule
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="id">Redirect rule id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deleted redirect rule</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(RedirectRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult> DeleteRedirectRuleAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteRedirectRuleRequest.Command(id), cancellationToken);

        return Ok(result);
    }
}
