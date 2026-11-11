using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Core.Dto;

namespace Modsen.Portal.Merch.Requests.Core.Loaders
{
    internal interface IItemsCountLoader
    {
        Task<List<ItemSizeCountDto>> LoadAsync(int[] itemIds, int warehouseId, CancellationToken cancellationToken);
        Task<ItemSizeCountDto> LoadAsync(int itemId, int warehouseId, CancellationToken cancellationToken);
    }

    internal class ItemsCountLoader : IItemsCountLoader
    {
        private readonly IMerchDbContext _merchDbContext;

        public ItemsCountLoader(IMerchDbContext merchDbContext)
        {
            _merchDbContext = merchDbContext;
        }

        public async Task<List<ItemSizeCountDto>> LoadAsync(int[] itemIds, int warehouseId, CancellationToken cancellationToken)
        {
            var existingItemIds = await _merchDbContext.Items
                .Where(item => itemIds.Contains(item.Id))
                .Select(item => item.Id)
                .ToListAsync(cancellationToken);

            var notFoundIds = itemIds.Except(existingItemIds).ToList();
            if (notFoundIds.Count > 0)
            {
                throw new EntityNotFoundException($"Item(s) not found: {string.Join(", ", notFoundIds)}");
            }

            var isWarehouseExist = await _merchDbContext.Warehouses
                .AnyAsync(w => w.Id == warehouseId, cancellationToken);
            if (!isWarehouseExist)
            {
                throw new EntityNotFoundException("Warehouse does not exist");
            }

            var availableSizes = await _merchDbContext.ItemTransactions
                .Where(t => itemIds.Contains(t.ItemId) && t.WarehouseId == warehouseId)
                .GroupBy(t => new { t.ItemId, t.SizeId })
                .Select(g => new
                {
                    g.Key.ItemId,
                    SizeCount = new SizeCountDto
                    {
                        SizeId = g.Key.SizeId,
                        Count = g.Sum(t => t.Count)
                    }
                })
                .ToListAsync(cancellationToken);

            var minAmountSizes = await _merchDbContext.ItemSizeMinAmounts
                .Where(a => itemIds.Contains(a.ItemId) && a.WarehouseId == warehouseId)
                .Select(a => new
                {
                    a.ItemId,
                    SizeCount = new SizeCountDto
                    {
                        SizeId = a.SizeId,
                        Count = 0
                    }
                })
                .ToListAsync(cancellationToken);

            var result = new List<ItemSizeCountDto>();
            foreach (var itemId in itemIds)
            {
                var available = availableSizes
                    .Where(x => x.ItemId == itemId)
                    .Select(x => x.SizeCount)
                    .ToList();

                var minSizes = minAmountSizes
                    .Where(x => x.ItemId == itemId)
                    .Select(x => x.SizeCount)
                    .Where(x => available.All(a => a.SizeId != x.SizeId))
                    .ToList();

                result.Add(new ItemSizeCountDto
                {
                    ItemId = itemId,
                    SizeCounts = available.Concat(minSizes).ToList()
                });
            }

            return result;
        }

        public async Task<ItemSizeCountDto> LoadAsync(int itemId, int warehouseId, CancellationToken cancellationToken)
        {
            var items = await LoadAsync([itemId], warehouseId, cancellationToken);
            return items.FirstOrDefault() ?? throw new EntityNotFoundException($"Item with id {itemId} does not exist");
        }
    }
}
