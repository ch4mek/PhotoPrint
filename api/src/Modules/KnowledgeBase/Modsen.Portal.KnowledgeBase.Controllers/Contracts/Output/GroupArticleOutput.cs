using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Output;

internal record GroupArticleOutput
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Icon { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailableOnMainScreen { get; set; }
    public int OrderWeight { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public static GroupArticleOutput Of(GroupArticleDto groupArticleDto)
    {
        return new GroupArticleOutput
        {
            Id = groupArticleDto.Id,
            Name = groupArticleDto.Name,
            Icon = groupArticleDto.Icon,
            IsActive = groupArticleDto.IsActive,
            IsAvailableOnMainScreen = groupArticleDto.IsAvailableOnMainScreen,
            OrderWeight = groupArticleDto.OrderWeight,
            CreatedAtUtc = groupArticleDto.CreatedAtUtc,
            UpdatedAtUtc = groupArticleDto.UpdatedAtUtc
        };
    }
}
