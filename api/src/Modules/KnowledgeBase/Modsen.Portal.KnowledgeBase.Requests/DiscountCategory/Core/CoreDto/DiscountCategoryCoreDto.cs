namespace Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.CoreDto;

internal class DiscountCategoryCoreDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int OrderWeight { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public IReadOnlyCollection<DiscountCategoryTagCoreDto> DiscountCategoryTags { get; set; } = [];
}
