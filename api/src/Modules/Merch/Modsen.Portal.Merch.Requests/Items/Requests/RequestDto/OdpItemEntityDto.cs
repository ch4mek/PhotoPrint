using System.Linq.Expressions;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
namespace Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

internal class OdpItemEntityDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Supplier { get; set; }

    public string? Status { get; set; }

    public bool IsClothing { get; set; }

    public string? Description { get; set; }

    public string? PhotoFileId { get; set; }

    public List<OdpItemSizeMinAmountEntityDto> OdpItemSizeMinAmounts { get; set; } = [];

    public static readonly Expression<Func<ItemEntity, OdpItemEntityDto>> Map =
        x => new OdpItemEntityDto
        {
            Id = x.Id,
            Status = x.Status.ToString(),
            IsClothing = x.IsClothing,
            Description = x.Description,
            Name = x.Name,
            Supplier = x.Supplier,
            PhotoFileId = x.PhotoFileId,
            OdpItemSizeMinAmounts = x.ItemSizeMinAmounts
                .OrderBy(f => f.MerchSize!.OrderWeight)
                .ThenBy(f => f.WarehouseId)
                .AsQueryable()
                .Select(OdpItemSizeMinAmountEntityDto.Map)
                .ToList()
        };
}
