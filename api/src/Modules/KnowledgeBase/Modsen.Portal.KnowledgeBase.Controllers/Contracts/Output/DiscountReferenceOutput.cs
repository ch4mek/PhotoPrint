using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal class DiscountReferenceOutput
{
    public int Id { get; set; }
    public int DiscountId { get; set; }

    public string? SocialMediaUrl { get; set; }

    public static DiscountReferenceOutput Of(DiscountReferenceCoreDto discountReferenceCoreDto)
    {
        return new DiscountReferenceOutput
        {
            Id = discountReferenceCoreDto.Id,
            DiscountId = discountReferenceCoreDto.Id,
            SocialMediaUrl = discountReferenceCoreDto.SocialMediaUrl
        };
    }
}
