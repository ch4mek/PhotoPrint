using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal class CityOutput
{
    public int CityId { get; set; }
    public required string Name { get; set; }

    public static CityOutput Of(DiscountCityCoreDto discountCityCoreDto)
    {
        return new CityOutput
        {
            CityId = discountCityCoreDto.CityId,
            Name = discountCityCoreDto.Name
        };
    }
}
