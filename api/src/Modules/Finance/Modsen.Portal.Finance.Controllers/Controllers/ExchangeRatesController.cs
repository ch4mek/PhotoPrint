using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Finance.Controllers.Contracts.Output;
using Modsen.Portal.Finance.Requests.ExchangeRates.Requests;

namespace Modsen.Portal.Finance.Controllers.Controllers;

[Area("[Finance] ExchangeRates")]
[Route("api/exchange-rates")]
internal sealed class ExchangeRatesController : BaseApiController
{
    private readonly ISender _sender;

    public ExchangeRatesController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get a list of exchange rates to BYN by date.
    /// Allowed Roles : CEO, CTO, users with ViewManagementTab claim
    /// </summary>
    /// <param name="date">Exchange rates date</param>
    /// <returns>A list of all exchange rates to BYN by date</returns>
    [HttpGet("{date}")]
    [ProducesResponseType(typeof(ExchangeRatesByDateOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewManagementTabPolicy)]
    public async Task<ActionResult<ExchangeRatesByDateOutput>> GetExchangeRates([FromRoute] DateOnly date)
    {
        var exchangeRates = await _sender.Send(new GetExchangeRatesRequest.Query(date), HttpContext.RequestAborted);

        return Ok(ExchangeRatesByDateOutput.Of(exchangeRates));
    }
}
