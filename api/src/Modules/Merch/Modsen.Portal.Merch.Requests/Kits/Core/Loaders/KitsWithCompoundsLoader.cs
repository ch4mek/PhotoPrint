using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Kits.Core.Loaders;

internal interface IKitsWithCompoundsLoader
{
    Task<List<KitWithCompoundsCoreDto>> LoadAsync(IReadOnlyCollection<int> kitsIds, IReadOnlyCollection<int>? storagesIds,
        CancellationToken cancellationToken);
}

internal sealed class KitsWithCompoundsLoader : IKitsWithCompoundsLoader
{
    private readonly IMerchDbContext _dbContext;

    public KitsWithCompoundsLoader(IMerchDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<KitWithCompoundsCoreDto>> LoadAsync(IReadOnlyCollection<int> kitsIds, IReadOnlyCollection<int>? storagesIds,
        CancellationToken cancellationToken)
    {
        return _dbContext.Kits
            .Where(kit => kitsIds.Contains(kit.Id))
            .AsSplitQuery()
            .Join(_dbContext.KitItems,
                kit => kit.Id,
                compound => compound.KitId,
                (kitEntity, compound) => new { kitEntity, compound })
            .GroupJoin(_dbContext.ItemTransactions,
                kitWithItems => kitWithItems.compound.ItemId,
                transaction => transaction.ItemId,
                (kitWithItems, transactions) => new { kitWithItems, transactions })
            .SelectMany(joinResult => joinResult.transactions.DefaultIfEmpty(),
            (joinResult, transaction) => new {joinResult.kitWithItems, transaction})

            .GroupBy(kit => new
            {
                kit.kitWithItems.kitEntity.Id,
                kit.kitWithItems.kitEntity.Name,
                kit.kitWithItems.kitEntity.Description,
                kit.kitWithItems.kitEntity.PhotoFileId,
                kit.kitWithItems.kitEntity.Status
            })

            .Select(grouping => new KitWithCompoundsCoreDto
            {
                Id = grouping.Key.Id,
                Name = grouping.Key.Name,
                PhotoFileId = grouping.Key.PhotoFileId,
                Description = grouping.Key.Description,
                Status = grouping.Key.Status == MerchItemStatus.Archived
                    ? StorageAvailabilityStatus.Archived
                    : default,

                ContainsClothing = grouping.Any(kit => kit.kitWithItems.compound.Item!.IsClothing),
                AllStorages = _dbContext.Warehouses
                    .Where(warehouse => storagesIds == null || storagesIds.Contains(warehouse.Id))
                    .Select(warehouse => warehouse.Location).ToArray(),

                Compounds = grouping
                    .GroupBy(kit => kit.kitWithItems.compound.ItemId)
                    .Select(group  => new KitCompoundCoreDto
                    {
                        Id = group.Key,
                        IsClothing = group.First().kitWithItems.compound.Item!.IsClothing,
                        Name = group.First().kitWithItems.compound.Item!.Name,
                        AmountInKit = group.First().kitWithItems.compound.Count
                    }).ToList()
            })
            .OrderBy(dto => Array.IndexOf(kitsIds.ToArray(), dto.Id))
            .ToListAsync(cancellationToken);
    }
}
