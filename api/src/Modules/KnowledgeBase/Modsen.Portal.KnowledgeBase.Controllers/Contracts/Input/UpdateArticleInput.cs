namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;

internal record UpdateArticleInput(
    int GroupId,
    string Name,
    string? Icon,
    string Text,
    string Keywords,
    bool IsActive,
    bool IsAvailableOnMainScreen,
    List<string> ArticleTags);
