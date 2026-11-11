using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Core.Dto;
using Modsen.Portal.Merch.Requests.Core.Loaders;
using Modsen.Portal.Merch.Requests.Kits.Core.Calculators;
using Modsen.Portal.Merch.Requests.Kits.Core.Loaders;
using Modsen.Portal.Merch.Requests.Kits.Core.Resolvers;

namespace Modsen.Portal.Merch.Requests.Kits.Requests;

internal sealed class GetKitCountsByIdRequest : IQueryHandler<GetKitCountsByIdRequest.Query, KitCountDto>
{
    private readonly IItemsCountLoader _itemsCountLoader;
    private readonly IKitPossibleAmountCalculator _kitPossibleAmountCalculator;
    private readonly IKitsWithCompoundsLoader _kitsLoader;
    private readonly ICompoundSizesInStockResolver _compoundSizesInStockResolver;
    public GetKitCountsByIdRequest(
        IItemsCountLoader itemsCountLoader,
        IKitPossibleAmountCalculator kitPossibleAmountCalculator,
        IKitsWithCompoundsLoader kitsLoader,
        ICompoundSizesInStockResolver compoundSizesInStockResolver)
    {
        _itemsCountLoader = itemsCountLoader;
        _kitPossibleAmountCalculator = kitPossibleAmountCalculator;
        _kitsLoader = kitsLoader;
        _compoundSizesInStockResolver = compoundSizesInStockResolver;
    }

    public async Task<KitCountDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var kits = await _kitsLoader.LoadAsync([request.KitId], [request.WarehouseId], cancellationToken);
        var kit = kits.FirstOrDefault() ?? throw new EntityNotFoundException("Kit not found");
        var compoundIds = kit.Compounds.Select(compound => compound.Id).ToArray();

        var kitCompoundInStockAmounts = await _compoundSizesInStockResolver
                .ResolveAsync(compoundIds, [request.WarehouseId], cancellationToken);

        var total = _kitPossibleAmountCalculator.Calculate(kit,
            Array.Empty<KitMinAmountEntity>().ToList(),
            kitCompoundInStockAmounts);
        var sizes = await _itemsCountLoader.LoadAsync(
            compoundIds,
            request.WarehouseId,
            cancellationToken);

        return new KitCountDto
        {
            AvailableAmountInStorage = total.TotalPossibleKits,
            ItemSizes = sizes.GroupJoin(
                kit.Compounds,
                size => size.ItemId,
                compound => compound.Id,
                (size, compoundGroup) => new { size, compound = compoundGroup.FirstOrDefault() }
            )
            .Select(x => new ItemSizeCountDto
            {
                ItemId = x.size.ItemId,
                CountInKit = x.compound?.AmountInKit ?? 0,
                SizeCounts = x.size.SizeCounts
            })
            .ToList(),
        };
    }

    internal sealed record Query(int KitId, int WarehouseId) : IQuery<KitCountDto>;
}
