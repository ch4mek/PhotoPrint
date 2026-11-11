using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Tags.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;

internal sealed class GetTagByIdRequest : IQueryHandler<GetTagByIdRequest.Query, TagDto>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public GetTagByIdRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<TagDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var result = await _knowledgeBaseDbContext.Tags
           .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
           ?? throw new EntityNotFoundException($"Tag by id {request.Id} is not found");

        return new TagDto
        {
            Id = result.Id,
            Name = result.Name
        };
    }

    internal sealed record Query(int Id) : IQuery<TagDto>;
}
