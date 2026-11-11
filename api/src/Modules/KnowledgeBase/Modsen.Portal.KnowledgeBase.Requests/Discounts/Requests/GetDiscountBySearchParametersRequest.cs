using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;

internal sealed class GetDiscountBySearchParametersRequest : IQueryHandler<GetDiscountBySearchParametersRequest.Query, List<DiscountCoreDto>>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IDiscountLoader _discountsLoader;

    public GetDiscountBySearchParametersRequest(
        IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IDiscountLoader discountLoader)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _discountsLoader = discountLoader;
    }

    public async Task<List<DiscountCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var name = request.Name?.ToLower(CultureInfo.InvariantCulture) ?? string.Empty;

        var discountIds = await _knowledgeBaseDbContext.Discounts
        .Where(x => request.DiscountCategoryId == null || x.DiscountCategoryId == request.DiscountCategoryId)
        .Where(x => request.TagId == null || x.DiscountTags.Any(t => t.TagId == request.TagId))
        .Where(x => request.Name == null || x.Name.ToLower().Contains(name))
        .Where(x => (request.CityId == null || x.DiscountCities.Any(c => c.CityId == request.CityId)))
        .Select(x => x.Id)
        .Distinct()
        .ToArrayAsync(cancellationToken);

        return await _discountsLoader.Load(discountIds, cancellationToken);
    }

    internal sealed record Query(
        int? DiscountCategoryId,
        int? TagId,
        string? Name,
        int? CityId) : IQuery<List<DiscountCoreDto>>;
}
