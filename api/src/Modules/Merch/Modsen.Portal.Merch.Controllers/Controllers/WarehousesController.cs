using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Merch.Requests.Warehouses.Requests;

namespace Modsen.Portal.Merch.Controllers.Controllers;

[Area("[Merch] Warehouses")]
[Route("api/warehouses")]
internal sealed class WarehousesController : BaseApiController
{
    private readonly ISender _sender;

    public WarehousesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("lookup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewMerchPolicy)]
    public async Task<ActionResult> GetWarehouseLookup()
    {
        var result = await _sender.Send(new GetWarehousesLookupRequest.Query());

        return Ok(result);
    }
}
