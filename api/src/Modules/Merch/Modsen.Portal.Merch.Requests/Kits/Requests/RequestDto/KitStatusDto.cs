using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;

internal sealed record KitStatusDto(
    int Id,
    string Name,
    MerchItemStatus Status)
{
    public static KitStatusDto Of(KitEntity item)
    {
        return new KitStatusDto(item.Id, item.Name, item.Status);
    }
}
