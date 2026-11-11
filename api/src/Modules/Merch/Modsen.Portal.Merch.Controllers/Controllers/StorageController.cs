using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;
using Modsen.Portal.Merch.Requests.Storage.Requests;
using Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

namespace Modsen.Portal.Merch.Controllers.Controllers;

[Area("[Merch] Storage")]
[Route("api/storage")]
internal class StorageController : BaseApiController
{
    private readonly ISender _sender;

    public StorageController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get storage items
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(PageableListOfItems<KitStorageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewMerchPolicy)]
    public async Task<ActionResult<PageableListOfItems<KitStorageDto>>> GetItemsStorageAsync(
        [FromQuery] int top,
        [FromQuery] int skip,
        [FromBody] StorageQueryOptionsDto queryOptions)
    {
        var query = new GetItemsStorageRequest.Query(new GetItemStorageDto
        {
            Top = top,
            Skip = skip,
            QueryOptions = queryOptions
        });

        var storageItems = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(storageItems);
    }
}
