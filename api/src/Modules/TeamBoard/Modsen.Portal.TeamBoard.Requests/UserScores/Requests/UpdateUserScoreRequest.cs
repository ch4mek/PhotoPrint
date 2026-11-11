using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.UserScores.Core;

namespace Modsen.Portal.TeamBoard.Requests.UserScores.Requests;

internal sealed class UpdateUserScoreRequest : ICommandHandler<UpdateUserScoreRequest.Command, int>
{
    private readonly IAuthenticatedUserContext _authenticatedUser;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IScoreSignatureValidator _signatureValidator;

    public UpdateUserScoreRequest(
        IAuthenticatedUserContext authenticatedUser,
        ITeamBoardDbContext teamBoardDbContext,
        IScoreSignatureValidator scoreSignatureValidator)
    {
        _authenticatedUser = authenticatedUser;
        _teamBoardDbContext = teamBoardDbContext;
        _signatureValidator = scoreSignatureValidator;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var userId = _authenticatedUser.GetRequiredUserId();

        if (!_signatureValidator.IsValid(userId, request.Score, request.Timestamp, request.Signature))
        {
            throw new InvalidDataAppException("Invalid score signature.");
        }

        var user = await _teamBoardDbContext.TeamBoardUsers
            .Include(x => x.UserScore)
            .FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken)
            ?? throw new EntityNotFoundException($"User with {userId} Id was not found");

        var score = user.UserScore;

        if (user.UserScore == null)
        {
            user.UserScore = new UserScore
            {
                UserId = userId,
                Score = request.Score
            };

            await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
        }
        else if (request.Score > user.UserScore.Score)
        {
            user.UserScore.Score = request.Score;
        }

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return user.UserScore.Id;
    }

    internal sealed record Command(
    int Score,
    long Timestamp,
    string Signature) : ICommand<int>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Score)
                .GreaterThanOrEqualTo(0);
        }
    }
}
