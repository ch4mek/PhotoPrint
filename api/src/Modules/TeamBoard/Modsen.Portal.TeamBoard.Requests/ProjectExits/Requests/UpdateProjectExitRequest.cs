using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;

internal sealed class UpdateProjectExitRequest : ICommandHandler<UpdateProjectExitRequest.Command, OdpProjectExitDto>
{
    private readonly ITeamBoardDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public UpdateProjectExitRequest(ITeamBoardDbContext dbContext, IAuthenticatedUserContext authenticatedUserContext)
    {
        _dbContext = dbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<OdpProjectExitDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var projectExit = await _dbContext.ProjectExits
            .FirstOrDefaultAsync(x => x.Id == request.Id && x.IsActive, cancellationToken)
            ?? throw new EntityNotFoundException("Project exit not found");

        var userExists = await _dbContext.TeamBoardUsers
            .AnyAsync(u => u.UserId == request.UserId, cancellationToken);

        if (!userExists)
        {
            throw new EntityNotFoundException("User not found");
        }

        var projectExists = await _dbContext.TeamBoardProjects
            .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (!projectExists)
        {
            throw new EntityNotFoundException("Project not found");
        }

        var closingReasonId = await GetOrCreateClosingReasonIdAsync(request.ClosingReasonName, cancellationToken);

        projectExit.UserId = request.UserId;
        projectExit.ProjectId = request.ProjectId;
        projectExit.ClientId = request.ClientId;
        projectExit.ClosingDate = request.ClosingDate;
        projectExit.ClosingReasonId = closingReasonId;
        projectExit.UpdatedById = _authenticatedUserContext.UserId;
        projectExit.UpdatedAtUtc = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await _dbContext.ProjectExits
            .Where(x => x.Id == projectExit.Id)
            .Select(OdpProjectExit.Map)
            .Select(OdpProjectExitDto.Map)
            .FirstAsync(cancellationToken);
    }

    private async Task<int> GetOrCreateClosingReasonIdAsync(string name, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.ProjectExitClosingReasons
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);

        if (existing is not null)
        {
            return existing.Id;
        }

        var newReason = new ProjectExitClosingReason { Name = name };
        await _dbContext.ProjectExitClosingReasons.AddAsync(newReason, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return newReason.Id;
    }

    internal sealed record Command(
        int Id,
        int UserId,
        int ProjectId,
        int ClientId,
        DateOnly ClosingDate,
        string ClosingReasonName) : ICommand<OdpProjectExitDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.ProjectId).NotEmpty();

            RuleFor(x => x.ClientId).NotEmpty();

            RuleFor(x => x.ClosingDate)
                .LessThan(x => new DateOnly(3000, 1, 1))
                .NotEmpty();

            RuleFor(x => x.ClosingReasonName)
                .NotEmpty()
                .ShortTextType();
        }
    }
}
