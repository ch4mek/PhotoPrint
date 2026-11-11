namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;

internal record CreateArticleInput(
    int GroupId,
    string Name,
    string? Icon,
    string Text,
    string Keywords,
    bool IsActive,
    bool IsAvailableOnMainScreen,
    List<string>? Tags);
