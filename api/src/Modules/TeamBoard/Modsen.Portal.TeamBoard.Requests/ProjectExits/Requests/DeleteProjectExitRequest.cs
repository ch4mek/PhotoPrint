using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;

internal sealed class DeleteProjectExitRequest : ICommandHandler<DeleteProjectExitRequest.Command, int>
{
    private readonly ITeamBoardDbContext _dbContext;

    public DeleteProjectExitRequest(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> Handle(Command request, CancellationToken cancellationToken)
    {
        var projectExit = await _dbContext.ProjectExits
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsActive, cancellationToken)
            ?? throw new EntityNotFoundException("Project exit not found");

        projectExit.IsActive = false;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return projectExit.Id;
    }

    internal sealed record Command(int Id) : ICommand<int>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
