using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

internal sealed record KitMinAmountDto(
    int StorageId,
    int MinimumAmount)
{
    public KitMinAmountEntity ToEntity()
    {
        return new KitMinAmountEntity { WarehouseId = StorageId, MinAmount = MinimumAmount };
    }
}
