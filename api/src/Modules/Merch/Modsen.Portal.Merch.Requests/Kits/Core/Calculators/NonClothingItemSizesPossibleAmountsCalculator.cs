using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Kits.Core.Calculators;

internal interface INonClothingItemSizesPossibleAmountsCalculator
{
    IReadOnlyCollection<KitSizeInStoragePossibleAmountCoreDto> Calculate(
        IReadOnlyCollection<KitMinAmountEntity> kitMinAmounts,
        IReadOnlyCollection<CompoundSizeAmountInStockCoreDto> amountsInStock,
        IReadOnlyCollection<KitCompoundCoreDto> compounds);
}

internal sealed class NonClothingItemSizesPossibleAmountsCalculator : INonClothingItemSizesPossibleAmountsCalculator
{
    public IReadOnlyCollection<KitSizeInStoragePossibleAmountCoreDto> Calculate(
        IReadOnlyCollection<KitMinAmountEntity> kitMinAmounts,
        IReadOnlyCollection<CompoundSizeAmountInStockCoreDto> amountsInStock,
        IReadOnlyCollection<KitCompoundCoreDto> compounds)
    {
        return amountsInStock
            .Join(compounds,
                amount => amount.ItemId,
                compound => compound.Id,
                (amount, compound) => new {amount, compound})

            .Where(amounts => !amounts.compound.IsClothing)
            .OrderBy(amounts => amounts.amount.StorageId)
                .ThenBy(amounts => amounts.amount.SizeOrderWeight)

            .GroupBy(amounts => new { amounts.amount.StorageId, amounts.amount.StorageName })
            .Select(grouping => new KitSizeInStoragePossibleAmountCoreDto
            {
                SizeId = grouping.Select(sizeAmount => sizeAmount.amount.SizeId).Distinct().First(),
                PossibleKitAmount = grouping
                    .Min(sizeAmount => sizeAmount.amount.Amount / sizeAmount.compound.AmountInKit),
                StorageName = grouping.Key.StorageName,
                Status = kitMinAmounts
                    .Where(minAmount => minAmount.WarehouseId == grouping.Key.StorageId)
                    .Select(minAmounts => minAmounts.MinAmount)
                    .FirstOrDefault() > grouping.Min(sizeAmount =>
                        sizeAmount.amount.Amount / sizeAmount.compound.AmountInKit)
                    ? StorageAvailabilityStatus.RunningOut
                    : StorageAvailabilityStatus.Available
            })
            .ToList();
    }
}
