using Modsen.Portal.KnowledgeBase.Requests.Dto;

namespace Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;

internal record ArticleDto
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public int Views { get; set; }
    public required string Text { get; set; }
    public string? Keywords { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailableOnMainScreen { get; set; }
    public int OrderWeight { get; set; }
    public KnowledgeBaseUserDto? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public KnowledgeBaseUserDto? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public IReadOnlyCollection<ArticleReferenceDto> ArticleReferences { get; set; } = [];
    public IReadOnlyCollection<TagDto> ArticleTags { get; set; } = [];
}
