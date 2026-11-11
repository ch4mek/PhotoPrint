using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.AuthenticationFilters;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests;

internal sealed class GetOdpConversationsRequest
    : IQueryHandler<GetOdpConversationsRequest.Query, PageableListOfItems<OdpConversation>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IConversationsAuthenticationFilter _authenticationFilter;

    public GetOdpConversationsRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IConversationsAuthenticationFilter authenticationFilter)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticationFilter = authenticationFilter;
    }

    public async Task<PageableListOfItems<OdpConversation>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _authenticationFilter.Apply(_teamBoardDbContext.Conversations)
            .AsNoTracking()
            .Include(x => x.ConversationDocumentLinks
                .OrderBy(f => f.Id))
            .Include(x => x.ConversationFiles
                .OrderBy(f => f.Id))
            .Include(x => x.User)
                .ThenInclude(u => u!.UserPerformanceReviews)
            .Include(x => x.Supervisor)
            .AsSplitQuery()
            .Select(OdpConversation.MapConversation);

        var total = await request.QueryOptions
            .ApplyTo(query)
            .OfType<OdpConversation>()
            .CountAsync(cancellationToken);

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpConversation>.Value;
        }

        var result = await request.QueryOptions
            .ApplyTo(query)
            .Cast<OdpConversation>()
            .ToListAsync(cancellationToken);

        return result.ToPageable(request.QueryOptions, total);
    }

    internal sealed record Query(ODataQueryOptions<OdpConversation> QueryOptions) : IQuery<PageableListOfItems<OdpConversation>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
