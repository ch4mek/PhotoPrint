using System.Globalization;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Storage.Core.Loaders;

internal interface IStorageItemsLoader
{
    Task<List<StorageItemDto>> LoadAsync(StorageQueryOptionsDto? queryOptions, CancellationToken cancellationToken);
}

internal sealed class StorageItemsLoader : IStorageItemsLoader
{
    private readonly IMerchDbContext _merchDbContext;

    public StorageItemsLoader(IMerchDbContext merchDbContext)
    {
        _merchDbContext = merchDbContext;
    }

    public async Task<List<StorageItemDto>> LoadAsync(StorageQueryOptionsDto? queryOptions,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(queryOptions);

        var query = from item in _merchDbContext.Items

            join transaction in _merchDbContext.ItemTransactions
                on item.Id equals transaction.ItemId into transactionGroup
            from transaction in transactionGroup.DefaultIfEmpty()

            join minAmount in _merchDbContext.ItemSizeMinAmounts
                on new { item.Id, transaction.SizeId, transaction.Warehouse!.Location }
                equals new { Id = minAmount.ItemId, minAmount.SizeId, minAmount.Warehouse!.Location }
                into minAmountGroup
            from minAmount in minAmountGroup.DefaultIfEmpty()

            group new { item, transaction, minAmount } by new
            {
                item.Id,
                item.Name,
                item.Description,
                item.PhotoFileId,
                item.Supplier,
                item.IsClothing,
                item.Status
            }
            into g

            select new StorageItemDto
            {
                Id = g.Key.Id,
                Name = g.Key.Name,
                Description = g.Key.Description,
                PhotoFileId = g.Key.PhotoFileId,
                Supplier = g.Key.Supplier,
                IsClothing = g.Key.IsClothing,

                SizeAmounts = g
                    .Where(x => x.transaction != null)
                    .Where(x => queryOptions.StorageIds == null ||
                        queryOptions.StorageIds.Any(id=> id == x.transaction.WarehouseId))
                    .GroupBy(x => new
                        {
                            x.transaction.Warehouse!.Location,
                            x.transaction.SizeId,
                            x.transaction.WarehouseId,
                            x.transaction.Size!.OrderWeight
                        })
                    .OrderBy(grouping => grouping.Key.WarehouseId)
                    .ThenBy(grouping => grouping.Key.OrderWeight)
                    .Select(sizeGroup => new ItemSizeAmountDto
                        {
                        StorageName = sizeGroup.Key.Location,
                        SizeId = sizeGroup.Key.SizeId,
                        Amount = sizeGroup.Sum(x => x.transaction.Count),
                        Status = g.Key.Status == MerchItemStatus.Archived ? StorageAvailabilityStatus.Archived :
                            sizeGroup.Sum(x => x.transaction.Count) <
                            sizeGroup.First().minAmount.MinAmount
                            ? StorageAvailabilityStatus.RunningOut
                            : StorageAvailabilityStatus.Available
                        }
                    )
                    .ToArray(),

                TotalAmount = g
                    .Where(x => x.transaction != null)
                    .Where(x => queryOptions.StorageIds == null ||
                        queryOptions.StorageIds.Any(id => id == x.transaction.WarehouseId))
                    .Sum(x => x.transaction.Count),

                AllStorages = g
                    .Where(x => x.transaction != null)
                    .Where(x => queryOptions.StorageIds == null ||
                        queryOptions.StorageIds.Any(id => id == x.transaction.WarehouseId))
                    .Select(x => x.transaction.Warehouse!.Location)
                    .Distinct()
                    .ToArray(),

                StorageAvailabilityStatuses = g
                    .Where(x => x.transaction != null)
                    .Where(x => queryOptions.StorageIds == null ||
                        queryOptions.StorageIds.Any(id => id == x.transaction.WarehouseId))
                    .GroupBy(x => new { x.transaction.Warehouse!.Location, x.transaction.SizeId})
                    .Select(sizeGroup => new
                    {
                        Storage = sizeGroup.Key.Location,
                        SizeStatus = g.Key.Status == MerchItemStatus.Archived ? StorageAvailabilityStatus.Archived :
                            sizeGroup.Sum(x => x.transaction.Count)
                            < sizeGroup.First().minAmount.MinAmount
                            ? StorageAvailabilityStatus.RunningOut
                            : StorageAvailabilityStatus.Available
                    })
                    .GroupBy(x => x.Storage)
                    .Select(storageGroup => new StorageAvailabilityStatusDto
                        {
                            StorageName = storageGroup.Key,
                            Status = g.Key.Status == MerchItemStatus.Archived ? StorageAvailabilityStatus.Archived :
                                storageGroup.Any(x => x.SizeStatus == StorageAvailabilityStatus.RunningOut)
                                ? StorageAvailabilityStatus.RunningOut
                                : StorageAvailabilityStatus.Available
                        }).ToArray(),

                Status = g.Key.Status == MerchItemStatus.Archived ? StorageAvailabilityStatus.Archived : g
                    .Where(x => x.transaction != null)
                    .Where(x => queryOptions.StorageIds == null ||
                        queryOptions.StorageIds.Any(id => id == x.transaction.WarehouseId))
                    .GroupBy(x => new { x.transaction.Warehouse!.Location, x.transaction.SizeId})
                    .Select(sizeGroup => new
                    {
                        Storage = sizeGroup.Key.Location,
                        SizeStatus = g.Key.Status == MerchItemStatus.Archived ? StorageAvailabilityStatus.Archived :
                            sizeGroup.Sum(x => x.transaction.Count)
                            < sizeGroup.First().minAmount.MinAmount
                                ? StorageAvailabilityStatus.RunningOut
                                : StorageAvailabilityStatus.Available
                    })
                    .GroupBy(x => x.Storage)
                    .Any(x => x.Any(storageSizeStatus =>
                        storageSizeStatus.SizeStatus == StorageAvailabilityStatus.RunningOut))
                        ? StorageAvailabilityStatus.RunningOut
                        : StorageAvailabilityStatus.Available
            };

        query = ApplyQueryFilters(query, queryOptions);
        query = ApplyQuerySorting(query, queryOptions);

        var items = await query
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return items;
    }

