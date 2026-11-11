namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;

internal record CreateGroupInput(
    int? ParentGroupId,
    string Name,
    string? Icon,
    bool IsActive,
    bool IsAvailableOnMainScreen);
