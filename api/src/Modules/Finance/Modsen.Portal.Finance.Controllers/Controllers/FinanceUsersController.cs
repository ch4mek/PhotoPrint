using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.FinanceUsers.Requests;

namespace Modsen.Portal.Finance.Controllers.Controllers;

[Area("[Finance] FinanceUsers")]
[Route("api/finance-users")]
internal sealed class FinanceUsersController : BaseApiController
{
    private readonly ISender _sender;

    public FinanceUsersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a lookup of all finance users for specified year and month
    /// Allowed Roles : CEO, CTO, users with ViewRates or ViewAllRates claims
    /// </summary>
    /// <param name="year">The year</param>
    /// <param name="month">The month</param>
    /// <returns>A list of all finance users</returns>
    [HttpGet("lookup/{year:int}/{month:int}")]
    [ProducesResponseType(typeof(List<UserCoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewProjectRatesPolicy)]
    public async Task<ActionResult<List<UserCoreDto>>> GetFinanceUsersLookup([FromRoute] int year, [FromRoute] int month)
    {
        var users = await _sender.Send(
            new FinanceUsersLookupRequest.Query(year, month),
            HttpContext.RequestAborted);

        return Ok(users);
    }
}
