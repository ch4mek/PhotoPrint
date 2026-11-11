using Modsen.Portal.Core.Merch;

namespace Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

internal sealed class StorageItemDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required bool IsClothing { get; set; }
    public string? PhotoFileId { get; set; }
    public string? Supplier { get; set; }
    public string? Description { get; set; }
    public int TotalAmount { get; set; }
    public StorageAvailabilityStatus Status { get; set; }
    public IReadOnlyCollection<string>? AllStorages { get; set; }
    public IReadOnlyCollection<ItemSizeAmountDto>? SizeAmounts { get; set; }
    public IReadOnlyCollection<StorageAvailabilityStatusDto>? StorageAvailabilityStatuses { get; set; }
}

