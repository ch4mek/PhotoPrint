using Microsoft.EntityFrameworkCore;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

namespace Modsen.Portal.KnowledgeBase.Requests.Core.Calculators;

internal interface IMaxOrderWeightCalculator
{
    Task<int> CalculateAsync(int? groupId, CancellationToken cancellationToken);
}

internal sealed class MaxOrderWeightCalculator(
    IKnowledgeBaseDbContext knowledgeBaseDbContext) : IMaxOrderWeightCalculator
{
    public async Task<int> CalculateAsync(int? groupId, CancellationToken cancellationToken)
    {
        var maxArticleOrderWeight = (await knowledgeBaseDbContext.Articles
            .Where(article => article.GroupId == groupId)
            .Select(article => article.OrderWeight)
            .ToListAsync(cancellationToken)).DefaultIfEmpty(0).Max();

        var maxGroupOrderWeight = (await knowledgeBaseDbContext.Groups
            .Where(group => group.ParentGroupId == groupId)
            .Select(article => article.OrderWeight)
            .ToListAsync(cancellationToken)).DefaultIfEmpty(0).Max();

        var maxOrderWeight = maxArticleOrderWeight > maxGroupOrderWeight ? maxArticleOrderWeight : maxGroupOrderWeight;

        return maxOrderWeight + 1;
    }
}
