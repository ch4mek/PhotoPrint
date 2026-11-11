using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Cv.Requests.CvUsers.Core.CoreDto;
using Modsen.Portal.Cv.Requests.CvUsers.Requests;

namespace Modsen.Portal.Cv.Controllers.Controllers;

[Area("[Cv] Cv users")]
[Route("api/cv-users")]
public class CvUsersController : BaseApiController
{
    private readonly ISender _sender;

    public CvUsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a lookup of all cv users
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <returns>A list of all cv users</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<CvUserLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageCvsPolicy)]
    public async Task<ActionResult<List<CvUserLookupDto>>> GetCvUsersLookup()
    {
        var cvUsers = await _sender.Send(
            new GetCvUsersLookupRequest.Query(),
            HttpContext.RequestAborted);

        return Ok(cvUsers);
    }
}
