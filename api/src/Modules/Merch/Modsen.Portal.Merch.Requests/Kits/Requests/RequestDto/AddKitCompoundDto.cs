using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

internal sealed record AddKitCompoundDto(
    int ItemId,
    int AmountInKit)
{
    public KitItemEntity ToEntity()
    {
        return new KitItemEntity { ItemId = ItemId, Count = AmountInKit };
    }
}
