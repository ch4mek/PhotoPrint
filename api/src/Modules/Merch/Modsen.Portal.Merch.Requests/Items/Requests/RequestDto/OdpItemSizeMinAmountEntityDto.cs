using System.Linq.Expressions;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

internal class OdpItemSizeMinAmountEntityDto
{
    public string SizeId { get; set; } = null!;

    public int SizeOrderWeight { get; set; }

    public int WarehouseId { get; set; }

    public string? WarehouseLocation { get; set; }

    public int MinAmount { get; set; }

    public static readonly Expression<Func<ItemSizeMinAmountEntity, OdpItemSizeMinAmountEntityDto>> Map =
        x => new OdpItemSizeMinAmountEntityDto
        {
            SizeId = x.SizeId,
            WarehouseId = x.WarehouseId,
            WarehouseLocation = x.Warehouse!.Location,
            MinAmount = x.MinAmount,
            SizeOrderWeight = x.MerchSize!.OrderWeight
        };
}
