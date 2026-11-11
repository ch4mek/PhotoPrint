using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Requests;

internal sealed class GetDiscountCategoryByIdRequest : IQueryHandler<GetDiscountCategoryByIdRequest.Query, DiscountCategoryCoreDto>
{
    private readonly IDiscountCategoryLoader _discountCategoryLoader;

    public GetDiscountCategoryByIdRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IDiscountCategoryLoader discountCategoryLoader)
    {
        _discountCategoryLoader = discountCategoryLoader;
    }

    public async Task<DiscountCategoryCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        return await _discountCategoryLoader.Load(request.Id, cancellationToken);
    }

    internal sealed record Query(int Id) : IQuery<DiscountCategoryCoreDto>;
}
