using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Requests;

internal sealed class GetUserSnapshotByDateRequest
    : IQueryHandler<GetUserSnapshotByDateRequest.Query, UserSnapshotCoreDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly IUserSkillsSnapshotLoader _userSkillsSnapshotLoader;

    public GetUserSnapshotByDateRequest(ITeamBoardDbContext teamBoardDbContext,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        IUserSkillsSnapshotLoader userSkillsSnapshotLoader)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _userSkillsSnapshotLoader = userSkillsSnapshotLoader;
    }

    public async Task<UserSnapshotCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var canUserBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule
            .IsCorrespondsToBusiness(request.UserId, cancellationToken);

        if (!canUserBeRetrieved)
        {
            throw new EntityNotFoundException();
        }

        var userSnapshot = await _teamBoardDbContext.UserSkillsSnapshots
            .AsNoTracking()
            .Where(s =>
                s.UserId == request.UserId &&
                !s.IsDeleted &&
                s.SnapshotDate == request.SnapshotDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (userSnapshot is null)
        {
            throw new EntityNotFoundException($"User with id {request.UserId} doesn't have entry for {request.SnapshotDate}");
        }

        return (await _userSkillsSnapshotLoader.Load(userSnapshot.Id, cancellationToken))!;
    }

    internal sealed record Query(int UserId, DateOnly SnapshotDate) : IQuery<UserSnapshotCoreDto>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.SnapshotDate)
                .NotNull()
                .NotEmpty();
        }
    }
}
