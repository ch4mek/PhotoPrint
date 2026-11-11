using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;
using Modsen.Portal.KnowledgeBase.Requests.Dto;

namespace Modsen.Portal.KnowledgeBase.Requests.Articles.Core;

internal interface IArticleLoader
{
    Task<ArticleDto?> LoadAsync(int articleId, CancellationToken cancellationToken);
    Task<List<ArticleDto>> LoadAsync(int[] articleIds, CancellationToken cancellationToken);
}

internal class ArticleLoader : IArticleLoader
{
    private readonly IKnowledgeBaseDbContext _dbContext;

    public ArticleLoader(IKnowledgeBaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ArticleDto>> LoadAsync(int[] articleIds, CancellationToken cancellationToken)
    {
        return await _dbContext.Articles
            .Where(x => articleIds.Contains(x.Id))
            .Include(x => x.CreatedByUser)
            .Include(x => x.UpdatedByUser)
            .Include(x => x.ArticleReferences)
            .Include(x => x.ArticleTags)
            .ThenInclude(bt => bt.Tag)
            .AsSplitQuery()
            .OrderByDescending(x => x.OrderWeight)
            .Select(x => new ArticleDto
            {
                Id = x.Id,
                GroupId = x.GroupId,
                Name = x.Name,
                Icon = x.Icon,
                Views = x.Views,
                Text = x.Text,
                Keywords = x.Keywords,
                IsActive = x.IsActive,
                IsAvailableOnMainScreen = x.IsAvailableOnMainScreen,
                OrderWeight = x.OrderWeight,
                CreatedByUser = x.CreatedByUser != null ? new KnowledgeBaseUserDto
                {
                    Id = x.CreatedByUser.UserId,
                    FirstName = x.CreatedByUser.FirstName,
                    Lastname = x.CreatedByUser.LastName,
                    Email = x.CreatedByUser.Email,
                    DepartmentId = x.CreatedByUser.DepartmentId
                } : new KnowledgeBaseUserDto
                {
                    Id = 0,
                    FirstName = string.Empty,
                    Lastname = string.Empty,
                    Email = string.Empty,
                    DepartmentId = string.Empty
                },
                CreatedAtUtc = x.CreatedAtUtc,
                UpdatedByUser = x.UpdatedByUser != null ? new KnowledgeBaseUserDto
                {
                    Id = x.UpdatedByUser.UserId,
                    FirstName = x.UpdatedByUser.FirstName,
                    Lastname = x.UpdatedByUser.LastName,
                    Email = x.UpdatedByUser.Email,
                    DepartmentId = x.UpdatedByUser.DepartmentId
                } : new KnowledgeBaseUserDto
                {
                    Id = 0,
                    FirstName = string.Empty,
                    Lastname = string.Empty,
                    Email = string.Empty,
                    DepartmentId = string.Empty
                },
                ArticleReferences = x.ArticleReferences
                .Select(r => new ArticleReferenceDto
                {
                    Id = r.Id,
                    InternalFileId = r.InternalFileId,
                    ExternalUrl = r.ExternalUrl,
                }).ToList(),
                ArticleTags = x.ArticleTags.Select(t => new TagDto
                {
                    Id = t.Tag!.Id,
                    Name = t.Tag.Name
                }).ToList()
            }).ToListAsync(cancellationToken);
    }

    public async Task<ArticleDto?> LoadAsync(int articleId, CancellationToken cancellationToken)
    {
        var articles = await LoadAsync([articleId], cancellationToken);

        if (articles.Count == 0)
        {
            throw new EntityNotFoundException($"Article with Id: {articleId} couldn't be found.");
        }

        return articles.FirstOrDefault();
    }
}
