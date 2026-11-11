using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Merch.Requests.Users;
using Modsen.Portal.Merch.Requests.Users.RequestDto;

namespace Modsen.Portal.Merch.Controllers.Controllers;

[Area("[Merch] Merch Users")]
[Route("api/merch-users")]

public class MerchUsersController : ControllerBase
{
    private readonly ISender _sender;

    public MerchUsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of all merch users if user has ManageTransactions claim, empty list otherwise
    /// </summary>
    /// <returns>A list of all merch users</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<MerchUserLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<MerchUserLookupDto>>> GetProjectExitUserLookup()
    {
        var merchUsers = await _sender.Send(
            new GetMerchUsersRequest.Query(),
            HttpContext.RequestAborted);

        return Ok(merchUsers);
    }
}
