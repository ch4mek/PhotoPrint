using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Requests;

internal sealed class GetAllDiscountCategoriesRequest : IQueryHandler<GetAllDiscountCategoriesRequest.Query, List<DiscountCategoryCoreDto>>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IDiscountCategoryLoader _discountCategoryLoader;

    public GetAllDiscountCategoriesRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IDiscountCategoryLoader discountCategoryLoader)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _discountCategoryLoader = discountCategoryLoader;
    }

    public async Task<List<DiscountCategoryCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var discountCategoryIds =  await _knowledgeBaseDbContext.DiscountCategories
           .Select(x => x.Id)
           .ToArrayAsync(cancellationToken);

        return await _discountCategoryLoader.Load(discountCategoryIds, cancellationToken);
    }

    internal sealed record Query : IQuery<List<DiscountCategoryCoreDto>>;
}
