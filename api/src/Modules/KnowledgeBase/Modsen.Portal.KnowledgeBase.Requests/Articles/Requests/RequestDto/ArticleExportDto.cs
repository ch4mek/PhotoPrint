namespace Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;

internal class ArticleExportDto
{
    public string? ArticleGroupName { get; set; }
    public string? Name { get; set; }
    public string? Text { get; set; }
    public string? Keywords { get; set; }

    public string? ArticleTagNames { get; set; }
    public string? InternalFileIds { get; set; }
    public string? ExternalUrls { get; set; }
}
