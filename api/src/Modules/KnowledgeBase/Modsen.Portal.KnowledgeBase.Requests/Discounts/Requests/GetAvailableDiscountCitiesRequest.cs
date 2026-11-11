using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;

internal sealed class GetAvailableDiscountCitiesRequest : IQueryHandler<GetAvailableDiscountCitiesRequest.Query, List<DiscountCityCoreDto>>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public GetAvailableDiscountCitiesRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<List<DiscountCityCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await _knowledgeBaseDbContext.Cities
            .AsNoTracking()
            .Select(x => new DiscountCityCoreDto
            {
                CityId = x.Id,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<DiscountCityCoreDto>>;
}
