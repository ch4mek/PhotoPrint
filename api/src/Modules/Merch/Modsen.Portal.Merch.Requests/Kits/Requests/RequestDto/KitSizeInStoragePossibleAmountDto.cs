using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

internal sealed class KitSizeInStoragePossibleAmountDto
{
    public required string StorageName { get; set; }
    public required string SizeId { get; set; }
    public int PossibleKitAmount { get; set; }
    public StorageAvailabilityStatus? Status { get; set; }

    public static KitSizeInStoragePossibleAmountDto Of(KitSizeInStoragePossibleAmountCoreDto coreDto)
    {
        return new KitSizeInStoragePossibleAmountDto
        {
            StorageName = coreDto.StorageName,
            SizeId = coreDto.SizeId,
            PossibleKitAmount = coreDto.PossibleKitAmount,
            Status = coreDto.Status
        };
    }
}
