using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

internal sealed record ItemWithAmountDto(
    int Id,
    string Name,
    bool IsClothing,
    string? Supplier,
    MerchItemStatus Status,
    string? Description,
    string? PhotoFileId,
    bool? IsPartOfAvailableKit,
    IReadOnlyCollection<SizeAmountDto> SizeAmounts)
{
    public static ItemWithAmountDto Of(ItemEntity item, List<ItemSizeMinAmountEntity> sizeMinAmounts,
        Dictionary<(string SizeId, int StarageId), int> availableAmounts,
        bool? isPartOfAvailableKit = null)
    {
        return new ItemWithAmountDto(
            Id: item.Id,
            Name: item.Name,
            IsClothing: item.IsClothing,
            Supplier: item.Supplier,
            Status: item.Status,
            Description: item.Description,
            PhotoFileId: item.PhotoFileId,
            IsPartOfAvailableKit: isPartOfAvailableKit,
            SizeAmounts: sizeMinAmounts
                .Select(x => new SizeAmountDto(
                    x.SizeId,
                    x.WarehouseId,
                    availableAmounts.GetValueOrDefault((x.SizeId, x.WarehouseId), 0),
                    x.MinAmount))
                .ToList());
    }
}
