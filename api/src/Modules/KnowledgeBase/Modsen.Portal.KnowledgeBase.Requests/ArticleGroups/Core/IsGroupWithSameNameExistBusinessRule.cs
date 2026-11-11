using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;

internal interface IIsGroupWithSameNameExistBusinessRule
{
    Task<(bool IsContain, string? Message)> IsCorrespondToRuleAsync(
        int? parentGroupId, string name, CancellationToken cancellationToken, int? subgroupId = null);
}

internal sealed class IsGroupWithSameNameExistBusinessRule : IIsGroupWithSameNameExistBusinessRule
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public IsGroupWithSameNameExistBusinessRule(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<(bool IsContain, string? Message)> IsCorrespondToRuleAsync(
        int? parentGroupId, string name, CancellationToken cancellationToken, int? subgroupId = null)
    {
        if (parentGroupId is null)
        {
            return await IsMainGroupWithSameNameExist(subgroupId, name, cancellationToken);
        }

        var groupId = await GetGroupIdAsync((int)parentGroupId, cancellationToken);


        return await IsGroupWithSameNameExist(groupId, subgroupId, name, cancellationToken);
    }

    private async Task<(bool IsContain, string? Message)> IsMainGroupWithSameNameExist(int? groupId, string name, CancellationToken cancellationToken)
    {
        var isMainGroupsContain = await _knowledgeBaseDbContext.Groups
            .Where(group => !group.IsDeleted)
            .Where(group => group.ParentGroupId == null)
            .Where(group => group.Id != groupId)
            .Where(group => group.Name == name)
            .AnyAsync(cancellationToken);

        if (isMainGroupsContain)
        {
            return (isMainGroupsContain, "Group with the same name already exists");
        }

        var childGroupIds = await _knowledgeBaseDbContext.Groups
            .Where(group => !group.IsDeleted)
            .Where(group => group.Id == groupId)
            .Select(group => group.ChildGroups.Where(childGroup => !childGroup.IsDeleted).Select(childGroup => childGroup.Id))
            .FirstOrDefaultAsync(cancellationToken);

        if (childGroupIds is null)
        {
            return (false, null);
        }

        foreach (var childGroupId in childGroupIds)
        {
            var isChildGroupContain = await IsGroupWithSameNameExist(childGroupId, null, name, cancellationToken);

            if (isChildGroupContain.IsContain)
            {
                return isChildGroupContain;
            }
        }

        return (false, null);
    }

    private async Task<(bool IsContain, string? Message)> IsGroupWithSameNameExist(int groupId, int? subgroupId, string name, CancellationToken cancellationToken)
    {
        var subGroup = await _knowledgeBaseDbContext.Groups
            .Where(group => !group.IsDeleted)
            .Where(group => group.Id == groupId)
            .Select(group => new
            {
                IsContain = group.Name == name
                    && group.Id != subgroupId
                    && group.ParentGroupId != null,
                ChildGroupIds = group.ChildGroups.Where(childGroup => !childGroup.IsDeleted).Select(childGroup => childGroup.Id),
                group.ParentGroupId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (subGroup is null)
        {
            throw new EntityNotFoundException("Sub group is not exist");
        }

        if (subGroup.IsContain)
        {
            var message = subGroup.ParentGroupId is null ?
                "Group with the same name already exists" :
                "Subgroup with the same name already exists";

            return (true, message);
        }

        foreach (var childGroupId in subGroup.ChildGroupIds)
        {
            var isChildGroupContain = await IsGroupWithSameNameExist(childGroupId, subgroupId, name, cancellationToken);

            if (isChildGroupContain.IsContain)
            {
                return isChildGroupContain;
            }
        }

        return (false, null);
    }

    private async Task<int> GetGroupIdAsync(int subgroupId, CancellationToken cancellationToken)
    {
        var subGroup = await _knowledgeBaseDbContext.Groups
            .Where(group => !group.IsDeleted)
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
