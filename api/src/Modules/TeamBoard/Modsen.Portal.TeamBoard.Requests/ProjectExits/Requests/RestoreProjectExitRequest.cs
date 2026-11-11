using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;

internal sealed class RestoreProjectExitRequest : ICommandHandler<RestoreProjectExitRequest.Command, OdpProjectExitDto>
{
    private readonly ITeamBoardDbContext _dbContext;

    public RestoreProjectExitRequest(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OdpProjectExitDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var projectExit = await _dbContext.ProjectExits
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsActive, cancellationToken)
            ?? throw new EntityNotFoundException("Project exit not found");

        projectExit.IsActive = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await _dbContext.ProjectExits
            .Where(x => x.Id == projectExit.Id)
            .Select(OdpProjectExit.Map)
            .Select(OdpProjectExitDto.Map)
            .FirstAsync(cancellationToken);
    }

    internal sealed record Command(int Id) : ICommand<OdpProjectExitDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
