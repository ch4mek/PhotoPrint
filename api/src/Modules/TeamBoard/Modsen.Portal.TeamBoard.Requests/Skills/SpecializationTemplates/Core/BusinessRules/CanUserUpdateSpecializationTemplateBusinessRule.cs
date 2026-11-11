using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;

internal interface ICanUserUpdateSpecializationTemplateBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int? createdById, int specializationId, CancellationToken cancellationToken);
}

internal class CanUserUpdateSpecializationTemplateBusinessRule : ICanUserUpdateSpecializationTemplateBusinessRule
{
    private readonly IGetSpecializationTemplateDepartmentsDbQuery _getSpecializationTemplateDepartmentsDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public CanUserUpdateSpecializationTemplateBusinessRule(
        IAuthenticatedUserContext authenticatedUserContext,
        IGetSpecializationTemplateDepartmentsDbQuery getSpecializationTemplateDepartmentsDbQuery)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _getSpecializationTemplateDepartmentsDbQuery = getSpecializationTemplateDepartmentsDbQuery;
    }

    public async Task<bool> IsCorrespondsToBusiness(int? createdById, int specializationId,
        CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.GetRoles().IsCeo() || _authenticatedUserContext.GetRoles().IsCto())
        {
            return true;
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() ||
            _authenticatedUserContext.GetRoles().IsGroupManager())
        {
            if (createdById == _authenticatedUserContext.UserId)
            {
                return true;
            }

            var departments = await _getSpecializationTemplateDepartmentsDbQuery
                .ExecuteAsync(specializationId, cancellationToken);
            var departmentIds = departments.Select(department => department.DepartmentId);

            if (departmentIds.Contains(_authenticatedUserContext.DepartmentId))
            {
                return true;
            }
        }

        return false;
    }
}
