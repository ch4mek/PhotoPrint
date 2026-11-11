using Modsen.Portal.Core.Merch;

namespace Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

internal sealed class ItemSizeAmountDto
{
    public required string StorageName { get; set; }
    public required string SizeId { get; set; }
    public int Amount { get; set; }
    public StorageAvailabilityStatus? Status { get; set; }
}
