using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Cv.Requests.CvClients.Core.CoreDto;
using Modsen.Portal.Cv.Requests.CvClients.Requests;

namespace Modsen.Portal.Cv.Controllers.Controllers;

[Area("[Cv] Cv clients")]
[Route("api/cv-clients")]
public class CvClientsController : BaseApiController
{
    private readonly ISender _sender;

    public CvClientsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a lookup of all cv clients
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <returns>A list of all cv clients</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<CvClientLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageCvsPolicy)]
    public async Task<ActionResult<List<CvClientLookupDto>>> GetCvClientsLookup()
    {
        var cvClients = await _sender.Send(
            new GetCvClientsLookupRequest.Query(),
            HttpContext.RequestAborted);

        return Ok(cvClients);
    }
}
