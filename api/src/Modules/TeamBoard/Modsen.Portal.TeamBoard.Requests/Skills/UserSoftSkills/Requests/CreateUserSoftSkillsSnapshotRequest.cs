using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests;

internal sealed class CreateUserSoftSkillsSnapshotRequest
    : ICommandHandler<CreateUserSoftSkillsSnapshotRequest.Command, int>
{
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly IUserSoftSkillSnapshotValidator _snapshotValidator;
    private readonly IUserSoftSkillsSnapshotComposer _snapshotComposer;
    private readonly ITeamBoardDbContext _dbContext;

    public CreateUserSoftSkillsSnapshotRequest(
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        IUserSoftSkillSnapshotValidator snapshotValidator,
        IUserSoftSkillsSnapshotComposer snapshotComposer,
        ITeamBoardDbContext dbContext)
    {
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _snapshotValidator = snapshotValidator;
        _snapshotComposer = snapshotComposer;
        _dbContext = dbContext;
    }

    public async Task<int> Handle(Command command, CancellationToken cancellationToken)
    {
        var request = command.RequestDto;

        var canTeamBoardUserBeRetrieved =
            await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(request.UserId, AccessOptions.Manager, cancellationToken);
        if (!canTeamBoardUserBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        await ValidateSnapshotDateAsync(request.SnapshotDate, request.UserId, cancellationToken);
        await _snapshotValidator.ValidateAsync(request.SkillsSeniorityLevels, cancellationToken);

        var snapshotCoreDto = UserSoftSkillSnapshotCoreDto.Of(request);

        var userSnapshotId = await _snapshotComposer.ComposeAsync(snapshotCoreDto, cancellationToken);

        return userSnapshotId;
    }

    private async Task ValidateSnapshotDateAsync(DateOnly snapshotDate, int userId,
        CancellationToken cancellationToken)
    {
        var snapshotDates = await GetSnapshotDates(userId, cancellationToken);

        var isSnapshotAlreadyExists = snapshotDates.Contains(snapshotDate);
        if (isSnapshotAlreadyExists)
        {
            throw new EntityConflictException("An entry for the selected date already exists");
        }

        var areNoSnapshotsInThePast = snapshotDates.Count == 0 &&
            snapshotDate != DateOnly.FromDateTime(DateTime.UtcNow);
        if (areNoSnapshotsInThePast)
        {
            throw new InvalidDataAppException(
                "You cannot create entries for dates in the past if there is no existed snapshots");
        }

        var isNewSnapshotDateBeforeLastDate = snapshotDates.Count > 0 && (snapshotDate < snapshotDates.Max());
        if (isNewSnapshotDateBeforeLastDate)
        {
            throw new InvalidDataAppException(
                "You cannot create entries for dates before the date in the last column");
        }
    }

    private Task<List<DateOnly>> GetSnapshotDates(int userId, CancellationToken cancellationToken)
    {
        return _dbContext.UserSoftSkillsSnapshots
            .Where(snapshot => snapshot.UserId == userId && snapshot.SoftSkills.Any(us => us.SoftSkill!.IsActive))
            .Select(snapshot => snapshot.SnapshotDate)
            .ToListAsync(cancellationToken);
    }

    internal sealed record Command(CreateUserSoftSkillsSnapshotDto RequestDto) : ICommand<int>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(command => command.RequestDto).SetValidator(new CreateUserSoftSkillsSnapshotDto.Validator());
        }
    }
}
