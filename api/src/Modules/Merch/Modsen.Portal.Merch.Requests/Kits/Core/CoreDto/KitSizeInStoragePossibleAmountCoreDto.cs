using Modsen.Portal.Core.Merch;

namespace Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

internal sealed class KitSizeInStoragePossibleAmountCoreDto
{
    public required string StorageName { get; set; }
    public required string SizeId { get; set; }
    public int PossibleKitAmount { get; set; }
    public StorageAvailabilityStatus? Status { get; set; }
}
