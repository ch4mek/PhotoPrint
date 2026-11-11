using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Items.Core.Composers;

internal interface IItemSizeMinAmountComposer
{
    List<ItemSizeMinAmountEntity> Compose(int itemId, ICollection<SizeMinAmountCoreDto> minAmounts);
}

internal sealed class ItemSizeMinAmountComposer : IItemSizeMinAmountComposer
{
    public List<ItemSizeMinAmountEntity> Compose(int itemId, ICollection<SizeMinAmountCoreDto> minAmounts)
    {
        var entitiesToCreate = new List<ItemSizeMinAmountEntity>();

        foreach (var minAmountDto in minAmounts)
        {
            var entity = minAmountDto.ToDatabaseEntity(itemId: itemId);

            entitiesToCreate.Add(entity);
        }

        return entitiesToCreate;
    }
}
