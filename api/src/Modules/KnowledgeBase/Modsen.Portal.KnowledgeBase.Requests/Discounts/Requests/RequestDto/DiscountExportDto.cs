namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests.RequestDto;

internal class DiscountExportDto
{
    public required string DiscountCategoryName { get; set; }

    public required string Name { get; set; }
    public int DiscountPercent { get; set; }
    public required string Text { get; set; }
    public string? Keywords { get; set; }

    public string? DiscountTagNames { get; set; }
    public string? DiscountCityNames { get; set; }
    public string? InternalFileIds { get; set; }
    public string? ExternalUrls { get; set; }
}
