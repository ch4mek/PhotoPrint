using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

namespace Modsen.Portal.KnowledgeBase.Requests.Articles.Requests;

internal sealed class DeleteArticleRequest : ICommandHandler<DeleteArticleRequest.Command, int>
{
    private readonly IKnowledgeBaseDbContext _dbContext;

    public DeleteArticleRequest(IKnowledgeBaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.Articles
            .Where(article => article.Id == request.Id)
            .Include(article => article.ArticleTags)
            .Include(article => article.ArticleReferences)
            .AsSplitQuery()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException($"There is no article with Id {request.Id}");

        _dbContext.ArticleTags.RemoveRange(entity.ArticleTags);
        _dbContext.ArticleReferences.RemoveRange(entity.ArticleReferences);
        _dbContext.Articles.Remove(entity);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return request.Id;
    }

    internal sealed record Command(
        int Id)
        : ICommand<int>, ITransactionalRequest;
}
