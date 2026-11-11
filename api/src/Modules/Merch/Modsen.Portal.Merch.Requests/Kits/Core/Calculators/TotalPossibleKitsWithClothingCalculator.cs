using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Kits.Core.Calculators;

internal interface ITotalPossibleKitsWithClothingCalculator
{
    int Calculate(IReadOnlyCollection<CompoundSizeAmountInStockCoreDto> kitCompoundInStockAmounts,
        IReadOnlyCollection<KitCompoundCoreDto> compounds);
}

internal sealed class TotalPossibleKitsWithClothingCalculator : ITotalPossibleKitsWithClothingCalculator
{
    public int Calculate(IReadOnlyCollection<CompoundSizeAmountInStockCoreDto> kitCompoundInStockAmounts,
        IReadOnlyCollection<KitCompoundCoreDto> compounds)
    {
        return kitCompoundInStockAmounts
            .Join(compounds,
                amount => amount.ItemId,
                compound => compound.Id,
                (amount, compound) => new { amount, compound })
            .GroupBy(res => new
            {
                res.amount.StorageId,
                res.amount.StorageName,
                res.amount.ItemId,
                res.compound.AmountInKit
            })
            .Select(grouping => new
            {
                grouping.Key.StorageName,
                ItemTotalStock = grouping.Sum(amount => amount.amount.Amount),
                grouping.Key.AmountInKit
            })
            .GroupBy(compound => compound.StorageName)
            .Select(group => group.Min(sizeAmount => sizeAmount.ItemTotalStock / sizeAmount.AmountInKit))
            .Sum();
    }
}
