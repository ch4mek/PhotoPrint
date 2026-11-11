using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Cv.Requests.CvProjects.Core.CoreDto;
using Modsen.Portal.Cv.Requests.CvProjects.Requests;

namespace Modsen.Portal.Cv.Controllers.Controllers;

[Area("[Cv] Cv projects")]
[Route("api/cv-projects")]
public class CvProjectsController : BaseApiController
{
    private readonly ISender _sender;

    public CvProjectsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a lookup of all cv projects
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <returns>A list of all cv projects</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<CvProjectLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageCvsPolicy)]
    public async Task<ActionResult<List<CvProjectLookupDto>>> GetCvProjectsLookup()
    {
        var cvProjects = await _sender.Send(
            new GetCvProjectsLookupRequest.Query(),
            HttpContext.RequestAborted);

        return Ok(cvProjects);
    }
}
