using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Kits.Core.Resolvers;

internal interface ICompoundSizesInStockResolver
{
    Task<List<CompoundSizeAmountInStockCoreDto>> ResolveAsync(IReadOnlyCollection<int> compoundIds,
        IReadOnlyCollection<int>? storagesIds, CancellationToken cancellationToken);
}

internal sealed class CompoundSizesInStockResolver : ICompoundSizesInStockResolver
{
    private readonly IMerchDbContext _dbContext;

    public CompoundSizesInStockResolver(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CompoundSizeAmountInStockCoreDto>> ResolveAsync(IReadOnlyCollection<int> compoundIds,
        IReadOnlyCollection<int>? storagesIds,
        CancellationToken cancellationToken)
    {
        var sizesInStock = await _dbContext.ItemTransactions
            .Where(transaction => compoundIds.Contains(transaction.ItemId))
            .Where(transaction => storagesIds == null || storagesIds.Any( id => id == transaction.WarehouseId))
            .GroupBy(transaction => new
            {
                transaction.ItemId,
                transaction.SizeId,
                transaction.Size!.OrderWeight,
                transaction.WarehouseId,
                transaction.Warehouse!.Location
            })
            .Select(grouping => new CompoundSizeAmountInStockCoreDto
            {
                ItemId = grouping.Key.ItemId,
                SizeOrderWeight = grouping.Key.OrderWeight,
                Amount = grouping.Sum(transaction => transaction.Count),
                StorageId = grouping.Key.WarehouseId,
                StorageName = grouping.Key.Location,
                SizeId = grouping.Key.SizeId
            })
            .ToListAsync(cancellationToken);

        var sizes = await _dbContext.MerchSizes.ToListAsync(cancellationToken);
        var storages = await _dbContext.Warehouses
            .Where(warehouse => storagesIds == null || storagesIds.Contains(warehouse.Id))
            .ToListAsync(cancellationToken);

        var itemIdsToCheck = sizesInStock.Select(amount => amount.ItemId).ToList();
        itemIdsToCheck.AddRange(compoundIds);

        await FillItems(itemIdsToCheck, sizesInStock, storages, sizes, cancellationToken);

        return sizesInStock;
    }

    private async Task FillItems(List<int> itemIdsToCheck, List<CompoundSizeAmountInStockCoreDto> sizesInStock, List<WarehouseEntity> storages,
        List<MerchSizeEntity> sizes, CancellationToken cancellationToken)
    {
        foreach (var itemId in itemIdsToCheck.Distinct())
        {
            var isItemClothing = await _dbContext.Items.Where(item => item.Id == itemId)
                .Select(item => item.IsClothing)
                .FirstOrDefaultAsync(cancellationToken);

            if (ShouldItemBeFilled(sizesInStock, itemId, storages, sizes, isItemClothing))
            {
                FillItem(sizes, isItemClothing, storages, sizesInStock, itemId);
            }
        }
    }

    private static void FillItem(List<MerchSizeEntity> sizes, bool isItemClothing, List<WarehouseEntity> storages, List<CompoundSizeAmountInStockCoreDto> sizesInStock, int itemId)
    {
        var itemSizes = sizes
            .Where(size => size.IsForClothing == isItemClothing)
            .ToList();

        foreach (var storage in storages)
        {
            foreach (var size in itemSizes)
            {
                FillSizesInStock(sizesInStock, itemId, size, storage);
            }
        }
    }

    private static void FillSizesInStock(List<CompoundSizeAmountInStockCoreDto> sizesInStock, int itemId, MerchSizeEntity size, WarehouseEntity storage)
    {
        var isStorageAndSizePairNotFilled = sizesInStock.FirstOrDefault(amount =>
            amount.ItemId == itemId &&
            amount.SizeId == size.Id &&
            amount.StorageName == storage.Location) is null;

        if (isStorageAndSizePairNotFilled)
        {
            sizesInStock.Add(new CompoundSizeAmountInStockCoreDto
            {
                ItemId = itemId,
                Amount = 0,
                SizeId = size.Id,
                SizeOrderWeight = size.OrderWeight,
                StorageName = storage.Location,
                StorageId = storage.Id
            });
        }
    }

    private static bool ShouldItemBeFilled(List<CompoundSizeAmountInStockCoreDto> sizesInStock, int itemId, List<WarehouseEntity> storages, List<MerchSizeEntity> sizes, bool isItemClothing)
    {
        var count = storages.Count * sizes.Count(size => size.IsForClothing == isItemClothing);
        return sizesInStock
            .Where(amount => amount.ItemId == itemId)
            .GroupBy(amount => amount.ItemId)
            .Count() < count;
    }
}