    private static IQueryable<StorageItemDto> ApplyQueryFilters(IQueryable<StorageItemDto> query,
        StorageQueryOptionsDto queryOptions)
    {
        if (queryOptions.Filters is not null)
        {
            foreach (var filterDto in queryOptions.Filters.Where(x => x.FilterColumn is not null))
            {
                if (filterDto.FilterColumn == "id")
                {
                    query = int.TryParse(filterDto.FilterValue, out var id)
                        ? query.Where(storageItem => storageItem.Id == id)
                        : query.Where(storageItem => storageItem.Id == -1);
                }

                query = FilterByColumn(query, filterDto);
            }
        }

        if (queryOptions.AmountMin is not null)
        {
            Expression<Func<StorageItemDto, bool>> filter = storageItem =>
                storageItem.TotalAmount >= queryOptions.AmountMin;
            query = query.Where(filter);
        }

        if (queryOptions.AmountMax is not null)
        {
            Expression<Func<StorageItemDto, bool>> filter = storageItem =>
                storageItem.TotalAmount <= queryOptions.AmountMax;
            query = query.Where(filter);
        }

        return query;
    }

    private static IQueryable<StorageItemDto> FilterByColumn(IQueryable<StorageItemDto> query, QueryFilterDto filterDto)
    {
        return filterDto.FilterColumn switch
        {
            "name" => query.Where(storageItem => EF.Functions.ILike(storageItem.Name, $"%{filterDto.FilterValue!}%")),
            "supplier" => query.Where(storageItem => EF.Functions.ILike(storageItem.Supplier!, $"%{filterDto.FilterValue!}%")),
            "description" => query.Where(storageItem => EF.Functions.ILike(storageItem.Description!, $"%{filterDto.FilterValue!}%")),
            _ => query.Where(storageItem => storageItem.Id != -1)
        };
    }

    private static IQueryable<StorageItemDto> ApplyQuerySorting(IQueryable<StorageItemDto> query,
        StorageQueryOptionsDto queryOptions)
    {
        Expression<Func<StorageItemDto, object>> keySelector = queryOptions.SortColumn switch
        {
            "id" => storageItem => storageItem.Id,
            "name" => storageItem => storageItem.Name,
            "totalAmount" => storageItem => storageItem.TotalAmount,
            "supplier" => storageItem => storageItem.Supplier ?? string.Empty,
            _ => storageItem => storageItem.Id,
        };

        query = queryOptions.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "desc"
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);

        return query;
    }
}
