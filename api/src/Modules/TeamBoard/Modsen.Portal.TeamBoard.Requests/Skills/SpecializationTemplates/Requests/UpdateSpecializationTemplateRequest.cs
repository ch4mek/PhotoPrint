using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;

internal sealed class UpdateSpecializationTemplateRequest
    : ICommandHandler<UpdateSpecializationTemplateRequest.Command, SpecializationTemplateWithDepartmentsAndHardSkillsDto>
{
    private readonly ISpecializationNotAssignedToAnyUserBusinessRule _specializationNotAssignedToAnyUserBusinessRule;
    private readonly IDeleteSpecializationTemplateHardSkillsDbQuery _deleteSpecializationTemplateHardSkillsDbQuery;
    private readonly IGetSpecializationTemplateDepartmentsDbQuery _getSpecializationTemplateDepartmentsDbQuery;
    private readonly IUpdateSpecializationTemplateValidator _specializationTemplateValidator;
    private readonly ISpecializationTemplateDepartmentsComposer _templateDepartmentsComposer;
    private readonly ISpecializationTemplateHardSkillsComposer _templateHardSkillsComposer;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamboardDbContext;
    private readonly IClockService _clockService;

    public UpdateSpecializationTemplateRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        IGetSpecializationTemplateDepartmentsDbQuery getSpecializationTemplateDepartmentsDbQuery,
        IDeleteSpecializationTemplateHardSkillsDbQuery deleteSpecializationTemplateHardSkillsDbQuery,
        ISpecializationNotAssignedToAnyUserBusinessRule specializationNotAssignedToAnyUserBusinessRule,
        ITeamBoardDbContext teamboardDbContext,
        IClockService clockService,
        IUpdateSpecializationTemplateValidator specializationTemplateValidator,
        ISpecializationTemplateDepartmentsComposer templateDepartmentsComposer,
        ISpecializationTemplateHardSkillsComposer templateHardSkillsComposer)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _getSpecializationTemplateDepartmentsDbQuery = getSpecializationTemplateDepartmentsDbQuery;
        _deleteSpecializationTemplateHardSkillsDbQuery = deleteSpecializationTemplateHardSkillsDbQuery;
        _specializationNotAssignedToAnyUserBusinessRule = specializationNotAssignedToAnyUserBusinessRule;
        _teamboardDbContext = teamboardDbContext;
        _clockService = clockService;
        _specializationTemplateValidator = specializationTemplateValidator;
        _templateDepartmentsComposer = templateDepartmentsComposer;
        _templateHardSkillsComposer = templateHardSkillsComposer;
    }

    public async Task<SpecializationTemplateWithDepartmentsAndHardSkillsDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var updatedName = await UpdateTemplateNameAsync(request.RequestDto, cancellationToken);

        var updatedSpecializationTemplateDepartmentIds = await UpdateTemplateDepartmentsAsync(
            request.RequestDto,
            cancellationToken);

        var updatedSpecializationTemplateHardSkills = await UpdateSpecializationTemplateHardSkillsAsync(
            request.RequestDto,
            cancellationToken);

        return new SpecializationTemplateWithDepartmentsAndHardSkillsDto
        {
            Id = request.RequestDto.Id,
            Name = updatedName,
            DepartmentIds = updatedSpecializationTemplateDepartmentIds,
            HardSkills = updatedSpecializationTemplateHardSkills
        };
    }

    private async Task<string> UpdateTemplateNameAsync(
        UpdateSpecializationTemplateDto request,
        CancellationToken cancellationToken)
    {
        await _specializationTemplateValidator.ValidateAsync(request, cancellationToken);

        var templateToUpdate = await _teamboardDbContext.SpecializationTemplates.FindAsync([request.Id],
            cancellationToken);

        if (templateToUpdate!.Name == request.Name)
        {
            return templateToUpdate.Name;
        }

        templateToUpdate.Name = request.Name;
        templateToUpdate.UpdatedAtUtc = _clockService.UtcNow;
        templateToUpdate.UpdatedById = _authenticatedUserContext.GetRequiredUserId();

        await _teamboardDbContext.SaveChangesAsync(cancellationToken);

        return templateToUpdate.Name;
    }

    private async Task<string[]> UpdateTemplateDepartmentsAsync(
        UpdateSpecializationTemplateDto request,
        CancellationToken cancellationToken)
    {
        var existingDepartments = await _getSpecializationTemplateDepartmentsDbQuery
            .ExecuteAsync(request.Id, cancellationToken);

        var newDepartmentsIds = request.DepartmentIds
            .Except(existingDepartments.Select(department => department.DepartmentId))
            .ToArray();

        var departmentsIdsToDelete = existingDepartments
            .ExceptBy(request.DepartmentIds, department => department.DepartmentId)
            .Select(department => department.DepartmentId)
            .ToArray();

        var finalNewDepartmentsIds = existingDepartments
            .Select(department => department.DepartmentId)
            .ToList();

        if (newDepartmentsIds.Length != 0)
        {
            var templateDepartmentsIds = await _templateDepartmentsComposer.ComposeAsync(request.Id,
                newDepartmentsIds, cancellationToken);

            finalNewDepartmentsIds.AddRange(templateDepartmentsIds);
        }

        if (departmentsIdsToDelete.Length != 0)
        {
            await _teamboardDbContext.SpecializationTemplateDepartments
                .Where(template => template.SpecializationTemplateId == request.Id &&
                    departmentsIdsToDelete.Contains(template.DepartmentId))
                .ExecuteDeleteAsync(cancellationToken);

            await _teamboardDbContext.SaveChangesAsync(cancellationToken);

            finalNewDepartmentsIds = finalNewDepartmentsIds
                .Where(departmentId => !departmentsIdsToDelete.Contains(departmentId))
                .ToList();
        }

        return finalNewDepartmentsIds.ToArray();
    }

    private async Task<List<SpecializationTemplateHardSkillDto>> UpdateSpecializationTemplateHardSkillsAsync(
        UpdateSpecializationTemplateDto request, CancellationToken cancellationToken)
    {
        var existingHardSkills = await _teamboardDbContext.SpecializationTemplateHardSkills
            .AsNoTracking()
            .Where(templateHardSkill => templateHardSkill.SpecializationTemplateId == request.Id)
            .Include(templateHardSkill => templateHardSkill.HardSkill)
            .ToArrayAsync(cancellationToken);

        var hardSkillNames = request.HardSkills!.Select(skillDto => skillDto.Name).ToArray();

        var newHardSkills = hardSkillNames
            .Except(existingHardSkills.Select(hardSkill => hardSkill.HardSkill!.Name))
            .ToArray();

        var hardSkillIdsToDelete = existingHardSkills
            .ExceptBy(hardSkillNames, hardSkill => hardSkill.HardSkill!.Name)
            .Select(hardSkill => hardSkill.HardSkillId)
            .ToArray();

        var deletingEqualsExisting = hardSkillIdsToDelete.SequenceEqual(
            existingHardSkills
                .Select(templateHardSkill => templateHardSkill.HardSkillId)
                .OrderBy(id => id)
            );

        if (deletingEqualsExisting && newHardSkills.Length == 0)
        {
            var isSpecializationAssignedToAnyUser = await _specializationNotAssignedToAnyUserBusinessRule
                .IsCorrespondsToBusiness(request.Id, cancellationToken);

            if (isSpecializationAssignedToAnyUser)
            {
                throw new EntityConflictException(
                    "You cannot leave a specialization that is already assigned to users without hard skills");
            }
        }

        var finalNewHardSkills = existingHardSkills
            .Select(SpecializationTemplateHardSkillDto.Of)
            .ToList();

        if (newHardSkills.Length != 0)
        {
            var createdHardSkills = await _templateHardSkillsComposer.ComposeAsync(request.Id, newHardSkills,
                cancellationToken);

            finalNewHardSkills.AddRange(createdHardSkills);
        }

        if (hardSkillIdsToDelete.Length != 0)
        {
            await _deleteSpecializationTemplateHardSkillsDbQuery.ExecuteAsync(
                request.Id,
                hardSkillIdsToDelete,
                cancellationToken);

            finalNewHardSkills = finalNewHardSkills
                .Where(templateHardSkillDto => !hardSkillIdsToDelete.Contains(templateHardSkillDto.Id))
                .ToList();
        }

        return finalNewHardSkills;
    }

    internal sealed record Command(UpdateSpecializationTemplateDto RequestDto)
        : ICommand<SpecializationTemplateWithDepartmentsAndHardSkillsDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RequestDto).SetValidator(new UpdateSpecializationTemplateDto.Validator());
        }
    }
}
