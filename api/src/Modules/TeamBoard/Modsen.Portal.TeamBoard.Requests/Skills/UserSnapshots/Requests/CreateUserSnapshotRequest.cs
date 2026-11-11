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
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Requests;

internal sealed class CreateUserSnapshotRequest(
    ITeamBoardDbContext teamBoardDbContext,
    ISpecializationSnapshotComposer specializationSnapshotComposer,
    ICreateUserSnapshotValidationService validationService,
    IUserSkillsSnapshotLoader userSkillsSnapshotLoader,
    IAuthenticatedUserContext authenticatedUserContext,
    ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
    IClockService clockService)
    : ICommandHandler<CreateUserSnapshotRequest.Command, UserSnapshotCoreDto>
{
    public async Task<UserSnapshotCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var canTeamBoardUserBeRetrieved =
            await canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(request.UserId, AccessOptions.Manager, cancellationToken);
        if (!canTeamBoardUserBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        await validationService.ValidateCommandAsync(request, cancellationToken);
        var userSpecializations = await GetUserSpecializationsByUserId(request.UserId, cancellationToken);

        foreach (var specialization in request.UserSpecializations)
        {
            await validationService.ValidateHardSkillsListAsync(specialization, cancellationToken);

            var userSpecialization = userSpecializations
                .FirstOrDefault(us => us.SpecializationTemplateId == specialization.SpecializationTemplateId);

            if (userSpecialization is null)
            {
                throw new EntityNotFoundException(
                    $"User specialization with specialization template id {specialization.SpecializationTemplateId} does not exist");
            }

            userSpecialization.SeniorityLevelId = specialization.SpecializationSeniorityLevelId;
            userSpecialization.SpecializationHardSkills = specialization.HardSkills
                .Select(hs => new UserSpecializationHardSkill
                {
                    HardSkillId = hs.Id,
                    SeniorityLevelId = hs.SeniorityLevelId,
                    UpdatedAtUtc = clockService.UtcNow,
                    UpdatedById = authenticatedUserContext.UserId
                }).ToList();
        }

        await teamBoardDbContext.SaveChangesAsync(cancellationToken);

        var snapshotId = await specializationSnapshotComposer.CreateForDate(request.UserId, request.SnapshotDate, cancellationToken);

        return (await userSkillsSnapshotLoader.Load(snapshotId, cancellationToken))!;
    }

    private Task<UserSpecialization[]> GetUserSpecializationsByUserId(int userId, CancellationToken cancellationToken)
    {
        return teamBoardDbContext.UserSpecializations
            .Where(specialization => specialization.UserId == userId)
            .Include(specialization => specialization.SpecializationHardSkills)
            .ToArrayAsync(cancellationToken);
    }

    internal sealed record Command(int UserId, DateOnly SnapshotDate,
        IReadOnlyCollection<CreateUserSpecializationDto> UserSpecializations) : ICommand<UserSnapshotCoreDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.SnapshotDate)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.UserSpecializations)
                .NotNull()
                .NotEmpty();
        }
    }
}
