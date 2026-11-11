using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;

internal class DiscountCategoryOrdersInput
{
    public IReadOnlyCollection<DiscountCategoryOrderDto> DiscountCategoryOrders { get; set; } = [];
}
