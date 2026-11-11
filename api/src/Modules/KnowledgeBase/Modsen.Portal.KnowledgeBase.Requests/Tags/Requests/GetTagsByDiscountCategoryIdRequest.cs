using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;

internal sealed class GetTagsByDiscountCategoryIdRequest : IQueryHandler<GetTagsByDiscountCategoryIdRequest.Query, List<TagDto>>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public GetTagsByDiscountCategoryIdRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<List<TagDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await _knowledgeBaseDbContext.DiscountCategoryTags
            .AsNoTracking()
            .Where(x => x.DiscountCategoryId == request.DiscountCategoryId)
            .Select(x => new TagDto
            {
                Id = x.TagId,
                Name = x.Tag!.Name
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(int DiscountCategoryId) : IQuery<List<TagDto>>;
}
