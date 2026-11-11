using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.LabBoard.CoreDto;
using Modsen.Portal.TeamBoard.Requests.LabBoard.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.LabBoard.Requests;

internal sealed class EditInternshipRequest : ICommandHandler<EditInternshipRequest.Command, InternshipDto>
{
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public EditInternshipRequest(
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<InternshipDto> Handle(Command request, CancellationToken cancellationToken)
    {
        await CheckAccess(request.UserId, cancellationToken);

        var internship = await LoadInternship(request.UserId, trackChanges: true, cancellationToken);

        if (internship == null)
        {
            if (request.InternshipInfo.Id is null)
            {
                await CheckIfCanCreateInternship(request.UserId, cancellationToken);

                internship = new InternshipDetail
                {
                    UserId = request.UserId,
                    Specialization = request.InternshipInfo.Specialization,
                    EngineerStage = request.InternshipInfo.EngineerStage,
                    StartDate = request.InternshipInfo.StartDate,
                    PlannedEndDate = request.InternshipInfo.PlannedEndDate,
                    Type = request.InternshipInfo.Type
                };

                await _teamBoardDbContext.InternshipDetails.AddAsync(internship, cancellationToken);
                await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
            }
            else
            {
                throw new EntityNotFoundException(PortalMessages.InternshipNotFound);
            }
        }

        UpdateInternshipBaseInfo(internship, request.InternshipInfo);

        await UpdateInternsipSpecializations(internship, request.InternshipSpecializations.ToList(), cancellationToken);

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        var internshipForReturn = await LoadInternship(request.UserId, trackChanges: true, cancellationToken);

        return InternshipDto.Of(internshipForReturn!);
    }

    private Task<InternshipDetail?> LoadInternship(int userId, bool trackChanges, CancellationToken cancellationToken)
    {
        var query = _teamBoardDbContext.InternshipDetails.AsQueryable();

        if (!trackChanges)
        {
            query = query.AsNoTracking();
        }

        return query
            .AsSplitQuery()
            .Include(i => i.InternshipSpecializations)
                .ThenInclude(s => s.InternshipSpecializationHardSkills)
                    .ThenInclude(hs => hs.HardSkill)
            .Include(i => i.InternshipSpecializations)
                .ThenInclude(s => s.SeniorityLevel)
            .Include(i => i.InternshipSpecializations)
                .ThenInclude(s => s.SpecializationTemplate)
            .Where(i => i.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task CheckAccess(int userId, CancellationToken cancellationToken)
    {
        var accessOptions = AccessOptions.Self
            | AccessOptions.Manager
            | AccessOptions.Accountant
            | AccessOptions.Coordinator;

        var canBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(
            userId, accessOptions, cancellationToken);

        if (!canBeRetrieved)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }
    }

    private async Task CheckIfCanCreateInternship(int userId, CancellationToken cancellationToken)
    {
        var user = await _teamBoardDbContext.TeamBoardUsers.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);

        if (user is null)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        if (!user.Position.IsIntern())
        {
            throw new InvalidDataAppException(PortalMessages.CannotCreateInternshipForNotIntern);
        }
    }

    private static void UpdateInternshipBaseInfo(InternshipDetail internship, EditInternshipRequestDto newInfo)
    {
        internship.Specialization = newInfo.Specialization;
        internship.ApprovedPoints = newInfo.ApprovedPoints;
        internship.CompletedPoints = newInfo.CompletedPoints;
        internship.EngineerStage = newInfo.EngineerStage;
        internship.StartDate = newInfo.StartDate;
        internship.PlannedEndDate = newInfo.PlannedEndDate;
        internship.Type = newInfo.Type;
        internship.FinalResult = newInfo.FinalResult;
    }

    private async Task UpdateInternsipSpecializations(InternshipDetail internship,
        List<InternshipSpecializationRequestDto> internsipSpecializations,
        CancellationToken cancellationToken)
    {
        var currentSpecializationTemplateIds = internship.InternshipSpecializations.Select(i => i.SpecializationTemplateId).ToList();

        var requestedSpecializationTemplateIds = internsipSpecializations.Select(s => s.SpecializationTemplateId).ToList();

        var specTemplateIdsToAdd = requestedSpecializationTemplateIds.Except(currentSpecializationTemplateIds).ToHashSet();
        var specTemplateIdsToDelete = currentSpecializationTemplateIds.Except(requestedSpecializationTemplateIds).ToHashSet();
        var specTemplateIdsToKeep = currentSpecializationTemplateIds.Intersect(requestedSpecializationTemplateIds).ToHashSet();

        //check if all specIdsToAdd exist
        var newSpecializations = _teamBoardDbContext.SpecializationTemplates.Where(s => specTemplateIdsToAdd.Contains(s.Id));

        if (specTemplateIdsToAdd.Count != newSpecializations.Count())
        {
            throw new EntityNotFoundException(PortalMessages.SomeSpecializationTemplatesNotFound);
        }

        List<InternshipSpecialization> specializationsToAdd = [];
        List<InternshipSpecializationHardSkill> specializationHardSkillsToAdd = [];
        List<(int specId, List<int> hardSkillId)> specializationHardSkillsToDelete = [];

        foreach (var spec in internsipSpecializations)
        {
            if (specTemplateIdsToAdd.Contains(spec.SpecializationTemplateId))
            {
                (var newInternsipSpecialization, var newInternshipSpecializationHardSkills) = await ComposeSpecializationAndSkills(
                    internship.Id, spec, cancellationToken);

                specializationsToAdd.Add(newInternsipSpecialization);
                specializationHardSkillsToAdd.AddRange(newInternshipSpecializationHardSkills);
            }
            else if (specTemplateIdsToKeep.Contains(spec.SpecializationTemplateId))
            {
                (var skillsToAdd, var skillsToDelete) = await ComposeSkills(spec, cancellationToken);

                specializationHardSkillsToAdd.AddRange(skillsToAdd);
                specializationHardSkillsToDelete.Add((spec.Id!.Value, skillsToDelete));
            }
        }

        await _teamBoardDbContext.InternshipSpecializations.AddRangeAsync(specializationsToAdd, cancellationToken);
        await _teamBoardDbContext.InternshipSpecializationHardSkills.AddRangeAsync(specializationHardSkillsToAdd, cancellationToken);

        await DeleteOldSpecializationAndSkills(internship.Id, specTemplateIdsToDelete, specializationHardSkillsToDelete, cancellationToken);

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<(InternshipSpecialization spec, List<InternshipSpecializationHardSkill> skills)> ComposeSpecializationAndSkills(
        int internshipId, InternshipSpecializationRequestDto specialization, CancellationToken cancellationToken)
    {
        var requestedSkillsIds = specialization.HardSkills.Select(x => x.Id).ToList();

        var specializationHardSkillIds = await _teamBoardDbContext.SpecializationTemplateHardSkills
            .Where(x => x.SpecializationTemplateId == specialization.SpecializationTemplateId)
            .Select(x => x.HardSkillId)
            .ToListAsync(cancellationToken);

        if (!requestedSkillsIds.All(s => specializationHardSkillIds.Contains(s)))
        {
            throw new InvalidDataAppException(PortalMessages.SomeHardSkillsDontBelongToSpecializationTemplate);
        }

        var newInternsipSpecialization = new InternshipSpecialization
        {
            InternshipDetailId = internshipId,
            SpecializationTemplateId = specialization.SpecializationTemplateId,
            SeniorityLevelId = specialization.SeniorityLevelId
        };

        var newInternshipSpecializationHardSkills = specialization.HardSkills.Select(x => new InternshipSpecializationHardSkill
        {
            InternshipSpecialization = newInternsipSpecialization,
            SeniorityLevelId = x.SeniorityLevelId,
            HardSkillId = x.Id
        }).ToList();

        return (newInternsipSpecialization, newInternshipSpecializationHardSkills);
    }

    private async Task<(List<InternshipSpecializationHardSkill> skillsToAdd, List<int> skillsToDelete)> ComposeSkills(
        InternshipSpecializationRequestDto specialization, CancellationToken cancellationToken)
    {
        var requestedHardSkillsIds = specialization.HardSkills.Select(x => x.Id).ToList();

        var specializationHardSkillIds = await _teamBoardDbContext.InternshipSpecializationHardSkills
            .AsNoTracking()
            .Where(x => x.InternsipSpecializationId == specialization.Id)
            .Select(x => x.HardSkillId)
            .ToListAsync(cancellationToken);

        var hardSkillsToAdd = requestedHardSkillsIds.Except(specializationHardSkillIds).ToList();
        var hardSkillsToDelete = specializationHardSkillIds.Except(requestedHardSkillsIds).ToList();

        var newInternshipSpecializationHardSkills = specialization.HardSkills
            .Where(x => hardSkillsToAdd.Contains(x.Id))
            .Select(x => new InternshipSpecializationHardSkill
            {
                InternsipSpecializationId = specialization.Id!.Value,
                SeniorityLevelId = x.SeniorityLevelId,
                HardSkillId = x.Id
            })
            .ToList();

        return (newInternshipSpecializationHardSkills, hardSkillsToDelete);
    }

    private async Task DeleteOldSpecializationAndSkills(
        int internshipId,
        HashSet<int> specTemplateIdsToDelete,
        List<(int specId, List<int> hardSkillId)> specializationHardSkillsToDelete,
        CancellationToken cancellationToken)
    {
        var specIdsToDelete = await _teamBoardDbContext.InternshipSpecializations
            .Where(s => s.InternshipDetailId == internshipId && specTemplateIdsToDelete.Contains(s.SpecializationTemplateId))
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        await _teamBoardDbContext.InternshipSpecializationHardSkills
            .Where(hs => specIdsToDelete.Contains(hs.InternsipSpecializationId))
            .ExecuteDeleteAsync(cancellationToken);

        await _teamBoardDbContext.InternshipSpecializations
            .Where(s => specIdsToDelete.Contains(s.Id))
            .ExecuteDeleteAsync(cancellationToken);

        foreach (var specializationHardSkillToDelete in specializationHardSkillsToDelete)
        {
            await _teamBoardDbContext.InternshipSpecializationHardSkills
                .Where(hs => specializationHardSkillToDelete.specId == hs.InternsipSpecializationId)
                .Where(hs => specializationHardSkillToDelete.hardSkillId.Contains(hs.HardSkillId))
                .ExecuteDeleteAsync(cancellationToken);
        }
    }

    internal sealed record Command(
        int UserId,
        EditInternshipRequestDto InternshipInfo,
        IEnumerable<InternshipSpecializationRequestDto> InternshipSpecializations)
    : ICommand<InternshipDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotNull().NotEmpty();

            RuleFor(x => x.InternshipInfo.Specialization)
                .NotNull().NotEmpty()
                .ShortTextType();

            RuleFor(x => x.InternshipInfo.EngineerStage > 0 && x.InternshipInfo.EngineerStage <= 9999);
            RuleFor(x => x.InternshipInfo.CompletedPoints > 0 && x.InternshipInfo.EngineerStage <= 9999);
            RuleFor(x => x.InternshipInfo.ApprovedPoints > 0 && x.InternshipInfo.EngineerStage <= 9999);

            RuleFor(x => x.InternshipSpecializations)
                .Must(x => x == null || x.Count() <= 10)
                .WithMessage("The number of specializations must not exceed 10.");

            RuleForEach(x => x.InternshipSpecializations)
                .ChildRules(specialization =>
                {
                    specialization.RuleFor(x => x.HardSkills)
                        .NotEmpty()
                        .WithMessage("A specialization must have at least one hard skill.")
                        .Must(x => x.Count() <= 20)
                        .WithMessage("A specialization must not have more than 20 hard skills.");
                });
        }
    }
}
