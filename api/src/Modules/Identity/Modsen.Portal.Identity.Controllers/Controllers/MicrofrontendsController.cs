using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Identity.Requests.Microfrontends.Requests;

namespace Modsen.Portal.Identity.Controllers.Controllers;

[Area("[Identity] Microfrontends")]
[Route("api/microfrontends")]
internal class MicrofrontendsController : BaseApiController
{
    private readonly ISender _sender;

    public MicrofrontendsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get microfrontends options
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <returns>Json with configs</returns>
    [HttpGet("config")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> GetMfeOptions()
    {
        var configs = await _sender.Send(
            new GetMicrofrontendsOptionsRequest.Query(),
            HttpContext.RequestAborted);

        return Ok(configs);
    }

    /// <summary>
    /// Update microfrontends options
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <returns>Json with configs</returns>
    [HttpPut("config")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> UpdateMfeOptions([FromBody] string newConfig)
    {
        var configs = await _sender.Send(
            new UpdateMicrofrontendOptionsRequest.Command(newConfig),
            HttpContext.RequestAborted);

        return Ok(configs);
    }
}
