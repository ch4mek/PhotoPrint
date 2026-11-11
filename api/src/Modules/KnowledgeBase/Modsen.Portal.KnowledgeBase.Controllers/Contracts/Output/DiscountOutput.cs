using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal class DiscountOutput
{
    public int Id { get; set; }

    public int DiscountCategoryId { get; set; }
    public required string DiscountCategoryName { get; set; }

    public required string Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageFileId { get; set; }
    public int DiscountPercent { get; set; }
    public int? DiscountPercentTo { get; set; }
    public string? CompanyType { get; set; }
    public string? Website { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Location { get; set; }
    public required string Text { get; set; }
    public bool IsActive { get; set; }
    public int OrderWeight { get; set; }

    public IReadOnlyCollection<DiscountReferenceOutput> DiscountReferences { get; set; } = [];
    public IReadOnlyCollection<DiscountTagOutput> DiscountTags { get; set; } = [];
    public IReadOnlyCollection<CityOutput> DiscountCities { get; set; } = [];

    public static DiscountOutput Of(DiscountCoreDto discountCoreDto)
    {
        return new DiscountOutput
        {
            Id = discountCoreDto.Id,
            DiscountCategoryId = discountCoreDto.DiscountCategoryId,
            DiscountCategoryName = discountCoreDto.DiscountCategoryName,
            Name = discountCoreDto.Name,
            ImageUrl = discountCoreDto.ImageUrl,
            ImageFileId = discountCoreDto.ImageFileId,
            DiscountPercent = discountCoreDto.DiscountPercent,
            DiscountPercentTo = discountCoreDto.DiscountPercentTo,
            CompanyType = discountCoreDto.CompanyType,
            Website = discountCoreDto.Website,
            PhoneNumber = discountCoreDto.PhoneNumber,
            Location = discountCoreDto.Location,
            Text = discountCoreDto.Text,
            IsActive = discountCoreDto.IsActive,
            OrderWeight = discountCoreDto.OrderWeight,
            DiscountReferences = (discountCoreDto.DiscountReferences).Select(DiscountReferenceOutput.Of).ToList(),
            DiscountTags = (discountCoreDto.DiscountTags).Select(DiscountTagOutput.Of).ToList(),
            DiscountCities = (discountCoreDto.DiscountCities).Select(CityOutput.Of).ToList()
        };
    }

}
