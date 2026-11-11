namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;

internal record UpdateGroupInput(
    int ParentGroupId,
    IReadOnlyCollection<int> ChildGroups,
    string Name,
    string? Icon,
    bool IsActive,
    bool IsAvailableOnMainScreen,
    IReadOnlyCollection<int> Articles);
