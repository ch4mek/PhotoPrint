using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.DiscountCategory.Core;

internal interface IDiscountCategoryLoader
{
    Task<DiscountCategoryCoreDto> Load(int discountCategoryId, CancellationToken cancellationToken);
    Task<List<DiscountCategoryCoreDto>> Load(int[] discountCotegoryIds, CancellationToken cancellationToken);
}

internal class DiscountCategoryLoader : IDiscountCategoryLoader
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public DiscountCategoryLoader(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<DiscountCategoryCoreDto> Load(int discountCategoryId, CancellationToken cancellationToken)
    {
        var discounts = await Load([discountCategoryId], cancellationToken);
        return discounts.FirstOrDefault()
            ?? throw new EntityNotFoundException($"Discount category by id {discountCategoryId} not found");
    }

    public async Task<List<DiscountCategoryCoreDto>> Load(int[] discountCotegoryIds, CancellationToken cancellationToken)
    {
        return await _knowledgeBaseDbContext.DiscountCategories
            .Where(x => discountCotegoryIds.Contains(x.Id))
            .Include(x => x.DiscountCategoryTags)
            .ThenInclude(x => x.Tag)
            .AsSplitQuery()
            .AsNoTracking()
            .OrderBy(x => x.OrderWeight)
            .Select(x => new DiscountCategoryCoreDto
            {
                Id = x.Id,
                Name = x.Name,
                OrderWeight = x.OrderWeight,
                CreatedById = x.CreatedById,
                CreatedAtUtc = x.CreatedAtUtc,
                UpdatedById = x.UpdatedById,
                UpdatedAtUtc = x.UpdatedAtUtc,
                DiscountCategoryTags = x.DiscountCategoryTags
                .Select(t => new DiscountCategoryTagCoreDto
                {
                    Id = t.Id,
                    DiscountCategoryId = t.DiscountCategoryId,
                    TagId = t.TagId,
                    Name = t.Tag!.Name
                }).ToList()
            }).ToListAsync(cancellationToken);
    }
}
