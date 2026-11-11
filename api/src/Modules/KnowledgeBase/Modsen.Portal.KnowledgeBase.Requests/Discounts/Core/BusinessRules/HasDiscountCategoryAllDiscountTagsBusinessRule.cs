using Microsoft.EntityFrameworkCore;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.BusinessRules;

internal interface IHasDiscountCategoryAllDiscountTagsBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(IReadOnlyCollection<int> discountTagIds, int discountCategoryId, CancellationToken cancellationToken);
}

internal class HasDiscountCategoryAllDiscountTagsBusinessRule : IHasDiscountCategoryAllDiscountTagsBusinessRule
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public HasDiscountCategoryAllDiscountTagsBusinessRule(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<bool> IsCorrespondsToBusiness(IReadOnlyCollection<int> discountTagIds, int discountCategoryId, CancellationToken cancellationToken)
    {
        var discountCategoryTagIds = await _knowledgeBaseDbContext.DiscountCategoryTags
            .Where(x => x.DiscountCategoryId == discountCategoryId)
            .Select(x => x.TagId)
            .ToListAsync(cancellationToken);

        return discountTagIds.All(discountCategoryTagIds.Contains);
    }
}
