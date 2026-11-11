using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal class DiscountCategoryShortOutput
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int OrderWeight { get; set; }

    public static DiscountCategoryShortOutput Of(DiscountCategoryCoreDto discountCategoryDto)
    {
        return new DiscountCategoryShortOutput
        {
            Id = discountCategoryDto.Id,
            Name = discountCategoryDto.Name,
            OrderWeight = discountCategoryDto.OrderWeight
        };
    }
}
