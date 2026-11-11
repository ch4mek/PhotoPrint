using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests;

internal sealed class GetUniqueExternalFeedbackReviewersRequest : IQueryHandler<GetUniqueExternalFeedbackReviewersRequest.Query, List<string?>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    public GetUniqueExternalFeedbackReviewersRequest(
        ITeamBoardDbContext teamBoardDbContext,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
    }

    public async Task<List<string?>> Handle(Query request, CancellationToken cancellationToken)
    {
        if(!await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(request.UserId, Portal.Core.AccessOptions.Manager, cancellationToken))
        {
            throw new ActionNotAllowedException();
        }

        return await _teamBoardDbContext.ExternalFeedbacks
            .Where(x => x.UserId == request.UserId && x.Reviewer != null)
            .Where(x => x.User!.Position != PortalRoleNames.Intern)
            .Select(x => x.Reviewer)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(int UserId) : IQuery<List<string?>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
