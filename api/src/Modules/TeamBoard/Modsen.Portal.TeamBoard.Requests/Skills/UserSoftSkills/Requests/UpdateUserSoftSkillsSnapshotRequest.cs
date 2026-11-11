using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests;

internal sealed class UpdateUserSoftSkillsSnapshotRequest
    : ICommandHandler<UpdateUserSoftSkillsSnapshotRequest.Command, int>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IUserSoftSkillSnapshotValidator _snapshotValidator;
    private readonly ITeamBoardDbContext _dbContext;
    private readonly IClockService _clockService;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;

    public UpdateUserSoftSkillsSnapshotRequest(IAuthenticatedUserContext authenticatedUserContext,
        IUserSoftSkillSnapshotValidator snapshotValidator,
        ITeamBoardDbContext dbContext,
        IClockService clockService,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _snapshotValidator = snapshotValidator;
        _dbContext = dbContext;
        _clockService = clockService;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;

    }

    public async Task<int> Handle(Command command, CancellationToken cancellationToken)
    {
        var request = command.RequestDto;

        var snapshot = await _dbContext.UserSoftSkillsSnapshots
            .Where(snapshot => snapshot.Id == request.SnapshotId)
            .Include(snapshot => snapshot.SoftSkills)
            .FirstOrDefaultAsync(cancellationToken);

        await ValidateSnapshot(snapshot, cancellationToken);
        await _snapshotValidator.ValidateAsync(request.SkillsSeniorityLevels, cancellationToken);

        return await UpdateSnapshotAsync(snapshot!, request.SkillsSeniorityLevels, cancellationToken);
    }

    private async Task ValidateSnapshot(UserSoftSkillsSnapshot? snapshot, CancellationToken cancellationToken)
    {
        if (snapshot is null)
        {
            throw new InvalidDataAppException("Snapshot not found");
        }

        var canTeamBoardUserBeRetrieved =
            await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(snapshot.UserId, AccessOptions.Manager, cancellationToken);
        if (!canTeamBoardUserBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        var maxEditAvailableDate = DateOnly.FromDateTime(new DateTime(
            snapshot.SnapshotDate.Year, snapshot.SnapshotDate.Month, snapshot.SnapshotDate.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(3));

        var editUnavailable = DateOnly.FromDateTime(_clockService.UtcNow) > maxEditAvailableDate;
        if (editUnavailable)
        {
            throw new EntityConflictException("You can't edit snapshot after 3 days left");
        }
    }

    private async Task<int> UpdateSnapshotAsync(UserSoftSkillsSnapshot snapshot,
        IReadOnlyCollection<UserSoftSkillSeniorityLevelDto> seniorityLevels,
        CancellationToken cancellationToken)
    {
        snapshot.UpdatedAtUtc = _clockService.UtcNow;
        snapshot.UpdatedById = _authenticatedUserContext.UserId;

        _dbContext.UserSoftSkills.RemoveRange(snapshot.SoftSkills);

        foreach (var softSkillLevel in seniorityLevels)
        {
            snapshot.SoftSkills.Add(new UserSoftSkill
            {
                UserId = snapshot.UserId,
                SoftSkillId = softSkillLevel.SoftSkillId,
                SeniorityLevelId = softSkillLevel.SeniorityLevelId,
                IsVisibleSeniorityLevel = true,
                IsVisibleSoftSkill = true
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return snapshot.Id;
    }

    internal sealed record Command(UpdateUserSoftSkillsSnapshotDto RequestDto) : ICommand<int>, ITransactionalRequest;
}
