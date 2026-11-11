using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Requests;

internal sealed class UpdateUserPayloadDetailsRequest : ICommandHandler<UpdateUserPayloadDetailsRequest.Command, string?>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public UpdateUserPayloadDetailsRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<string?> Handle(Command request, CancellationToken cancellationToken)
    {
        var teamboardUser = await _teamBoardDbContext.TeamBoardUsers
            .Where(x => x.UserId == request.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (teamboardUser is null)
        {
            throw new EntityNotFoundException("User is not exist");
        }

        if (teamboardUser.Position.IsIntern())
        {
            throw new ActionNotAllowedException(PortalMessages.NotAllowedOperationWithIntern);
        }

        teamboardUser.AvatarColor = request.Color;

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return request.Color;
    }

    internal sealed record Command(int UserId, string? Color) : ICommand<string?>, ITransactionalRequest;
}
