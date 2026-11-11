using Microsoft.EntityFrameworkCore;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

namespace Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.BusinessRules;

internal interface IHasTagAnyDiscountsInCategory
{
    Task<bool> IsCorrespondsToBusiness(List<string> namesToDelete, int discountCategoryId, CancellationToken cancellationToken);
}

internal class HasTagAnyDiscountsInCategory : IHasTagAnyDiscountsInCategory
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public HasTagAnyDiscountsInCategory(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(List<string> namesToDelete, int discountCategoryId, CancellationToken cancellationToken)
    {
        return await _knowledgeBaseDbContext.DiscountTags
            .Where(x => x.Discount!.DiscountCategoryId == discountCategoryId)
            .Where(x => namesToDelete.Contains(x.Tag!.Name))
            .AnyAsync(cancellationToken);
    }
}
