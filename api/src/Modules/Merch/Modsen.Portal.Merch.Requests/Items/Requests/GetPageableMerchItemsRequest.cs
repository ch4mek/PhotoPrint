using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Items.Requests;

internal sealed class GetPageableMerchItemsRequest
    : IQueryHandler<GetPageableMerchItemsRequest.Query, PageableListOfItems<OdpItemEntityDto>>
{
    private readonly IMerchDbContext _merchDbContext;

    public GetPageableMerchItemsRequest(IMerchDbContext merchDbContext)
    {
        _merchDbContext = merchDbContext;
    }

    public async Task<PageableListOfItems<OdpItemEntityDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _merchDbContext.Items
            .AsNoTracking()
            .Select(OdpItemEntityDto.Map)
            .AsQueryable();        

        var total = await request.QueryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpItemEntityDto>()
            .CountAsync(cancellationToken);

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpItemEntityDto>.Value;
        }

        var result = await request.QueryOptions
            .ApplyTo(query)
            .Cast<OdpItemEntityDto>()
            .ToListAsync(cancellationToken);

        if (request.QueryParams.StoragesIds != null)
        {
            var storageIds = request.QueryParams.StoragesIds.ToHashSet();

            foreach (var item in result)
            {
                item.OdpItemSizeMinAmounts = item.OdpItemSizeMinAmounts
                    .Where(amount => storageIds.Contains(amount.WarehouseId))
                    .ToList();
            }
        }

        foreach (var item in result)
        {
            ApplyOrdering(request, item);
        }

        return result.ToPageable(request.QueryOptions, total);
    }

    private static void ApplyOrdering(Query request, OdpItemEntityDto item)
    {
        var amountsToSort = item.OdpItemSizeMinAmounts;
        var orderedAmounts = ApplyStorageSort(request, null, amountsToSort);

        orderedAmounts = ApplyMinAmountSort(request, orderedAmounts, amountsToSort);

        if (orderedAmounts != null)
        {
            item.OdpItemSizeMinAmounts = orderedAmounts.ToList();
        }
    }

    private static IOrderedEnumerable<OdpItemSizeMinAmountEntityDto>? ApplyStorageSort(Query request, IOrderedEnumerable<OdpItemSizeMinAmountEntityDto>? orderedAmounts,
        List<OdpItemSizeMinAmountEntityDto> amountsToSort)
    {
        if (request.QueryParams.StorageSortAscending != null)
        {
            return request.QueryParams.StorageSortAscending.Value
                ? amountsToSort.OrderBy(minAmount => minAmount.WarehouseId)
                : amountsToSort.OrderByDescending(minAmount => minAmount.WarehouseId);
        }

        return orderedAmounts;
    }

    private static IOrderedEnumerable<OdpItemSizeMinAmountEntityDto>? ApplyMinAmountSort(Query request, IOrderedEnumerable<OdpItemSizeMinAmountEntityDto>? orderedAmounts,
        List<OdpItemSizeMinAmountEntityDto> amountsToSort)
    {
        if (request.QueryParams.MinAmountSortAscending.HasValue)
        {
            if (orderedAmounts != null)
            {
                return request.QueryParams.MinAmountSortAscending.Value
                    ? orderedAmounts.ThenBy(minAmount => minAmount.MinAmount)
                    : orderedAmounts.ThenByDescending(minAmount => minAmount.MinAmount);
            }

            return request.QueryParams.MinAmountSortAscending.Value
                ? amountsToSort.OrderBy(minAmount => minAmount.MinAmount)
                : amountsToSort.OrderByDescending(minAmount => minAmount.MinAmount);
        }

        return orderedAmounts;
    }

    internal sealed record Query(ODataQueryOptions<OdpItemEntityDto> QueryOptions, ExtendedQueryParams QueryParams)
        : IQuery<PageableListOfItems<OdpItemEntityDto>>;

    internal sealed class ExtendedQueryParams
    {
        public int[]? StoragesIds { get; set; }

        public bool? StorageSortAscending { get; set; }

        public bool? MinAmountSortAscending { get; set; }

    }

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
