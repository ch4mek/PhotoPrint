using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;

internal sealed class GetDiscountsByDiscountCategoryIdRequest : IQueryHandler<GetDiscountsByDiscountCategoryIdRequest.Query, List<DiscountCoreDto>>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IDiscountLoader _discountsLoader;

    public GetDiscountsByDiscountCategoryIdRequest(
        IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IDiscountLoader discountLoader)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _discountsLoader = discountLoader;
    }

    public async Task<List<DiscountCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var discountIds = await _knowledgeBaseDbContext.Discounts
             .Where(x => x.DiscountCategoryId == request.DiscountCategoryId)
             .Select(x => x.Id)
             .ToArrayAsync(cancellationToken);

        return await _discountsLoader.Load(discountIds, cancellationToken);
    }

    internal sealed record Query(int DiscountCategoryId) : IQuery<List<DiscountCoreDto>>;
}
