using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

internal sealed record ItemDto(
    int Id,
    string Name,
    bool IsClothing,
    string? Supplier,
    MerchItemStatus Status,
    string? Description,
    string? PhotoFileId,
    bool? IsPartOfAvailableKit,
    IReadOnlyCollection<SizeMinAmountDto> SizeMinAmounts)
{
    public static ItemDto Of(ItemEntity item, List<ItemSizeMinAmountEntity> sizeMinAmounts,
        bool? isPartOfAvailableKit = null)
    {
        return new ItemDto(
            Id: item.Id,
            Name: item.Name,
            IsClothing: item.IsClothing,
            Supplier: item.Supplier,
            Status: item.Status,
            Description: item.Description,
            PhotoFileId: item.PhotoFileId,
            IsPartOfAvailableKit: isPartOfAvailableKit,
            SizeMinAmounts: sizeMinAmounts
                .Select(x => new SizeMinAmountDto(x.SizeId, x.WarehouseId, x.MinAmount))
                .ToList());
    }
}
