using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal class DiscountCategoryTagOutput
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int DiscountCategoryId { get; set; }
    public int TagId { get; set; }

    public static DiscountCategoryTagOutput Of(DiscountCategoryTagCoreDto discountCategoryTagDto)
    {
        return new DiscountCategoryTagOutput
        {
            Id = discountCategoryTagDto.Id,
            DiscountCategoryId = discountCategoryTagDto.DiscountCategoryId,
            TagId = discountCategoryTagDto.TagId,
            Name = discountCategoryTagDto.Name
        };
    }
}
