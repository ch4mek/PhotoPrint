using Modsen.Portal.Core.Merch;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

namespace Modsen.Portal.Merch.Requests.Kits.Core.Calculators
{
    internal interface IKitPossibleAmountCalculator
    {
        KitTotalWithPossibleInStorageCoreDto Calculate(
            KitWithCompoundsCoreDto kit,
            List<KitMinAmountEntity> kitMinAmounts,
            List<CompoundSizeAmountInStockCoreDto> kitCompoundInStockAmounts);
    }

    internal class KitPossibleAmountCalculator : IKitPossibleAmountCalculator
    {
        private readonly INonClothingItemSizesPossibleAmountsCalculator _possibleNonClothingAmountsCalculator;
        private readonly ITotalPossibleKitsWithClothingCalculator _totalPossibleKitsWithClothingCalculator;
        private readonly IClothingItemSizesPossibleAmountsCalculator _possibleClothingAmountsCalculator;

        public KitPossibleAmountCalculator(
            INonClothingItemSizesPossibleAmountsCalculator possibleNonClothingAmountsCalculator,
            ITotalPossibleKitsWithClothingCalculator totalPossibleKitsWithClothingCalculator,
            IClothingItemSizesPossibleAmountsCalculator possibleClothingAmountsCalculator)
        {
            _possibleNonClothingAmountsCalculator = possibleNonClothingAmountsCalculator;
            _totalPossibleKitsWithClothingCalculator = totalPossibleKitsWithClothingCalculator;
            _possibleClothingAmountsCalculator = possibleClothingAmountsCalculator;
        }

        public KitTotalWithPossibleInStorageCoreDto Calculate(KitWithCompoundsCoreDto kit,
            List<KitMinAmountEntity> kitMinAmounts,
            List<CompoundSizeAmountInStockCoreDto> kitCompoundInStockAmounts)
        {
            if (!kit.ContainsClothing)
            {
                var possible = _possibleNonClothingAmountsCalculator.Calculate(kitMinAmounts,
                    kitCompoundInStockAmounts,
                    kit.Compounds);

                return new KitTotalWithPossibleInStorageCoreDto
                {
                    PossibleKits = possible,
                    TotalPossibleKits = possible.Sum(possibleAmount => possibleAmount.PossibleKitAmount),
                };
            }

            var amountsOfNonClothingCompoundsList = _possibleNonClothingAmountsCalculator.Calculate(kitMinAmounts,
                kitCompoundInStockAmounts,
                kit.Compounds);

            var amountsOfClothingCompoundSizesList = _possibleClothingAmountsCalculator.Calculate(kitMinAmounts,
                kitCompoundInStockAmounts,
                kit.Compounds);

            foreach (var clothingSizePossibleAmount in amountsOfClothingCompoundSizesList)
            {
                var isNonClothingSizeAmountLess = amountsOfNonClothingCompoundsList
                    .Where(amount => amount.StorageName == clothingSizePossibleAmount.StorageName)
                    .Any(amount => amount.PossibleKitAmount < clothingSizePossibleAmount.PossibleKitAmount);

                if (isNonClothingSizeAmountLess)
                {
                    clothingSizePossibleAmount.PossibleKitAmount = amountsOfNonClothingCompoundsList
                        .Where(amount => amount.StorageName == clothingSizePossibleAmount.StorageName)
                        .Min(amount => amount.PossibleKitAmount);

                    clothingSizePossibleAmount.Status = amountsOfNonClothingCompoundsList
                        .Where(amount => amount.StorageName == clothingSizePossibleAmount.StorageName)
                        .Any(amount => amount.Status == StorageAvailabilityStatus.RunningOut)
                        ? StorageAvailabilityStatus.RunningOut
                        : StorageAvailabilityStatus.Available;
                }
            }

            return new KitTotalWithPossibleInStorageCoreDto
            {
                PossibleKits = amountsOfClothingCompoundSizesList,
                TotalPossibleKits = _totalPossibleKitsWithClothingCalculator.Calculate(kitCompoundInStockAmounts,
                kit.Compounds),
            };
        }
    }
}
