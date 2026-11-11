
using Modsen.Portal.Core.Merch;

namespace Modsen.Portal.Merch.Requests.Transactions.Requests.RequestDto;

internal class TransationOdpDto
{
    public int SumAmount { get; set; }
    public string? Creator { get; set; }
    public string? ElementFileId { get; set; }
    public string? ElementName { get; set; }
    public string? ElementDescription { get; set; }
    public string? SenderName { get; set; }
    public string? Recipient { get; set; }
    public bool IsClothing { get; set; }
    public DateTime Date { get; set; }
    public Guid GroupCode { get; set; }
    public string? Notes { get; set; }
    public MerchTransactionType Type { get; set; }
    public int OrderWeight { get; set; }
    public IEnumerable<TransactionDto> Transactions { get; set; } = [];
    public IEnumerable<string> WarehouseNames { get; set; } = [];
    public int OrderTransactionId { get; set; }
    public int TransactionIdFrom { get; set; }
    public int TransactionIdTo { get; set; }
}
