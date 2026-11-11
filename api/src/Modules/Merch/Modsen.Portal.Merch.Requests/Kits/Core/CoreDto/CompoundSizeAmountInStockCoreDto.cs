namespace Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;

internal class CompoundSizeAmountInStockCoreDto
{
    public int ItemId { get; set; }

    public required string SizeId { get; set; }
    public int SizeOrderWeight { get; set; }

    public int StorageId { get; set; }
    public required string StorageName { get; set; }

    public int Amount { get; set; }
}
