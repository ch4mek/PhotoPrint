namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

internal class DiscountTagCoreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int DiscountId { get; set; }
    public int TagId { get; set; }
}
