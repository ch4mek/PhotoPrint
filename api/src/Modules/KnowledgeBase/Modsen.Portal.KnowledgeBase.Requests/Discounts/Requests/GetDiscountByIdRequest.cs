using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;

internal sealed class GetDiscountByIdRequest : IQueryHandler<GetDiscountByIdRequest.Query, DiscountCoreDto>
{
    private readonly IDiscountLoader _discountLoader;

    public GetDiscountByIdRequest(IDiscountLoader discountLoader)
    {
        _discountLoader = discountLoader;
    }

    public async Task<DiscountCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var discount = await _discountLoader.Load(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"Discount by id {request.Id} is not found");

        return discount;
    }

    internal sealed record Query(int Id) : IQuery<DiscountCoreDto>;
}
