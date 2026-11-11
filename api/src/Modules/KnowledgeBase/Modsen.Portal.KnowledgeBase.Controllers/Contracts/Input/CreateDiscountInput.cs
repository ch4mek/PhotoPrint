namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;

internal class CreateDiscountInput
{
    public int DiscountCategoryId { get; set; }

    public required string Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageFileId { get; set; }
    public string? CompanyType { get; set; }
    public string? Website { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Location { get; set; }
    public int DiscountPercent { get; set; }
    public int? DiscountPercentTo { get; set; }
    public required string Text { get; set; }
    public bool IsActive { get; set; }

    public IReadOnlyCollection<int>? DiscountTagIds { get; set; }
    public required List<int> DiscountCityIds { get; set; }
    public IReadOnlyCollection<string>? DiscountSocialMediaUrls { get; set; }
}
