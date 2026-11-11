namespace Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;

internal record ArticleReferenceDto
{
    public int Id { get; set; }
    public string? InternalFileId { get; set; }
    public string? ExternalUrl { get; set; }
}
