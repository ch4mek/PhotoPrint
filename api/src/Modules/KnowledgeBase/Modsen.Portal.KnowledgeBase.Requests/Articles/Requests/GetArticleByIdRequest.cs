using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Core;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Articles.Requests;

internal sealed class GetArticleByIdRequest : IQueryHandler<GetArticleByIdRequest.Query, ArticleDto>
{
    private readonly IArticleLoader _articleLoader;

    public GetArticleByIdRequest(IArticleLoader articleLoader)
    {
        _articleLoader = articleLoader;
    }

    public async Task<ArticleDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var articleDto = await _articleLoader.LoadAsync(request.Id, cancellationToken) ??
            throw new EntityNotFoundException($"Cannot find article by ID {request.Id}");

        return articleDto;
    }

    internal sealed record Query(
        int Id)
        : IQuery<ArticleDto>, ITransactionalRequest;
}
