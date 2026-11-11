using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Items.Core.CoreDto;

internal sealed record SizeMinAmountCoreDto(
    string SizeId,
    int StorageId,
    int MinAmount)
{
    public static SizeMinAmountCoreDto Of(SizeMinAmountDto source)
    {
        return new SizeMinAmountCoreDto(source.SizeId, source.StorageId, source.MinAmount);
    }

    public ItemSizeMinAmountEntity ToDatabaseEntity(int itemId)
    {
        return new ItemSizeMinAmountEntity
        {
            MinAmount = MinAmount,
            ItemId = itemId,
            WarehouseId = StorageId,
            SizeId = SizeId
        };
    }
}
