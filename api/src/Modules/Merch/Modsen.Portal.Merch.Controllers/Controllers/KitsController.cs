using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Merch.Controllers.Contracts.Input;
using Modsen.Portal.Merch.Requests.Core.Dto;
using Modsen.Portal.Merch.Requests.Kits.Requests;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

namespace Modsen.Portal.Merch.Controllers.Controllers;

[Area("[Merch] Kits")]
[Route("api/kits")]
internal class KitsController : BaseApiController
{
    private readonly ISender _sender;

    public KitsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get pageable kits
    /// Allowed Roles : CTO, CEO
    /// Supports ordering on following sort columns: id, name, status, possibleKits.
    /// Supports filtering on following values: name, compound, description, amount, statusIds, storageIds
    /// </summary>
    /// <returns>Merch kits with possible amounts</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PageableListOfItems<KitStorageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewMerchPolicy)]
    public async Task<ActionResult<PageableListOfItems<KitStorageDto>>> GetKitsAsync(
        [FromQuery] int top,
        [FromQuery] int skip,
        [FromBody] KitsQueryOptionsDto queryOptions)
    {
        var query = new GetPageableKitsRequest.Query(new GetPageableKitsDto
        {
            Top = top,
            Skip = skip,
            QueryOptions = queryOptions
        });

        var pageableKits = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(pageableKits);
    }

    /// <summary>
    /// Gets a kit
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <param name="id">Identifier of kit to get</param>
    /// <returns>Kit</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(KitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<KitDto>> GetKitAsync(int id)
    {
        var kitDto = await _sender.Send(new GetKitByIdRequest.Query(id), HttpContext.RequestAborted);

        return Ok(kitDto);
    }

    /// <summary>
    /// Creates a kit
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <param name="input">Model with kit properties</param>
    /// <returns>Created kit</returns>
    [HttpPost]
    [ProducesResponseType(typeof(KitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageItemsAndKitsPolicy)]
    public async Task<ActionResult<KitDto>> CreateKitAsync([FromBody] CreateKitDto input)
    {
        var kitDto = await _sender.Send(new CreateKitRequest.Command(input), HttpContext.RequestAborted);

        return Ok(kitDto);
    }

    /// <summary>
    /// Updates a kit
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <param name="id">Identifier of kit to update</param>
    /// <param name="input">Model with kit properties</param>
    /// <returns>Updated kit</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(KitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageItemsAndKitsPolicy)]
    public async Task<ActionResult<KitDto>> UpdateKitAsync(int id, [FromBody] UpdateKitInput input)
    {
        var kitDto = await _sender.Send(new UpdateKitRequest.Command(input.ToRequestDto(id)),
            HttpContext.RequestAborted);

        return Ok(kitDto);
    }

    [HttpGet("{id:int}/warehouse/{warehouseId:int}/counts")]
    [ProducesResponseType(typeof(KitCountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewMerchPolicy)]
    public async Task<ActionResult<KitCountDto>> GetKitSizeCounts(int id, int warehouseId)
    {
        var result = await _sender.Send(new GetKitCountsByIdRequest.Query(id, warehouseId), HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Update merch item status (archive/available)
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <param name="id"> Id of item</param>
    /// <param name="input"> New status of the item</param>
    /// <returns>Status Dto</returns>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(KitStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageItemsAndKitsPolicy)]
    public async Task<ActionResult<KitStatusDto>> UpdateKitStatusByIdAsync(int id,
        [FromBody] UpdateKitStatusInput input)
    {
        ICommand<KitStatusDto> command;

        switch (input.Status)
        {
            case MerchItemStatus.Archived:
                command = new ArchiveKitRequest.Command(id);
                break;

            case MerchItemStatus.Available:
                command = new ActivateKitRequest.Command(id);
                break;

            default:
                return BadRequest("Invalid status");
        }

        var statusDto = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(statusDto);
    }

}
