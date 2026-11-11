using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal class DiscountCategoryOutput
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int OrderWeight { get; set; }
    public IReadOnlyCollection<DiscountCategoryTagOutput> DiscountCategoryTags { get; set; } = [];

    public static DiscountCategoryOutput Of(DiscountCategoryCoreDto discountCategoryDto)
    {
        return new DiscountCategoryOutput
        {
            Id = discountCategoryDto.Id,
            Name = discountCategoryDto.Name,
            OrderWeight = discountCategoryDto.OrderWeight,
            DiscountCategoryTags = discountCategoryDto.DiscountCategoryTags.Select(DiscountCategoryTagOutput.Of).ToList(),
        };
    }
}
