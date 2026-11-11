namespace Modsen.Portal.Merch.Requests.Core.Dto
{
    internal class KitCountDto
    {
        public int AvailableAmountInStorage { get; set; }
        public IReadOnlyCollection<ItemSizeCountDto> ItemSizes { get; set; } = [];
    }
}
