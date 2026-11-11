using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Finance.Requests.Currencies.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Currencies.Requests;

namespace Modsen.Portal.Finance.Controllers.Controllers;

[Area("[Finance] Currencies")]
[Route("api/currencies")]
internal sealed class CurrenciesController : BaseApiController
{
    private readonly ISender _sender;

    public CurrenciesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a lookup of all currencies
    /// Allowed Roles : CEO, CTO, users with ManageRates claim
    /// </summary>
    /// <returns>A list of all currencies</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<CurrencyLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageProjectRatesPolicy)]
    public async Task<ActionResult<List<CurrencyLookupDto>>> GetCurrenciesLookup()
    {
        var currencies = await _sender.Send(
            new CurrencyLookupRequest.Query(),
            HttpContext.RequestAborted);

        return Ok(currencies);
    }
}
