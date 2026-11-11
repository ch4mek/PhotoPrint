namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;

internal record DiscountCategoryInput
{
    public required string Name { get; set; }
    public IReadOnlyCollection<string>? DiscountCategoryTags { get; set; } = [];
}
