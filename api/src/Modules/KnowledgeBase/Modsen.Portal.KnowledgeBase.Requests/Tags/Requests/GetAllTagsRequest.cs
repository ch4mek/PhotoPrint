using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;

internal sealed class GetAllTagsRequest : IQueryHandler<GetAllTagsRequest.Query, List<TagDto>>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public GetAllTagsRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<List<TagDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await _knowledgeBaseDbContext.Tags
          .AsNoTracking()
          .OrderBy(x => x.Id)
          .Select(x => new TagDto
          {
              Id = x.Id,
              Name = x.Name
          })
          .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<List<TagDto>>;
}
