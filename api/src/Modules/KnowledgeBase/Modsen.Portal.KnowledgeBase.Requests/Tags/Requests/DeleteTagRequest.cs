using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

namespace Modsen.Portal.KnowledgeBase.Requests.Tags.Requests;

internal sealed class DeleteTagRequest : ICommandHandler<DeleteTagRequest.Command, int>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;

    public DeleteTagRequest(IKnowledgeBaseDbContext knowledgeBaseDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var entity = await _knowledgeBaseDbContext.Tags
            .Include(t => t.DiscountTags)
            .Include(t => t.ArticleTags)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"There is no tag with Id {request.Id}");

        _knowledgeBaseDbContext.DiscountTags.RemoveRange(entity.DiscountTags);
        _knowledgeBaseDbContext.ArticleTags.RemoveRange(entity.ArticleTags);

        _knowledgeBaseDbContext.Tags.Remove(entity);

        await _knowledgeBaseDbContext.SaveChangesAsync(cancellationToken);

        return request.Id;
    }

    internal sealed record Command(int Id) : ICommand<int>, ITransactionalRequest;
}
