namespace Modsen.Portal.Merch.Requests.Kits.Core.CoreDto
{
    internal class KitTotalWithPossibleInStorageCoreDto
    {
        public IReadOnlyCollection<KitSizeInStoragePossibleAmountCoreDto> PossibleKits { get; set; } = [];
        public int TotalPossibleKits { get; set; }
    }
}
