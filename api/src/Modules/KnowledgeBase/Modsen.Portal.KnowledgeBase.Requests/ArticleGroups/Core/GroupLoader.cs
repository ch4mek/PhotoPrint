using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests.RequestDto;
using Modsen.Portal.KnowledgeBase.Requests.Dto;

namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;

internal interface IGroupLoader
{
    Task<GroupDto> LoadAsync(int groupId, CancellationToken cancellationToken);

    Task<List<GroupDto>> LoadGroupsAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
}

internal class GroupLoader : IGroupLoader
{
    private readonly IKnowledgeBaseDbContext _dbContext;

    public GroupLoader(IKnowledgeBaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GroupDto> LoadAsync(int groupId, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Groups
            .Where(group => !group.IsDeleted)
            .Include(g => g.Articles)
            .Include(g => g.CreatedByUser)
            .Include(g => g.UpdatedByUser)
            .Include(g => g.ChildGroups)
            .ThenInclude(g => g.Articles)
            .AsSplitQuery()
            .OrderByDescending(x => x.OrderWeight)
            .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);

        if (result is null || result.IsDeleted)
        {
            throw new EntityNotFoundException($"Article Group by id {groupId} is not found");
        }

        var resultDto = FillGroupDto(result);

        return resultDto;
    }

    public async Task<List<GroupDto>> LoadGroupsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
    {
        var entities = await _dbContext.Groups
            .Where(g => !g.IsDeleted && ids.Contains(g.Id))
            .Include(g => g.Articles)
            .Include(g => g.CreatedByUser)
            .Include(g => g.UpdatedByUser)
            .Include(g => g.ChildGroups)
            .ThenInclude(c => c.Articles)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var resultDtos = entities.Select(FillGroupDto).ToList();

        foreach (var group in resultDtos)
        {
            var childGroupIds = group.ChildGroups
                .Select(childGroup => childGroup.Id)
                .ToList();

            group.ChildGroups = await LoadGroupsAsync(childGroupIds, cancellationToken);
        }

        return resultDtos;
    }

    private GroupDto FillGroupDto(GroupEntity result)
    {
        var childGroups = result.ChildGroups
            .Where(g => !g.IsDeleted)
            .Select(FillGroupDto)
            .ToList();

        return new GroupDto {
            Id = result.Id,
            ParentGroupId = result.ParentGroupId,
            Name = result.Name,
            Icon = result.Icon,
            IsActive = result.IsActive,
            IsAvailableOnMainScreen = result.IsAvailableOnMainScreen,
            OrderWeight = result.OrderWeight,
            CreatedByUser = new KnowledgeBaseUserDto
            {
                Id = result.CreatedByUser?.UserId ?? 0,
                FirstName = result.CreatedByUser?.FirstName ?? string.Empty,
                Lastname = result.CreatedByUser?.LastName ?? string.Empty,
                Email = result.CreatedByUser?.Email ?? string.Empty,
                DepartmentId = result.CreatedByUser?.DepartmentId ?? string.Empty
            },
            CreatedAtUtc = result.CreatedAtUtc,
            UpdatedByUser = new KnowledgeBaseUserDto
            {
                Id = result.UpdatedByUser?.UserId ?? 0,
                FirstName = result.UpdatedByUser?.FirstName ?? string.Empty,
                Lastname = result.UpdatedByUser?.LastName ?? string.Empty,
                Email = result.UpdatedByUser?.Email ?? string.Empty,
                DepartmentId = result.UpdatedByUser?.DepartmentId ?? string.Empty
            },
            UpdatedAtUtc = result.UpdatedAtUtc,
            ChildGroups = childGroups,
            Articles = result.Articles.Select(a =>
            new GroupArticleDto(
                Id: a.Id,
                Name: a.Name,
                Icon: a.Icon,
                IsActive: a.IsActive,
                IsAvailableOnMainScreen: a.IsAvailableOnMainScreen,
                OrderWeight: a.OrderWeight,
                CreatedAtUtc: a.CreatedAtUtc,
                UpdatedAtUtc: a.UpdatedAtUtc)).ToList()
        };
    }
}
