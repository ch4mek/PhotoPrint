using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;

internal class DiscountOrdersInput
{
    public IReadOnlyCollection<DiscountOrderDto> DiscountOrders { get; set; } = [];
}
