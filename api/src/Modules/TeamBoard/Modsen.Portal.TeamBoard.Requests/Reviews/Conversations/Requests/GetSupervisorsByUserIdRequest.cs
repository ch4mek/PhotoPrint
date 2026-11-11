using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Core.AuthenticationFilters;
using Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.Conversations.Requests;

internal sealed class GetSupervisorsByUserIdRequest : IQueryHandler<GetSupervisorsByUserIdRequest.Query, List<SupervisorDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly IConversationsAuthenticationFilter _authenticationFilter;

    public GetSupervisorsByUserIdRequest(
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        ITeamBoardDbContext teamBoardDbContext,
        IConversationsAuthenticationFilter conversationsAuthenticationFilter)
    {
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _teamBoardDbContext = teamBoardDbContext;
        _authenticationFilter = conversationsAuthenticationFilter;
    }

    public async Task<List<SupervisorDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var accessOptions = AccessOptions.Self | AccessOptions.Manager | AccessOptions.Hr;
        var canBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(
            request.UserId, accessOptions, cancellationToken);

        if (!canBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        return await _authenticationFilter.Apply(_teamBoardDbContext.Conversations)
            .Where(x => x.UserId == request.UserId)
            .AsNoTracking()
            .Select(x => new SupervisorDto(x.SupervisorId, $"{x.Supervisor!.LastName} {x.Supervisor.FirstName}"))
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(int UserId) : IQuery<List<SupervisorDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotNull().NotEmpty();
        }
    }
}
