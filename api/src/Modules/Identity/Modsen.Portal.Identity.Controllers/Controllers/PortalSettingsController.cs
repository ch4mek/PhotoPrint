using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Identity.Requests.PortalSettings;

namespace Modsen.Portal.Identity.Controllers.Controllers;

[Route("api/portal-settings")]
internal class PortalSettingsController : BaseApiController
{
    private readonly ISender _sender;

    public PortalSettingsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get all portal settings
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <returns>list of portal settings</returns>
    [ProducesResponseType(typeof(List<PortalSettingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<List<PortalSettingDto>>> GetAllPortalSettingsAsync()
    {
        var cancellationToken = HttpContext.RequestAborted;

        var portalSettingsDtos = await _sender.Send(new GetPortalSettingsRequest.Query(), cancellationToken);

        return Ok(portalSettingsDtos);
    }
}
