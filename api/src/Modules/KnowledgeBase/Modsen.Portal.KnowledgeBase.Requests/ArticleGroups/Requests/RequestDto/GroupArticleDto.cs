namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests.RequestDto;

internal record GroupArticleDto(
    int Id,
    string Name,
    string? Icon,
    bool IsActive,
    bool IsAvailableOnMainScreen,
    int OrderWeight,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
