using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.Requests.Core.Dto;

namespace Modsen.Portal.Merch.Controllers.Contracts.Input;

internal sealed class AddItemTransactionInput
{
    public MerchTransactionType TransactionType { get; set; }
    public int ItemId { get; set; }
    public IReadOnlyCollection<SizeCountDto> SizeCounts { get; set; } = [];
    public int WarehouseId { get; set; }
    public int? UserRecipientId { get; set; }
    public int? WarehouseRecipientId { get; set; }
    public string? Notes { get; set; }
}
