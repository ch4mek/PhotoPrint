using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;

internal interface IIsSpecializationNotAssignedToAnyUserBusinessRule
{
    Task<bool> IsCorrespondsToBusiness(int templateId, string[] notUpdatedDepartmentIds,
        CancellationToken cancellationToken);
}

internal sealed class IsSpecializationNotAssignedToAnyUserBusinessRule
    : IIsSpecializationNotAssignedToAnyUserBusinessRule
{
    private readonly IDepartmentsSpecializationAssignmentsDbQuery _departmentsSpecializationAssignmentsDbQuery;

    public IsSpecializationNotAssignedToAnyUserBusinessRule(
        IDepartmentsSpecializationAssignmentsDbQuery departmentsSpecializationAssignmentsDbQuery)
    {
        _departmentsSpecializationAssignmentsDbQuery = departmentsSpecializationAssignmentsDbQuery;
    }

    public async Task<bool> IsCorrespondsToBusiness(int templateId, string[] notUpdatedDepartmentIds,
        CancellationToken cancellationToken)
    {
        if (notUpdatedDepartmentIds.Length == 0)
        {
            return true;
        }

        var isSpecializationAssignedToAnyUserFromDepartments =
            (await _departmentsSpecializationAssignmentsDbQuery.ExecuteAsync(templateId,
                notUpdatedDepartmentIds,
                cancellationToken))
            .Any(department => department.IsUserFromDepartmentAssigned);

        if (isSpecializationAssignedToAnyUserFromDepartments)
        {
            return false;
        }

        return true;
    }
}
