using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.Requests.Items.Core.DbQueries;

internal interface IGetItemSizeMinAmountsDbQuery
{
    Task<List<ItemSizeMinAmountEntity>> ExecuteAsync(int itemId, CancellationToken cancellationToken);
}

internal sealed class GetItemSizeMinAmountsDbQuery : IGetItemSizeMinAmountsDbQuery
{
    private readonly IMerchDbContext _dbContext;

    public GetItemSizeMinAmountsDbQuery(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<ItemSizeMinAmountEntity>> ExecuteAsync(int itemId, CancellationToken cancellationToken)
    {
        return _dbContext.ItemSizeMinAmounts
            .AsNoTracking()
            .Where(minAmount => minAmount.ItemId == itemId)
            .OrderBy(minAmount => minAmount.WarehouseId)
            .ThenBy(minAmount => minAmount.MerchSize!.OrderWeight)
            .ToListAsync(cancellationToken);
    }
}
