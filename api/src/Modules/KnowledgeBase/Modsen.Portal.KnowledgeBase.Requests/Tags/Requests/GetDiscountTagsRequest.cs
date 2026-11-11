using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;

internal sealed class GetDiscountTagsRequest : IQueryHandler<GetDiscountTagsRequest.Query, List<TagDto>>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public GetDiscountTagsRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<List<TagDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await _knowledgeBaseDbContext.Tags
            .AsNoTracking()
            .Where(x => x.DiscountTags.Any())
            .Select(x => new TagDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<TagDto>>;
}
