using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Items.Core.DbQueries;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Items.Requests;

internal sealed class GetMerchItemByIdRequest : IQueryHandler<GetMerchItemByIdRequest.Query, ItemWithAmountDto>
{
    private readonly IGetItemSizeMinAmountsDbQuery _getItemSizeMinAmountsDbQuery;
    private readonly IGetMerchItemByIdDbQuery _getMerchItemById;
    private readonly IMerchDbContext _merchDbContext;

    public GetMerchItemByIdRequest(IGetMerchItemByIdDbQuery getMerchItemById,
        IGetItemSizeMinAmountsDbQuery getItemSizeMinAmountsDbQuery,
        IMerchDbContext merchDbContext)
    {
        _getItemSizeMinAmountsDbQuery = getItemSizeMinAmountsDbQuery;
        _merchDbContext = merchDbContext;
        _getMerchItemById = getMerchItemById;
    }

    public async Task<ItemWithAmountDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var item = await _getMerchItemById.ExecuteAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.MerchItemNotFound);

        var storageItemAmounts = await _merchDbContext.ItemTransactions
            .Where(x => x.ItemId == request.Id)
            .GroupBy(x => new {x.SizeId, StarageId = x.WarehouseId})
            .Select(x => new { x.Key, Sum = x.Sum(y => y.Count) })
            .ToDictionaryAsync(x => (x.Key.SizeId, x.Key.StarageId), x => x.Sum, cancellationToken: cancellationToken);

        var minAmounts = await _getItemSizeMinAmountsDbQuery
            .ExecuteAsync(item.Id, cancellationToken);

        var kitsIds = await _merchDbContext.KitItems
            .Where(kitItem => kitItem.ItemId == item.Id && kitItem.Kit!.Status == MerchItemStatus.Available)
            .Select(kitItem => kitItem.KitId)
            .ToListAsync(cancellationToken);

        var isPartOfKit = kitsIds.Count > 0;

        return ItemWithAmountDto.Of(item, minAmounts, storageItemAmounts, isPartOfKit);
    }

    internal sealed record Query(int Id) : IQuery<ItemWithAmountDto>;
}
