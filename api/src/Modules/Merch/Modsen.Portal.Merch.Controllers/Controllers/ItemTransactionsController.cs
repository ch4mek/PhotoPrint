
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Merch.Controllers.Contracts.Input;
using Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;
using Modsen.Portal.Merch.Requests.Transactions.Requests;
using Modsen.Portal.Merch.Requests.Transactions.Requests.RequestDto;

namespace Modsen.Portal.Merch.Controllers.Controllers
{
    [Area("[Merch] Transactions")]
    [Route("api/[controller]")]
    internal class ItemTransactionsController : BaseApiController
    {
        private readonly ISender _sender;

        public ItemTransactionsController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Get Transactions
        /// Allowed Roles : CTO, CEO
        /// </summary>
        /// <returns></returns>
        [HttpPost("search")]
        [ProducesResponseType(typeof(PageableListOfItems<StorageItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PortalPolicyNames.ViewMerchPolicy)]
        public async Task<ActionResult<PageableListOfItems<TransationOdpDto>>> GetItemsStorageAsync(
            ODataQueryOptions<TransationOdpDto> queryOptions,
            TransactionFilters filters)
        {

            var result = await _sender.Send(new GetTransactionsRquest.Query(queryOptions,
                filters.IdFrom, filters.IdTo), HttpContext.RequestAborted);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PortalPolicyNames.ManageTransactionsPolicy)]
        public async Task<ActionResult> AddTransaction(AddItemTransactionInput input)
        {
            var result = await _sender.Send(new AddItemTransactionRequest.Command()
            {
                TransactionType = input.TransactionType,
                ItemId = input.ItemId,
                SizeCounts = input.SizeCounts,
                WarehouseId = input.WarehouseId,
                UserRecipientId = input.UserRecipientId,
                WarehouseRecipientId = input.WarehouseRecipientId,
                Notes = input.Notes,
            }, HttpContext.RequestAborted);

            return Ok(result);
        }

        [HttpPost("kit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = PortalPolicyNames.ManageTransactionsPolicy)]
        public async Task<ActionResult> AddKitTransactions(AddKitTransactionInput input)
        {
            var result = await _sender.Send(new AddKitTransactionRequest.Command()
            {
                Amount = input.Amount,
                TransactionType = input.TransactionType,
                KitId = input.KitId,
                SizeCounts = input.SizeCounts,
                WarehouseId = input.WarehouseId,
                UserRecipientId = input.UserRecipientId,
                WarehouseRecipientId = input.WarehouseRecipientId,
                Notes = input.Notes,
            }, HttpContext.RequestAborted);

            return Ok(result);
        }
    }
}
