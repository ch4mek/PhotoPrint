using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

namespace Modsen.Portal.KnowledgeBase.Requests.Articles.Core;

internal interface IIsArticleWithSameNameExistBusinessRule
{
    Task<bool> IsCorrespondToRuleAsync(int subgroupId, string name, CancellationToken cancellationToken, int? articleId = null);
}

internal sealed class IsArticleWithSameNameExistBusinessRule : IIsArticleWithSameNameExistBusinessRule
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public IsArticleWithSameNameExistBusinessRule(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<bool> IsCorrespondToRuleAsync(int subgroupId, string name, CancellationToken cancellationToken, int? articleId = null)
    {
        var groupId = await GetGroupIdAsync(subgroupId, cancellationToken);

        return await IsArticleWithSameNameExist(groupId, name, articleId, cancellationToken);
    }

    private async Task<bool> IsArticleWithSameNameExist(int groupId, string name, int? articleId, CancellationToken cancellationToken)
    {
        var group = await _knowledgeBaseDbContext.Groups
            .Where(group => group.Id == groupId)
            .Select(group => new
            {
                IsContain = group.Articles.Any(article => article.Name == name && article.Id != articleId),
                ChildGroupIds = group.ChildGroups.Select(childGroup => childGroup.Id),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (group is null)
        {
            throw new EntityNotFoundException("Sub group is not exist");
        }

        if (group.IsContain)
        {
            return group.IsContain;
        }

        foreach (var id in group.ChildGroupIds)
        {
            var isChildGroupContain = await IsArticleWithSameNameExist(id, name, articleId, cancellationToken);

            if (isChildGroupContain)
            {
                return isChildGroupContain;
            }
        }

        return false;
    }

    private async Task<int> GetGroupIdAsync(int subgroupId, CancellationToken cancellationToken)
    {
        var subGroup = await _knowledgeBaseDbContext.Groups
            .Where(group => group.Id == subgroupId)
            .Select(group => new
            {
                group.Id,
                group.ParentGroupId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (subGroup is null)
        {
            throw new EntityNotFoundException("Sub group is not exist");
        }

        if (subGroup.ParentGroupId is not null)
        {
            return await GetGroupIdAsync((int)subGroup.ParentGroupId, cancellationToken);
        }

        return subGroup.Id;
    }
}
