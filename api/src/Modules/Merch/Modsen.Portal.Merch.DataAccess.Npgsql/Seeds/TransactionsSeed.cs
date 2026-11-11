
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Merch.DataAccess.Npgsql.Seeds
{
    internal class TransactionsSeed : ISeedEntitiesProvider<MerchDbContext>
    {
        private readonly MerchDbContext _dbContext;
        private readonly IHostEnvironmentContext _hostEnvironmentContext;

        public TransactionsSeed(MerchDbContext dbContext, IHostEnvironmentContext hostEnvironmentContext)
        {
            _dbContext = dbContext;
            _hostEnvironmentContext = hostEnvironmentContext;
        }

        public void Seed()
        {
            if (_hostEnvironmentContext.IsProductionEnvironment())
            {
                return;
            }

            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                Receipt();
                Transfer();
                WriteOff();

                ReceiptWarsaw();
                ReceiptOne();

                _dbContext.SaveChanges();

                ValidateStockConsistency();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private void ValidateStockConsistency()
        {
            var clothingItems = _dbContext.Items
                .Where(i => i.IsClothing)
                .Select(i => new { i.Id, i.Name })
                .ToList();

            var itemIds = clothingItems.Select(i => i.Id).ToList();

            var transactions = _dbContext.ItemTransactions
                .Where(t => itemIds.Contains(t.ItemId))
                .ToList();

            var grouped = transactions
                .GroupBy(t => new { t.ItemId, t.SizeId, t.WarehouseId })
                .ToList();

            foreach (var group in grouped)
            {
                var itemId = group.Key.ItemId;
                var sizeId = group.Key.SizeId;
                var warehouseId = group.Key.WarehouseId;

                var stock = group.Sum(t => t.Count);

                if (stock < 0)
                {
                    var itemName = clothingItems.First(i => i.Id == itemId).Name;
                    throw new InvalidOperationException(
                        $"Inconsistent stock for item '{itemName}' (Size: {sizeId}, WarehouseId: {warehouseId}) — stock went negative: {stock}");
                }
            }
        }

        public void Receipt()
        {
            var sizes = new[]
            {
                "XS", "S", "M", "L", "XL", "2XL", "3XL", "4XL"
            };

            if (!_dbContext.ItemTransactionGroups.Any(x => x.Notes == "Initial receipt of Modsen T-shirt"))
            {
                var createdAt = DateTime.UtcNow;
                var createdById = 3;

                var group = new ItemTransactionGroupEntity
                {
                    CreatedById = createdById,
                    Notes = "Initial receipt of Modsen T-shirt",
                    CreatedAtUtc = createdAt,
                };

                var items = new List<ItemTransactionEntity>();
                var warehouseId = _dbContext.Warehouses.FirstOrDefault(x => x.Location == "Minsk")!.Id;

                foreach (var size in sizes)
                {
                    items.Add(new ItemTransactionEntity
                    {
                        Type = MerchTransactionType.Receipt,
                        OrderWeight = ((int)MerchTransactionType.Receipt + 1) * 100,
                        Count = 10,
                        CreatedAtUtc = createdAt,
                        CreatedById = createdById,
                        Item = _dbContext.Items.FirstOrDefault(x => x.Name == "Modsen T-shirt"),
                        Sender = null,
                        WarehouseRecipientId = warehouseId,
                        WarehouseId = warehouseId,
                        SizeId = size,
                    });
                }

                group.ItemTransactions = items;
                _dbContext.ItemTransactionGroups.Add(group);
            }
        }

        public void ReceiptOne()
        {
            for (var i = 0; i < 5; i++)
            {

                if (!_dbContext.ItemTransactionGroups.Any(x => x.Notes == $"Initial receipt of 1 year anniversary at Modsen postcard [{i}]"))
                {
                    var createdAt = DateTime.UtcNow;
                    var createdById = 3;

                    var group = new ItemTransactionGroupEntity
                    {
                        CreatedById = createdById,
                        Notes = $"Initial receipt of 1 year anniversary at Modsen postcard [{i}]",
                        CreatedAtUtc = createdAt,
                    };

                    var items = new List<ItemTransactionEntity>();
                    var warehouseId = i % 2 == 0 ?
                        _dbContext.Warehouses.FirstOrDefault(x => x.Location == "Vitebsk")!.Id
                        : _dbContext.Warehouses.FirstOrDefault(x => x.Location == "Minsk")!.Id;

                    items.Add(new ItemTransactionEntity
                    {
                        Type = MerchTransactionType.Receipt,
                        OrderWeight = ((int)MerchTransactionType.Receipt + 1) * 100,
                        Count = 20,
                        CreatedAtUtc = createdAt,
                        CreatedById = createdById,
                        Item = _dbContext.Items.FirstOrDefault(x => x.Name == "1 year anniversary at Modsen postcard"),
                        Sender = null,
                        WarehouseRecipientId = warehouseId,
                        WarehouseId = warehouseId,
                        SizeId = "One",
                    });

                    group.ItemTransactions = items;
                    _dbContext.ItemTransactionGroups.Add(group);
                }
            }
        }


        public void ReceiptWarsaw()
        {
            var sizes = new[]
            {
                "XS", "S", "M", "L",
            };

            if (!_dbContext.ItemTransactionGroups.Any(x => x.Notes == "Initial receipt of New Year 2026 black hoodie"))
            {
                var createdAt = DateTime.UtcNow;
                var createdById = 3;

                var group = new ItemTransactionGroupEntity
                {
                    CreatedById = createdById,
                    Notes = "Initial receipt of New Year 2026 black hoodie",
                    CreatedAtUtc = createdAt,
                };

                var items = new List<ItemTransactionEntity>();
                var warehouseId = _dbContext.Warehouses.FirstOrDefault(x => x.Location == "Warsaw")!.Id;
                var counter = 0;
                foreach (var size in sizes)
                {
                    items.Add(new ItemTransactionEntity
                    {
                        Type = MerchTransactionType.Receipt,
                        OrderWeight = ((int)MerchTransactionType.Receipt + 1) * 100,
                        Count = ++counter,
                        CreatedAtUtc = createdAt,
                        CreatedById = createdById,
                        Item = _dbContext.Items.FirstOrDefault(x => x.Name == "New Year 2026 black hoodie"),
                        Sender = null,
                        WarehouseRecipientId = warehouseId,
                        WarehouseId = warehouseId,
                        SizeId = size,
                    });
                }

                group.ItemTransactions = items;
                _dbContext.ItemTransactionGroups.Add(group);
            }
        }

        public void Transfer()
        {
            var sizes = new[]
            {
                "XS", "S", "M"
            };

            if (!_dbContext.ItemTransactionGroups.Any(x => x.Notes == "Transfer of Modsen T-shirt from Minsk to Vitebsk"))
            {
                var createdAt = DateTime.UtcNow;
                var createdById = 3;

                var group = new ItemTransactionGroupEntity
                {
                    CreatedAtUtc = createdAt,
                    CreatedById = createdById,
                    Notes = "Transfer of Modsen T-shirt from Minsk to Vitebsk",
                };


                var recipientId = _dbContext.Warehouses.FirstOrDefault(x => x.Location == "Vitebsk")!.Id;
                var senderId = _dbContext.Warehouses.FirstOrDefault(x => x.Location == "Minsk")!.Id;
                var items = new List<ItemTransactionEntity>();
                foreach (var size in sizes)
                {
                    items.Add(new ItemTransactionEntity
                    {
                        Type = MerchTransactionType.Transfer,
                        OrderWeight = ((int)MerchTransactionType.Transfer + 1) * 100,
                        Count = -4,
                        CreatedAtUtc = createdAt,
                        CreatedById = 3,
                        Item = _dbContext.Items.FirstOrDefault(x => x.Name == "Modsen T-shirt"),
                        SenderId = senderId,
                        WarehouseRecipientId = recipientId,
                        WarehouseId = senderId,
                        SizeId = size,
                    });

                    items.Add(new ItemTransactionEntity
                    {
                        Type = MerchTransactionType.Transfer,
                        OrderWeight = ((int)MerchTransactionType.Transfer + 1) * 100,
                        Count = 4,
                        CreatedAtUtc = createdAt,
                        CreatedById = 3,
                        Item = _dbContext.Items.FirstOrDefault(x => x.Name == "Modsen T-shirt"),
                        SenderId = senderId,
                        WarehouseRecipientId = recipientId,
                        WarehouseId = recipientId,
                        SizeId = size,
                    });
                }

                group.ItemTransactions = items;
                _dbContext.ItemTransactionGroups.Add(group);
            }
        }

        public void WriteOff()
        {
            var sizes = new[]
            {
                "L", "XL", "2XL"
            };

            if (!_dbContext.ItemTransactionGroups.Any(x => x.Notes == "Write off of Modsen T-shirt"))
            {
                var createdAt = DateTime.UtcNow;
                var createdById = 3;
                var senderId = _dbContext.Warehouses.FirstOrDefault(x => x.Location == "Minsk")!.Id;
                var group = new ItemTransactionGroupEntity
                {
                    CreatedAtUtc = createdAt,
                    CreatedById = 3,
                    Notes = "Write off of Modsen T-shirt",
                };
                var items = new List<ItemTransactionEntity>();
                foreach (var size in sizes)
                {
                    items.Add(new ItemTransactionEntity
                    {
                        Type = MerchTransactionType.WriteOff,
                        OrderWeight = ((int)MerchTransactionType.WriteOff + 1) * 100,
                        Count = -3,
                        CreatedAtUtc = createdAt,
                        CreatedById = createdById,
                        Item = _dbContext.Items.FirstOrDefault(x => x.Name == "Modsen T-shirt"),
                        SenderId = senderId,
                        UserRecipientId = 3,
                        WarehouseId = senderId,
                        SizeId = size,
                    });
                }

                group.ItemTransactions = items;
                _dbContext.ItemTransactionGroups.Add(group);
            }
        }
    }
}
