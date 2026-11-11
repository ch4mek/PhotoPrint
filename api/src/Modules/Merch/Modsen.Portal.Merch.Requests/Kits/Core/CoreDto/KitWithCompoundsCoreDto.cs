using Modsen.Portal.Core.Merch;

namespace Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

internal sealed class KitWithCompoundsCoreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? PhotoFileId { get; set; }
    public string? Description { get; set; }

    public bool ContainsClothing { get; set; }

    public int TotalPossibleKits { get; set; }
    public string[]? AllStorages { get; set; }
    public StorageAvailabilityStatus Status { get; set; }

    public IReadOnlyCollection<KitCompoundCoreDto> Compounds { get; set; } = [];
    public IReadOnlyCollection<KitSizeInStoragePossibleAmountCoreDto> PossibleKits { get; set; } = [];
    public IReadOnlyCollection<StorageAvailabilityStatusCoreDto> StorageAvailabilityStatuses { get; set; } = [];
}
