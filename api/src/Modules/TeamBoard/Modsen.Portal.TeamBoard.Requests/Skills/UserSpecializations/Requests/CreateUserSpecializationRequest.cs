using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.BusinessRules;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests;

internal sealed class CreateUserSpecializationRequest
    : ICommandHandler<CreateUserSpecializationRequest.Command, UserSpecializationCoreDto>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ISpecializationSnapshotComposer _specializationSnapshotComposer;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IAreAllHardSkillsListedBusinessRule _areAllHardSkillsListedBusinessRule;
    private readonly IUserSpecializationLoader _userSpecializationLoader;
    private readonly IClockService _clockService;
    private readonly IHardSkillsExistBusinessRule _hardSkillsExistBusinessRule;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;

    private CancellationToken _cancellationToken;

    public CreateUserSpecializationRequest(ITeamBoardDbContext teamBoardDbContext,
        ISpecializationSnapshotComposer specializationSnapshotComposer,
        IAuthenticatedUserContext authenticatedUserContext,
        IClockService clockService,
        IAreAllHardSkillsListedBusinessRule areAllHardSkillsListedBusinessRule,
        IUserSpecializationLoader userSpecializationLoader,
        IHardSkillsExistBusinessRule hardSkillsExistBusinessRule,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _specializationSnapshotComposer = specializationSnapshotComposer;
        _authenticatedUserContext = authenticatedUserContext;
        _clockService = clockService;
        _areAllHardSkillsListedBusinessRule = areAllHardSkillsListedBusinessRule;
        _userSpecializationLoader = userSpecializationLoader;
        _hardSkillsExistBusinessRule = hardSkillsExistBusinessRule;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
    }

    public async Task<UserSpecializationCoreDto> Handle(Command request, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        var canTeamBoardUserBeRetrieved =
            await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(request.UserId, AccessOptions.Manager, cancellationToken);
        if (!canTeamBoardUserBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        var areAllHardSkillsListed = await _areAllHardSkillsListedBusinessRule.IsCorrespondsToBusiness(
            request.SpecializationTemplateId, request.HardSkills, _cancellationToken);
        if (!areAllHardSkillsListed)
        {
            throw new EntityConflictException("Not all hard skills are listed in request");
        }

        var isSeniorityLevelExists =
            !request.SpecializationSeniorityLevelId.IsNullOrWhiteSpace() &&
            await _teamBoardDbContext.SeniorityLevels.AnyAsync(seniority => seniority.Id == request.SpecializationSeniorityLevelId, cancellationToken);
        if (!isSeniorityLevelExists)
        {
            throw new EntityNotFoundException($"Seniority with id {request.SpecializationSeniorityLevelId} does not exist");
        }

        if (!SeniorityLevel.IsSeniorityLevelSigned(request.SpecializationSeniorityLevelId, checkForNoneSeniorityLevel: true))
        {
            throw new InvalidDataAppException("Specialization seniority level without sign can't be used");
        }

        await _hardSkillsExistBusinessRule.IsCorrespondsToBusiness(request.HardSkills, cancellationToken);

        var isPrimarySpecializationExists = await IsUserPrimarySpecializationExist(request.UserId);

        var createdUserSpecialization = await CreateUserSpecialization(request,
            isPrimary: !isPrimarySpecializationExists,
            seniorityLevelId: request.SpecializationSeniorityLevelId);

        await _specializationSnapshotComposer.Create(request.UserId, cancellationToken);

        return (await _userSpecializationLoader.Load(createdUserSpecialization.Id, cancellationToken))!;
    }

    private async Task<UserSpecialization> CreateUserSpecialization(Command request, bool isPrimary, string seniorityLevelId)
    {
        var userSpecialization = new UserSpecialization
        {
            UserId = request.UserId,
            SpecializationTemplateId = request.SpecializationTemplateId,
            IsPrimary = isPrimary,
            SeniorityLevelId = seniorityLevelId,
            IsVisibleSeniorityLevel = true,
            IsVisibleSpecialization = true,
            CreatedAtUtc = _clockService.UtcNow,
            CreatedById = _authenticatedUserContext.UserId,
            SpecializationHardSkills = request.HardSkills
                .Select(hs => new UserSpecializationHardSkill
                {
                    HardSkillId = hs.Id,
                    SeniorityLevelId = hs.SeniorityLevelId
                }).ToList()
        };

        _teamBoardDbContext.UserSpecializations.Add(userSpecialization);
        await _teamBoardDbContext.SaveChangesAsync(_cancellationToken);

        return userSpecialization;
    }

    private Task<bool> IsUserPrimarySpecializationExist(int userId)
    {
        return  _teamBoardDbContext.UserSpecializations
            .Where(specialization => specialization.UserId == userId)
            .Where(specialization => specialization.IsPrimary)
            .AnyAsync(_cancellationToken);
    }

    internal sealed record Command(
        int UserId,
        int SpecializationTemplateId,
        string SpecializationSeniorityLevelId,
        IReadOnlyCollection<HardSkillSeniorityDto> HardSkills) : ICommand<UserSpecializationCoreDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.SpecializationTemplateId)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.SpecializationSeniorityLevelId)
                .NotNull()
                .NotEmpty();
        }
    }
}
