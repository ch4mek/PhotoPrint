namespace Modsen.Portal.Merch.Requests.Core.Dto
{
    internal class ItemSizeCountDto
    {
        public int CountInKit { get; set; }
        public int ItemId { get; set; }
        public IReadOnlyCollection<SizeCountDto> SizeCounts { get; set; } = [];
    }
}
