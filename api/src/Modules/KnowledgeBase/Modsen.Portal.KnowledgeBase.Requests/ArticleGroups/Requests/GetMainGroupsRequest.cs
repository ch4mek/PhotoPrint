using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests.RequestDto;

namespace Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Requests;

internal sealed class GetMainGroupsRequest : IQueryHandler<GetMainGroupsRequest.Query, List<GroupDto>>
{
    private readonly IKnowledgeBaseDbContext _dbContext;
    private readonly IGroupLoader _groupLoader;

    public GetMainGroupsRequest(
        IKnowledgeBaseDbContext dbContext,
        IGroupLoader groupLoader)
    {
        _dbContext = dbContext;
        _groupLoader = groupLoader;
    }

    public async Task<List<GroupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var groupIds = await _dbContext.Groups
            .Where(group => !group.IsDeleted)
            .Where(g => g.ParentGroupId == null)
            .Select(group => group.Id)
            .ToArrayAsync(cancellationToken);

        var groupsDto = await _groupLoader.LoadGroupsAsync(groupIds, cancellationToken);

        return groupsDto;
    }

    internal sealed record Query : IQuery<List<GroupDto>>, ITransactionalRequest;
}
