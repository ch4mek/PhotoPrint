using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests;

internal sealed class UpdateUserSpecializationsRequest
    : ICommandHandler<UpdateUserSpecializationsRequest.Command, UserSnapshotCoreDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IGetLastSnapshotByUserIdDbQuery _getLastSnapshotByUserIdDbQuery;
    private readonly ISpecializationSnapshotComposer _specializationSnapshotComposer;
    private readonly IUserSkillsSnapshotLoader _userSkillsSnapshotLoader;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IClockService _clockService;
    private readonly IHardSkillsExistBusinessRule _hardSkillsExistBusinessRule;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;

    public UpdateUserSpecializationsRequest(ITeamBoardDbContext teamBoardDbContext,
        IGetLastSnapshotByUserIdDbQuery getLastSnapshotByUserIdDbQuery,
        ISpecializationSnapshotComposer specializationSnapshotComposer,
        IUserSkillsSnapshotLoader userSkillsSnapshotLoader,
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService,
        IHardSkillsExistBusinessRule hardSkillsExistBusinessRule,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
    {
        _getLastSnapshotByUserIdDbQuery = getLastSnapshotByUserIdDbQuery;
        _teamBoardDbContext = teamBoardDbContext;
        _specializationSnapshotComposer = specializationSnapshotComposer;
        _userSkillsSnapshotLoader = userSkillsSnapshotLoader;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
        _hardSkillsExistBusinessRule = hardSkillsExistBusinessRule;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
    }

    public async Task<UserSnapshotCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var canTeamBoardUserBeRetrieved =
            await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(request.UserId, AccessOptions.Manager, cancellationToken);
        if (!canTeamBoardUserBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        var snapshotToUpdate = await CheckCanSnapshotBeUpdated(request.SnapshotId, request.UserId, cancellationToken);

        var userSpecializations = await GetUserSpecializationsByUserId(request.UserId, cancellationToken);

        foreach (var specialization in request.UserSpecializations)
        {
            var userSpecialization = userSpecializations
                .FirstOrDefault(x => x.SpecializationTemplateId == specialization.SpecializationTemplateId);

            if (userSpecialization is null)
            {
                throw new EntityNotFoundException(
                    $"User specialization with specialization id {specialization.SpecializationTemplateId} does not exist");
            }

            var isSeniorityLevelExists = await _teamBoardDbContext.SeniorityLevels
                .AnyAsync(seniority => seniority.Id == specialization.SpecializationSeniorityLevelId, cancellationToken);
            if (!isSeniorityLevelExists)
            {
                throw new EntityNotFoundException($"Seniority with id {specialization.SpecializationSeniorityLevelId} does not exist");
            }

            if (!SeniorityLevel.IsSeniorityLevelSigned(specialization.SpecializationSeniorityLevelId, checkForNoneSeniorityLevel: true))
            {
                throw new InvalidDataAppException("Specialization seniority level without sign can't be used");
            }

            await _hardSkillsExistBusinessRule.IsCorrespondsToBusiness(specialization.HardSkills, cancellationToken);

            userSpecialization.SeniorityLevelId = specialization.SpecializationSeniorityLevelId;

            userSpecialization.SpecializationHardSkills = specialization.HardSkills
                .Select(hs => new UserSpecializationHardSkill
                {
                    HardSkillId = hs.Id,
                    SeniorityLevelId = hs.SeniorityLevelId,
                    UpdatedAtUtc = _clockService.UtcNow,
                    UpdatedById = _authenticatedUserContext.UserId
                }).ToList();
        }

        snapshotToUpdate.IsDeleted = true;

        _teamBoardDbContext.UserSkillsSnapshots.Update(snapshotToUpdate);
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        var snapshotId = await _specializationSnapshotComposer.CreateForDate(
            request.UserId, snapshotToUpdate.SnapshotDate, cancellationToken);

        return (await _userSkillsSnapshotLoader.Load(snapshotId, cancellationToken))!;
    }

    private async Task<UserSkillsSnapshot> CheckCanSnapshotBeUpdated(int snapshotId, int userId, CancellationToken cancellationToken)
    {
        var snapshot = await _teamBoardDbContext.UserSkillsSnapshots
            .Where(s => s.Id == snapshotId && !s.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (snapshot is null)
        {
            throw new EntityNotFoundException($"Entry with id {snapshotId} was not found");
        }

        var lastSnapshot = await _getLastSnapshotByUserIdDbQuery.ExecuteAsync(userId, cancellationToken);
        if (lastSnapshot?.Id != snapshot.Id)
        {
            throw new EntityConflictException("Only last entry can be updated");
        }

        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        if (snapshot.SnapshotDate.AddDays(3) < currentDate)
        {
            throw new EntityConflictException("Only within three days after creation entry can be updated");
        }

        return snapshot;
    }

    private Task<UserSpecialization[]> GetUserSpecializationsByUserId(int userId, CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.UserSpecializations
            .Where(x => x.UserId == userId)
            .Include(x => x.SpecializationHardSkills)
            .ToArrayAsync(cancellationToken);
    }

    internal sealed record Command(
        int SnapshotId,
        int UserId,
        IReadOnlyCollection<CreateUserSpecializationDto> UserSpecializations)
        : ICommand<UserSnapshotCoreDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.SnapshotId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.UserSpecializations)
                .NotNull()
                .NotEmpty();
        }
    }
}
