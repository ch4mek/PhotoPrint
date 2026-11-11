using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.Validators;

internal interface IUpdateSpecializationTemplateValidator
{
    Task ValidateAsync(UpdateSpecializationTemplateDto request, CancellationToken cancellationToken);
}

internal sealed class UpdateSpecializationTemplateValidator : IUpdateSpecializationTemplateValidator
{
    private readonly IIsSpecializationNotAssignedToAnyUserBusinessRule _isSpecializationNotAssignedToAnyUserBusinessRule;
    private readonly ICanUserUpdateSpecializationTemplateBusinessRule _canUserUpdateSpecializationTemplateBusinessRule;
    private readonly IIsSpecializationTemplateNameUniqueBusinessRule _isTemplateNameUniqueBusinessRule;
    private readonly IGetSpecializationTemplateByIdDbQuery _getSpecializationTemplateByIdDbQuery;

    public UpdateSpecializationTemplateValidator(
        IIsSpecializationNotAssignedToAnyUserBusinessRule isSpecializationNotAssignedToAnyUserBusinessRule,
        ICanUserUpdateSpecializationTemplateBusinessRule canUserUpdateSpecializationTemplateBusinessRule,
        IIsSpecializationTemplateNameUniqueBusinessRule isTemplateNameUniqueBusinessRule,
        IGetSpecializationTemplateByIdDbQuery getSpecializationTemplateByIdDbQuery)
    {
        _isSpecializationNotAssignedToAnyUserBusinessRule = isSpecializationNotAssignedToAnyUserBusinessRule;
        _canUserUpdateSpecializationTemplateBusinessRule = canUserUpdateSpecializationTemplateBusinessRule;
        _isTemplateNameUniqueBusinessRule = isTemplateNameUniqueBusinessRule;
        _getSpecializationTemplateByIdDbQuery = getSpecializationTemplateByIdDbQuery;
    }

    public async Task ValidateAsync(UpdateSpecializationTemplateDto request, CancellationToken cancellationToken)
    {
        var specializationTemplate = await _getSpecializationTemplateByIdDbQuery.ExecuteAsync(request.Id,
            cancellationToken)
            ?? throw new EntityNotFoundException($"Specialization by id {request.Id} is not found");

        var isTemplateNameUnique = await _isTemplateNameUniqueBusinessRule.IsCorrespondsToBusiness(request.Name,
            request.Id,
            cancellationToken);

        if (!isTemplateNameUnique)
        {
            throw new InvalidDataAppException("Specialization with the same name already exists");
        }

        var canUserUpdateTemplate = await _canUserUpdateSpecializationTemplateBusinessRule.IsCorrespondsToBusiness(
            specializationTemplate.CreatedById,
            request.Id,
            cancellationToken);

        if (!canUserUpdateTemplate)
        {
            throw new ActionNotAllowedException();
        }

        var notUpdatedActualDepartments = specializationTemplate.SpecializationTemplateDepartments
            .Select(department => department.DepartmentId)
            .Except(request.DepartmentIds)
            .ToArray();

        var isTemplateNotAssignedToAnyUserFromDepartments = await _isSpecializationNotAssignedToAnyUserBusinessRule
                .IsCorrespondsToBusiness(request.Id, notUpdatedActualDepartments, cancellationToken);
        if (!isTemplateNotAssignedToAnyUserFromDepartments)
        {
            throw new EntityConflictException();
        }
    }
}
