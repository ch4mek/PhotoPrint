using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests;

internal sealed class GetGroupByIdRequest : IQueryHandler<GetGroupByIdRequest.Query, GroupDto>
{
    private readonly IGroupLoader _groupLoader;

    public GetGroupByIdRequest(IGroupLoader groupLoader)
    {
        _groupLoader = groupLoader;
    }

    public async Task<GroupDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var resultDto = await _groupLoader.LoadAsync(request.Id, cancellationToken);

        return resultDto;
    }

    internal sealed record Query(int Id) : IQuery<GroupDto>, ITransactionalRequest;
}
