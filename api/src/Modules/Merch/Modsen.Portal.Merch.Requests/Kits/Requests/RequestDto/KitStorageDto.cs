using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;
using Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

internal class KitStorageDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? PhotoFileId { get; set; }
    public string? Description { get; set; }

    public bool ContainsClothing { get; set; }

    public int TotalPossibleKits { get; set; }
    public string[]? AllStorages { get; set; }
    public StorageAvailabilityStatus Status { get; set; }

    public IReadOnlyCollection<KitCompoundLookupDto>? Compounds { get; set; }
    public IReadOnlyCollection<KitSizeInStoragePossibleAmountDto>? PossibleKits { get; set; }
    public IReadOnlyCollection<StorageAvailabilityStatusDto>? StorageAvailabilityStatuses { get; set; }

    public static KitStorageDto Of(KitWithCompoundsCoreDto coreDto)
    {
        return new KitStorageDto
        {
            Id = coreDto.Id,
            Name = coreDto.Name,
            PhotoFileId = coreDto.PhotoFileId,
            Description = coreDto.Description,
            ContainsClothing = coreDto.ContainsClothing,
            AllStorages = coreDto.AllStorages,
            TotalPossibleKits = coreDto.TotalPossibleKits,
            Status = coreDto.Status,
            Compounds = coreDto.Compounds.Select(KitCompoundLookupDto.Of).ToArray(),
            PossibleKits = coreDto.PossibleKits.Select(KitSizeInStoragePossibleAmountDto.Of).ToArray(),
            StorageAvailabilityStatuses = coreDto.StorageAvailabilityStatuses
                .Select(StorageAvailabilityStatusDto.Of).ToArray()
        };
    }
}
