using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;

internal sealed class GroupOrderInput
{
    public IReadOnlyCollection<GroupOrderDto> GroupOrders { get; set; } = [];
}
