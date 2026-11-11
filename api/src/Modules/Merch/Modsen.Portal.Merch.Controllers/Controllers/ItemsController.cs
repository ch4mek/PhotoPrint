using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Merch.Controllers.Contracts.Input;
using Modsen.Portal.Merch.Requests.Core.Dto;
using Modsen.Portal.Merch.Requests.Items.Requests;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

namespace Modsen.Portal.Merch.Controllers.Controllers;

[Area("[Merch] Items")]
[Route("api/items")]
internal class ItemsController : BaseApiController
{
    private readonly ISender _sender;

    public ItemsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get available filters for merch items
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <returns>Available filters Dto</returns>
    [HttpGet("filters")]
    [ProducesResponseType(typeof(AvailableFiltersDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewMerchPolicy)]
    public async Task<ActionResult<AvailableFiltersDto>> GetAvailableFiltersAsync()
    {
        var query = new GetItemsAvailableFiltersRequest.Query();

        var filtersDto = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(filtersDto);
    }

    [HttpGet("{id:int}/warehouse/{warehouseId:int}/counts")]
    [ProducesResponseType(typeof(List<SizeCountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewMerchPolicy)]
    public async Task<ActionResult<List<SizeCountDto>>> GetItemSizeCounts(int id, int warehouseId)
    {
        var result = await _sender.Send(new GetItemCountsById.Query(id, warehouseId), HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get a merch item by id
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <param name="id"> Id of item</param>
    /// <returns>Item Dto</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ItemWithAmountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewMerchPolicy)]
    public async Task<ActionResult<ItemWithAmountDto>> GetMerchItemByIdAsync(int id)
    {
        var query = new GetMerchItemByIdRequest.Query(id);

        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get pageable merch items
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <param name="queryOptions">Parameters for filtering, searching, ordering</param>
    /// <param name="queryParams">Parameters for filtering and ordering among min amount and storage</param>
    /// <returns>Pageable list of merch items</returns>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PageableListOfItems<OdpItemEntityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewMerchPolicy)]
    public async Task<ActionResult<PageableListOfItems<OdpItemEntityDto>>> GetPageableMerchItemsAsync(
        ODataQueryOptions<OdpItemEntityDto> queryOptions,
        [FromBody] GetPageableMerchItemsRequest.ExtendedQueryParams queryParams)
    {
        var result = await _sender.Send(new GetPageableMerchItemsRequest.Query(queryOptions, queryParams),
            HttpContext.RequestAborted);

        return Ok(result);
    }

    /// <summary>
    /// Get a merch items lookup
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <returns>List of item lookup dtos</returns>
    [HttpGet("lookup")]
    [ProducesResponseType(typeof(List<ItemLookupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ViewMerchPolicy)]
    public async Task<ActionResult<List<ItemLookupDto>>> GetMerchItemsLookupAsync()
    {
        var query = new GetItemsLookupRequest.Query();

        var itemsLookup = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(itemsLookup);
    }

    /// <summary>
    /// Creates a merch item
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <param name="input">Model with item properties</param>
    /// <returns>Created item</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageItemsAndKitsPolicy)]
    public async Task<ActionResult<ItemDto>> CreateMerchItemAsync([FromBody] CreateItemDto input)
    {
        var itemDto = await _sender.Send(new CreateMerchItemRequest.Command(input), HttpContext.RequestAborted);

        return Ok(itemDto);
    }

    /// <summary>
    /// Updates a merch item
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <param name="input">Model with item properties</param>
    /// <returns>Updated item</returns>
    [HttpPut]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageItemsAndKitsPolicy)]
    public async Task<ActionResult<ItemDto>> UpdateMerchItemAsync([FromBody] UpdateItemDto input)
    {
        var itemDto = await _sender.Send(new UpdateMerchItemRequest.Command(input), HttpContext.RequestAborted);

        return Ok(itemDto);
    }

    /// <summary>
    /// Update merch item status (archive/available)
    /// Allowed Roles : CTO, CEO
    /// </summary>
    /// <param name="id"> Id of item</param>
    /// <param name="input"> New status of the item</param>
    /// <returns>Status Dto</returns>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(ItemStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ManageItemsAndKitsPolicy)]
    public async Task<ActionResult<ItemStatusDto>> UpdateMerchItemStatusByIdAsync(int id,
        [FromBody] UpdateItemStatusInput input)
    {
        ICommand<ItemStatusDto> command;

        switch (input.Status)
        {
            case MerchItemStatus.Archived:
                command = new ArchiveItemRequest.Command(id);
                break;

            case MerchItemStatus.Available:
                command = new ActivateItemRequest.Command(id);
                break;

            default:
                return BadRequest("Invalid status");
        }

        var statusDto = await _sender.Send(command, HttpContext.RequestAborted);

        return Ok(statusDto);
    }
}
