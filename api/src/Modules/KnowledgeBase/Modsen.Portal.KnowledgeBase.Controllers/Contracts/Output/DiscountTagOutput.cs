using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal class DiscountTagOutput
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int DiscountId { get; set; }
    public int TagId { get; set; }

    public static DiscountTagOutput Of(DiscountTagCoreDto discountTagCoreDto)
    {
        return new DiscountTagOutput
        {
            Id = discountTagCoreDto.Id,
            DiscountId = discountTagCoreDto.DiscountId,
            TagId = discountTagCoreDto.TagId,
            Name = discountTagCoreDto.Name
        };
    }
}
