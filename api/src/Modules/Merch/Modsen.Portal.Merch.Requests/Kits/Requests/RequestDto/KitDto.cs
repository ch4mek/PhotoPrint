using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

internal sealed record KitDto(
    int Id,
    string Name,
    MerchItemStatus Status,
    string? PhotoFileId,
    string? Description,
    IReadOnlyCollection<KitCompoundDto> Compounds,
    IReadOnlyCollection<KitMinAmountDto> MinAmounts)
{
    public static KitDto Of(KitEntity entity, List<ItemLookupCoreDto> compounds)
    {
        return new KitDto(
            Id: entity.Id,
            Name: entity.Name,
            Status: entity.Status,
            Description: entity.Description,
            PhotoFileId: entity.PhotoFileId,
            Compounds: entity.KitItems
                .Select(kitItem => new KitCompoundDto(
                    kitItem.ItemId,
                    compounds.FirstOrDefault(item => item.Id == kitItem.ItemId)!.IsClothing,
                    compounds.FirstOrDefault(item => item.Id == kitItem.ItemId)!.Name,
                    kitItem.Count))
                .ToList(),
            MinAmounts: entity.KitMinAmounts
                .Select(amount => new KitMinAmountDto(amount.WarehouseId, amount.MinAmount))
                .ToList());
    }
}
