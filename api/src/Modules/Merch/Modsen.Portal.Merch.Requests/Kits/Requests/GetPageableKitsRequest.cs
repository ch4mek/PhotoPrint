using System.Globalization;
using System.Linq.Expressions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.Calculators;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;
using Modsen.Portal.Merch.Requests.Kits.Core.Loaders;
using Modsen.Portal.Merch.Requests.Kits.Core.Resolvers;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;
using Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;

namespace Modsen.Portal.Merch.Requests.Kits.Requests;

internal sealed class GetPageableKitsRequest
    : IQueryHandler<GetPageableKitsRequest.Query, PageableListOfItems<KitStorageDto>>
{
    private readonly IKitPossibleAmountCalculator _kitPossibleAmountCalculator;
    private readonly ICompoundSizesInStockResolver _compoundSizesInStockResolver;
    private readonly IKitsWithCompoundsLoader _kitsLoader;
    private readonly IMerchDbContext _dbContext;

    public GetPageableKitsRequest(
        ICompoundSizesInStockResolver compoundSizesInStockResolver,
        IKitsWithCompoundsLoader kitsLoader,
        IMerchDbContext dbContext,
        IKitPossibleAmountCalculator kitPossibleAmountCalculator)
    {
        _compoundSizesInStockResolver = compoundSizesInStockResolver;
        _kitsLoader = kitsLoader;
        _dbContext = dbContext;
        _kitPossibleAmountCalculator = kitPossibleAmountCalculator;
    }

    public async Task<PageableListOfItems<KitStorageDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.RequestDto.QueryOptions;

        var kitsIdsToLoad = await GetKitsIdsToLoadAsync(queryOptions, cancellationToken);
        var kitsCoreDtos = await _kitsLoader.LoadAsync(kitsIdsToLoad, queryOptions.StorageIds, cancellationToken);

        if (kitsCoreDtos.Count == 0)
        {
            return EmptyPageableListOfItems<KitStorageDto>.Value;
        }

        var kitsMinAmounts = await _dbContext.KitMinAmounts
            .Where(amount => queryOptions.StorageIds == null || queryOptions.StorageIds.Contains(amount.WarehouseId))
            .Where(amount => kitsIdsToLoad.Contains(amount.KitId))
            .ToListAsync(cancellationToken);

        foreach (var kit in kitsCoreDtos)
        {
            await FillKit(kitsMinAmounts, kit, queryOptions, cancellationToken);
        }

        kitsCoreDtos = ApplyFilteringAndOrderingAfterProcessing(kitsCoreDtos, queryOptions);
        if (kitsCoreDtos.Count == 0)
        {
            return EmptyPageableListOfItems<KitStorageDto>.Value;
        }

        var totalCount = kitsCoreDtos.Count;

        var kits = kitsCoreDtos
            .Skip(request.RequestDto.Skip)
            .Take(request.RequestDto.Top)
            .Select(KitStorageDto.Of)
            .ToList();

        foreach (var kitStorageDto in kits)
        {
            kitStorageDto.AllStorages = kitStorageDto.AllStorages?
                .OrderBy(x => x)
                .ToArray();
        }

        return kits.ToPageable()
            .WithPaging(
                page: (request.RequestDto.Skip / request.RequestDto.Top) + 1,
                pageSize: request.RequestDto.Top)
            .WithTotal(totalCount);
    }

    private async Task FillKit(List<KitMinAmountEntity> kitsMinAmounts, KitWithCompoundsCoreDto kit,
        KitsQueryOptionsDto queryOptions, CancellationToken cancellationToken)
    {
        var kitMinAmounts = kitsMinAmounts.Where(amount => amount.KitId == kit.Id).ToList();
        var compoundIds = kit.Compounds.Select(compound => compound.Id).ToArray();
        var kitCompoundInStockAmounts = await _compoundSizesInStockResolver
            .ResolveAsync(compoundIds, queryOptions.StorageIds, cancellationToken);
        var calculated = _kitPossibleAmountCalculator.Calculate(kit, kitMinAmounts, kitCompoundInStockAmounts);

        kit.PossibleKits = calculated.PossibleKits;
        kit.TotalPossibleKits = calculated.TotalPossibleKits;

        kit.Status = kit.Status == StorageAvailabilityStatus.Archived ? StorageAvailabilityStatus.Archived :
            kit.PossibleKits
                .Any(possibleAmountDto => possibleAmountDto.Status == StorageAvailabilityStatus.RunningOut)
                ? StorageAvailabilityStatus.RunningOut
                : StorageAvailabilityStatus.Available;

        kit.StorageAvailabilityStatuses = kit.PossibleKits
            .GroupBy(possibleKit => possibleKit.StorageName)
            .Select(grouping => new StorageAvailabilityStatusCoreDto
            {
                StorageName = grouping.Key,
                Status = ResolveStatus(grouping, kit.Status)
            })
            .ToList();
    }

    private static StorageAvailabilityStatus ResolveStatus(IGrouping<string, KitSizeInStoragePossibleAmountCoreDto> kits,
        StorageAvailabilityStatus kitStatus)
    {
        if (kitStatus == StorageAvailabilityStatus.Archived)
        {
            return StorageAvailabilityStatus.Archived;
        }

        if (kits.Any(possibleKit => possibleKit.Status == StorageAvailabilityStatus.RunningOut))
        {
            return StorageAvailabilityStatus.RunningOut;
        }

        return StorageAvailabilityStatus.Available;
    }

    private Task<int[]> GetKitsIdsToLoadAsync(KitsQueryOptionsDto queryOptions, CancellationToken cancellationToken)
    {
        var query = ApplyFilters(_dbContext.Kits.AsQueryable(), queryOptions.Filters);

        Expression<Func<KitEntity, object>> keySelector = queryOptions.SortColumn switch
        {
            "id" => kit => kit.Id,
            "name" => kit => kit.Name,
            _ => kit => kit.Id
        };

        query = queryOptions.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "desc"
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);

        return query.Select(kit => kit.Id).ToArrayAsync(cancellationToken);
    }

    private static IQueryable<KitEntity> ApplyFilters(IQueryable<KitEntity> query, IReadOnlyCollection<QueryFilterDto>? filters)
    {
        if (filters is null || filters.Count == 0)
        {
            return query;
        }

        foreach (var filterDto in filters)
        {
            if (filterDto.FilterColumn is null)
            {
                continue;
            }

            query = query.Where(BuildFilter(filterDto));
        }

        return query;
    }

    private static Expression<Func<KitEntity, bool>> BuildFilter(QueryFilterDto filterDto)
    {
        if (filterDto.FilterColumn == "id")
        {
            return int.TryParse(filterDto.FilterValue, out var id)
                ? kitEntity => kitEntity.Id == id
                : kitEntity => kitEntity.Id == -1;
        }

        return filterDto.FilterColumn switch
        {
            "name" => kitEntity => kitEntity.Name.ToLower()
                .Contains(filterDto.FilterValue!.ToLower()),
            "compound" => kitEntity => kitEntity.KitItems.Select(item => item.Item!.Name.ToLower())
                .Any(name => name.Contains(filterDto.FilterValue!.ToLower())),
            "description" => kitEntity => kitEntity.Description!.ToLower()
                .Contains(filterDto.FilterValue!.ToLower()),
            _ => kitEntity => kitEntity.Id != -1
        };
    }

    private static List<KitWithCompoundsCoreDto> ApplyFilteringAndOrderingAfterProcessing(
        List<KitWithCompoundsCoreDto> kitsCoreDtos,
        KitsQueryOptionsDto queryOptions)
    {
        if (queryOptions.SortColumn == "status" || queryOptions.SortColumn == "possibleKits")
        {
            Func<KitWithCompoundsCoreDto, object> resultOrderingSelector = queryOptions.SortColumn switch
            {
                "status" => kit => kit.Status,
                "possibleKits" => kit => kit.TotalPossibleKits,
                _ => kit => kit.Id
            };

            kitsCoreDtos = queryOptions.SortOrder?.ToLower(CultureInfo.InvariantCulture) == "desc"
                ? kitsCoreDtos.OrderByDescending(resultOrderingSelector).ToList()
                : kitsCoreDtos.OrderBy(resultOrderingSelector).ToList();
        }

        kitsCoreDtos = kitsCoreDtos
            .Where(kit => queryOptions.AmountMax == null || kit.TotalPossibleKits <= queryOptions.AmountMax)
            .Where(kit => queryOptions.AmountMin == null || kit.TotalPossibleKits >= queryOptions.AmountMin)
            .Where(kit => queryOptions.StatusIds == null || queryOptions.StatusIds.Contains(kit.Status)).ToList();

        return kitsCoreDtos;
    }

    internal sealed record Query(GetPageableKitsDto RequestDto) : IQuery<PageableListOfItems<KitStorageDto>>;

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

            RuleFor(query => query.RequestDto.QueryOptions).SetValidator(new KitsQueryOptionsDto.Validator());
        }
    }
}
