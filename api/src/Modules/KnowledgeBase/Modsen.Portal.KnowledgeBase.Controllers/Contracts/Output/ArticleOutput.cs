using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal record ArticleOutput
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
    public KnowledgeBaseUserOutput? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public KnowledgeBaseUserOutput? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public IReadOnlyCollection<ArticleReferenceOutput> ArticleReferences { get; set; } = [];
    public IReadOnlyCollection<string> Tags { get; set; } = [];

    public static ArticleOutput Of(ArticleDto articleDto)
    {
        return new ArticleOutput
        {
            Id = articleDto.Id,
            GroupId = articleDto.GroupId,
            Name = articleDto.Name,
            Icon = articleDto.Icon,
            Views = articleDto.Views,
            Text = articleDto.Text,
            Keywords = articleDto.Keywords,
            IsActive = articleDto.IsActive,
            IsAvailableOnMainScreen = articleDto.IsAvailableOnMainScreen,
            OrderWeight = articleDto.OrderWeight,
            CreatedByUser = KnowledgeBaseUserOutput.Of(articleDto.CreatedByUser),
            CreatedAtUtc = articleDto.CreatedAtUtc,
            UpdatedByUser = KnowledgeBaseUserOutput.Of(articleDto.UpdatedByUser),
            UpdatedAtUtc = articleDto.UpdatedAtUtc,
            Tags = articleDto.ArticleTags
               .Select(dto => dto.Name).ToList(),
            ArticleReferences = articleDto.ArticleReferences.Select(ArticleReferenceOutput.Of).ToList()
        };
    }
}
