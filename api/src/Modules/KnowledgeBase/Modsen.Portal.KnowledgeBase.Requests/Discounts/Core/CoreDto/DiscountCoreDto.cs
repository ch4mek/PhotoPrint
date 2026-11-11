namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

internal class DiscountCoreDto
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

    public IReadOnlyCollection<DiscountReferenceCoreDto> DiscountReferences { get; set; } = [];
    public IReadOnlyCollection<DiscountTagCoreDto> DiscountTags { get; set; } = [];
    public IReadOnlyCollection<DiscountCityCoreDto> DiscountCities { get; set; } = [];
}
