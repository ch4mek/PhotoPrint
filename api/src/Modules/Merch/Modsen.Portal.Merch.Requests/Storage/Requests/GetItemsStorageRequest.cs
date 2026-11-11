using System.Globalization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Storage.Core.Loaders;
using Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Storage.Requests;

internal sealed class GetItemsStorageRequest
    : IQueryHandler<GetItemsStorageRequest.Query, PageableListOfItems<StorageItemDto>>
{
    private readonly IStorageItemsLoader _itemsLoader;
    private readonly IMerchDbContext _merchDbContext;

    public GetItemsStorageRequest(IMerchDbContext merchDbContext, IStorageItemsLoader itemsLoader)
    {
        _merchDbContext = merchDbContext;
        _itemsLoader = itemsLoader;
    }

    public async Task<PageableListOfItems<StorageItemDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.RequestDto.QueryOptions;

        var items = await _itemsLoader.LoadAsync(queryOptions, cancellationToken);

        await FillItemsWithoutTransactions(items, queryOptions, cancellationToken);

        items = ApplyItemsFiltering(items, queryOptions);
        items = ApplyItemsSorting(items, queryOptions);

        var totalCount = items.Count;

        items = items
            .Skip(request.RequestDto.Skip)
            .Take(request.RequestDto.Top)
            .ToList();

        return items.ToPageable()
            .WithPaging(
                page: (request.RequestDto.Skip / request.RequestDto.Top) + 1,
                pageSize: request.RequestDto.Top)
            .WithTotal(totalCount);
    }

    private async Task FillItemsWithoutTransactions(List<StorageItemDto> items, StorageQueryOptionsDto? queryOptions,
        CancellationToken cancellationToken)
    {
        var ids = queryOptions?.StorageIds ?? [];
        var storages = await _merchDbContext.Warehouses
                .Where(warehouse => ids.Count == 0 || ids.Contains(warehouse.Id))
                .Select(warehouse => new {warehouse.Location, warehouse.Id})
                .ToArrayAsync(cancellationToken);

        var sizes = await _merchDbContext.MerchSizes.ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            item.AllStorages = storages.Select(storage => storage.Location).ToArray();

            var itemSizes = sizes
                .Where(size => size.IsForClothing == item.IsClothing)
                .ToList();

            var itemMinAmounts = await _merchDbContext.ItemSizeMinAmounts
                .Where(minAmount => minAmount.ItemId == item.Id)
                .ToListAsync(cancellationToken);

            var sizesAmounts = new List<ItemSizeAmountDto>();

            foreach (var storage in storages)
            {
                var amountsWithStatuses = itemSizes
                    .OrderBy(size => size.OrderWeight)
                    .Select(size => new ItemSizeAmountDto
                    {
                        Amount = item.SizeAmounts?.FirstOrDefault(amount =>
                            amount.SizeId == size.Id && amount.StorageName == storage.Location)?.Amount ?? 0,
                        SizeId = size.Id,
                        StorageName = storage.Location,
                        Status = GetItemStatus(item, size, storage.Id, storage.Location, itemMinAmounts),
                    }).ToList();

                sizesAmounts.AddRange(amountsWithStatuses);
            }

            item.SizeAmounts = sizesAmounts.ToArray();
            item.StorageAvailabilityStatuses = sizesAmounts
                .GroupBy(amount => amount.StorageName)
                .Select(grouping => new StorageAvailabilityStatusDto
                {
                    StorageName = grouping.Key,
                    Status = GetItemStatus(grouping.ToArray()),
                })
                .ToArray();
            item.Status = GetItemStatus(sizesAmounts);
        }
    }

    private static StorageAvailabilityStatus GetItemStatus(StorageItemDto item, MerchSizeEntity size,
        int storageId, string storageLocation, IReadOnlyCollection<ItemSizeMinAmountEntity> itemMinAmounts)
    {
        if (item.Status == StorageAvailabilityStatus.Archived)
        {
            return StorageAvailabilityStatus.Archived;
        }

        var sizeAmount = item.SizeAmounts?
                .Where(x => x.SizeId == size.Id)
                .FirstOrDefault(x => x.StorageName == storageLocation);
        if (sizeAmount?.Status != null)
        {
            return sizeAmount.Status.Value;
        }

        var itemMinAmount = itemMinAmounts
                .Where(x => x.SizeId == size.Id)
                .FirstOrDefault(minAmount => minAmount.WarehouseId == storageId);

        return itemMinAmount is { MinAmount: > 0 }
            ? StorageAvailabilityStatus.RunningOut
            : StorageAvailabilityStatus.Available;
    }

    private static StorageAvailabilityStatus GetItemStatus(IReadOnlyCollection<ItemSizeAmountDto> sizesAmounts)
    {
        if (sizesAmounts.Any(amount => amount.Status == StorageAvailabilityStatus.Archived))
        {
            return StorageAvailabilityStatus.Archived;
        }

        return sizesAmounts.Any(amount => amount.Status == StorageAvailabilityStatus.RunningOut)
            ? StorageAvailabilityStatus.RunningOut
            : StorageAvailabilityStatus.Available;
    }

    private static List<StorageItemDto> ApplyItemsFiltering(List<StorageItemDto> items,
        StorageQueryOptionsDto? queryOptions)
    {
        if (queryOptions?.StatusIds is not null)
        {
            items = items.Where(item => queryOptions.StatusIds.Contains(item.Status)).ToList();
        }

        if (queryOptions?.AmountMin is not null)
        {
            items = items.Where(item => item.TotalAmount >= queryOptions.AmountMin).ToList();
        }

        if (queryOptions?.AmountMax is not null)
        {
            items = items.Where(item => item.TotalAmount <= queryOptions.AmountMax).ToList();
        }

        return items;
    }

    private static List<StorageItemDto> ApplyItemsSorting(List<StorageItemDto> items,
        StorageQueryOptionsDto? queryOptions)
    {
        if (queryOptions?.SortColumn == "status")
        {
            items = queryOptions.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "desc"
                ? items.OrderByDescending(item => item.Status).ToList()
                : items.OrderBy(item => item.Status).ToList();
        }

        return items;
    }

    internal sealed record Query(GetItemStorageDto RequestDto) : IQuery<PageableListOfItems<StorageItemDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(optionsDto => optionsDto.RequestDto.Skip)
                .NotNull()
                .GreaterThanOrEqualTo(0);

            RuleFor(optionsDto => optionsDto.RequestDto.Top)
                .NotNull()
                .GreaterThanOrEqualTo(1);

            RuleFor(query => query.RequestDto.QueryOptions).SetValidator(new StorageQueryOptionsDto.Validator());
        }
    }
}
