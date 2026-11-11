using Microsoft.EntityFrameworkCore;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;

internal interface IDiscountLoader
{
    Task<DiscountCoreDto?> Load(int discountId, CancellationToken cancellationToken);
    Task<List<DiscountCoreDto>> Load(int[] discountIds, CancellationToken cancellationToken);
}

internal class DiscountLoader : IDiscountLoader
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public DiscountLoader(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<DiscountCoreDto?> Load(int discountId, CancellationToken cancellationToken)
    {
        var discounts = await Load([discountId], cancellationToken);
        return discounts.FirstOrDefault();
    }

    public async Task<List<DiscountCoreDto>> Load(int[] discountIds, CancellationToken cancellationToken)
    {
        return await _knowledgeBaseDbContext.Discounts
            .Where(x => discountIds.Contains(x.Id))
            .Include(x => x.DiscountReferences)
            .Include(x => x.DiscountTags)
            .ThenInclude(bt => bt.Tag)
            .Include(x => x.DiscountCities)
            .ThenInclude(bc => bc.City)
            .AsSplitQuery()
            .OrderBy(x => x.OrderWeight)
            .Select(x => new DiscountCoreDto
            {
                Id = x.Id,
                DiscountCategoryId = x.DiscountCategoryId,
                DiscountCategoryName = x.DiscountCategory!.Name,
                Name = x.Name,
                ImageUrl = x.ImageUrl,
                ImageFileId = x.ImageFileId,
                DiscountPercent = x.DiscountPercent,
                DiscountPercentTo = x.DiscountPercentTo,
                Text = x.Text,
                CompanyType = x.CompanyType,
                Website = x.Website,
                PhoneNumber = x.PhoneNumber,
                Location = x.Location,
                IsActive = x.IsActive,
                OrderWeight = x.OrderWeight,
                DiscountReferences = x.DiscountReferences
                .Select(f => new DiscountReferenceCoreDto
                {
                    Id = f.Id,
                    DiscountId = f.DiscountId,
                    SocialMediaUrl = f.SocialMediaUrl,
                })
                .ToList(),
                DiscountTags = x.DiscountTags
                .Select(f => new DiscountTagCoreDto
                {
                    Id = f.Id,
                    Name = f.Tag!.Name,
                    DiscountId = f.DiscountId,
                    TagId = f.TagId
                })
                .ToList(),
                DiscountCities = x.DiscountCities
                .Select(f => new DiscountCityCoreDto
                {
                    CityId = f.CityId,
                    Name = f.City!.Name
                })
                .ToList()
            }).ToListAsync(cancellationToken);
    }
}
