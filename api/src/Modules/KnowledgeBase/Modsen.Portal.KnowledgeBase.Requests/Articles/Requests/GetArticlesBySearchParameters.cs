using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Core;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.Articles.Requests;

internal sealed class GetArticlesBySearchParameters : IQueryHandler<GetArticlesBySearchParameters.Query, List<ArticleDto>>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IArticleLoader _articleLoader;

    public GetArticlesBySearchParameters(
        IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IArticleLoader articleLoader)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _articleLoader = articleLoader;
    }

    public async Task<List<ArticleDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var keywords = request.Keywords?.ToLower(CultureInfo.InvariantCulture).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];
        var name = request.Name?.ToLower(CultureInfo.InvariantCulture) ?? string.Empty;

        var articleIds = await _knowledgeBaseDbContext.Articles
        .Where(x => request.GroupId == null || x.GroupId == request.GroupId)
        .Where(x => request.TagId == null || x.ArticleTags.Any(t => t.Id == request.TagId))
        .Where(x => request.Name == null || x.Name.ToLower().Contains(name))
        .Where(x => keywords.Length == 0 || keywords.Any(key => x.Keywords != null && x.Keywords.ToLower().Contains(key)))
        .Select(x => x.Id)
        .Distinct()
        .ToArrayAsync(cancellationToken);

        return await _articleLoader.LoadAsync(articleIds, cancellationToken);
    }

    internal sealed record Query(
        int? GroupId,
        int? TagId,
        string? Name,
        string? Keywords) : IQuery<List<ArticleDto>>;
}
