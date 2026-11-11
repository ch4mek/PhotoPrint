using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

internal sealed record ItemStatusDto(
    int Id,
    string Name,
    MerchItemStatus Status)
{
    public static ItemStatusDto Of(ItemEntity item)
    {
        return new ItemStatusDto(item.Id, item.Name, item.Status);
    }
}
