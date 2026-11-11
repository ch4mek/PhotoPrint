namespace Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.CoreDto;

internal class DiscountCategoryTagCoreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int DiscountCategoryId { get; set; }
    public int TagId { get; set; }
}
