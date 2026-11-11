using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;
using SpecializationTemplate = Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities.SpecializationTemplate;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;

internal sealed class CreateSpecializationTemplateRequest
    : ICommandHandler<CreateSpecializationTemplateRequest.Command, SpecializationTemplateDto>
{
    private readonly ICanUserUpdateSpecializationTemplateBusinessRule _canUserUpdateTemplateBusinessRule;
    private readonly IIsSpecializationTemplateNameUniqueBusinessRule _isTemplateExistsUniqueBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamboardDbContext;
    private readonly IClockService _clockService;

    public CreateSpecializationTemplateRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ICanUserUpdateSpecializationTemplateBusinessRule canUserUpdateTemplateBusinessRule,
        IIsSpecializationTemplateNameUniqueBusinessRule isTemplateNameUniqueBusinessRule,
        ITeamBoardDbContext teamboardDbContext,
        IClockService clockService)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _canUserUpdateTemplateBusinessRule = canUserUpdateTemplateBusinessRule;
        _isTemplateExistsUniqueBusinessRule = isTemplateNameUniqueBusinessRule;
        _teamboardDbContext = teamboardDbContext;
        _clockService = clockService;
    }

    public async Task<SpecializationTemplateDto> Handle(Command command, CancellationToken cancellationToken)
    {
        var request = command.RequestDto;

        await CheckIfTemplateCanBeCreated(request, cancellationToken);

        var areHardSkillsListed = request.HardSkillIds is not null && request.HardSkillIds.Count != 0;
        if (areHardSkillsListed)
        {
            var actualIds = await _teamboardDbContext.HardSkills.Select(x => x.Id).ToListAsync(cancellationToken);
            var areHardSkillsExist = request.HardSkillIds!.All(actualIds.Contains);

            if (!areHardSkillsExist)
            {
                throw new EntityNotFoundException(PortalMessages.SomeHardSkillsNotFound);
            }
        }

        var createdTemplateId = await CreateTemplateAsync(request, cancellationToken);

        var canBeUpdated = await _canUserUpdateTemplateBusinessRule.IsCorrespondsToBusiness(
            createdTemplateId,
            _authenticatedUserContext.GetRequiredUserId(),
            cancellationToken);

        return new SpecializationTemplateDto
        {
            Id = createdTemplateId,
            Name = request.Name,
            CanBeUpdated = canBeUpdated
        };
    }

    private async Task CheckIfTemplateCanBeCreated(CreateSpecializationTemplateDto request,
        CancellationToken cancellationToken)
    {
        var roles = _authenticatedUserContext.GetRoles();
        var isFromAnotherDepartment = (roles.IsHeadOfDepartment() || roles.IsGroupManager()) &&
            request.DepartmentIds != null &&
            !request.DepartmentIds.Contains(_authenticatedUserContext.DepartmentId);

        if (isFromAnotherDepartment)
        {
            throw new ActionNotAllowedException("You can not create specialization with this department");
        }

        var isTemplateNameUnique = await _isTemplateExistsUniqueBusinessRule.IsCorrespondsToBusiness(
            name: request.Name,
            id: null,
            cancellationToken);

        if (!isTemplateNameUnique)
        {
            throw new EntityConflictException("Specialization with the same name already exists");
        }
    }

    private async Task<int> CreateTemplateAsync(CreateSpecializationTemplateDto request,
        CancellationToken cancellationToken)
    {
        var specializationTemplateHardSkills = new Lazy<List<SpecializationTemplateHardSkill>>();
        var specializationTemplateDepartments = new Lazy<List<SpecializationTemplateDepartment>>();

        var areHardSkillsListed = request.HardSkillIds is not null && request.HardSkillIds.Count != 0;
        if (areHardSkillsListed)
        {
            var hardSkillIds = await _teamboardDbContext.HardSkills
                .Where(hardSkill => hardSkill.IsActive)
                .Where(hardSkill => request.HardSkillIds!.Contains(hardSkill.Id))
                .ToListAsync(cancellationToken);

            specializationTemplateHardSkills.Value.AddRange(
                hardSkillIds.ConvertAll(hardSkillId => new SpecializationTemplateHardSkill
                {
                    CreatedAtUtc = _clockService.UtcNow,
                    CreatedById = _authenticatedUserContext.UserId,
                    HardSkillId = hardSkillId.Id
                }));
        }

        var areDepartmentsListed = request.DepartmentIds is not null && request.DepartmentIds.Count != 0;
        if (areDepartmentsListed)
        {
            specializationTemplateDepartments.Value.AddRange(
                (request.DepartmentIds ?? []).ToList().ConvertAll(department => new SpecializationTemplateDepartment
                {
                    CreatedAtUtc = _clockService.UtcNow,
                    CreatedById = _authenticatedUserContext.UserId,
                    DepartmentId = department
                }));
        }

        var specializationTemplate = new SpecializationTemplate
        {
            Name = request.Name,
            IsActive = true,
            CreatedAtUtc = _clockService.UtcNow,
            CreatedById = _authenticatedUserContext.UserId,
            SpecializationTemplateDepartments = specializationTemplateDepartments.IsValueCreated
                ? specializationTemplateDepartments.Value
                : [],
            SpecializationTemplateHardSkills = specializationTemplateHardSkills.IsValueCreated
                ? specializationTemplateHardSkills.Value
                : []
        };

        await _teamboardDbContext.SpecializationTemplates.AddAsync(specializationTemplate, cancellationToken);
        await _teamboardDbContext.SaveChangesAsync(cancellationToken);

        return specializationTemplate.Id;
    }

    internal sealed record Command(CreateSpecializationTemplateDto RequestDto)
        : ICommand<SpecializationTemplateDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RequestDto).SetValidator(new CreateSpecializationTemplateDto.Validator());
        }
    }

}
