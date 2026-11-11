using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

internal sealed class StorageAvailabilityStatusDto
{
    public required string StorageName { get; set; }
    public StorageAvailabilityStatus? Status { get; set; }

    public static StorageAvailabilityStatusDto Of(StorageAvailabilityStatusCoreDto coreDto)
    {
        return new StorageAvailabilityStatusDto { StorageName = coreDto.StorageName, Status = coreDto.Status };
    }
}
