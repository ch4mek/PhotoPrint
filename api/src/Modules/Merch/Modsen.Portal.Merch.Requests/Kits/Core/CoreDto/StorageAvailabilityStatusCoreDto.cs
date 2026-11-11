using Modsen.Portal.Core.Merch;

namespace Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

internal sealed class StorageAvailabilityStatusCoreDto
{
    public required string StorageName { get; set; }
    public StorageAvailabilityStatus? Status { get; set; }
}
