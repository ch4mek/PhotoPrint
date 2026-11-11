using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;

internal interface IDepartmentsSpecializationAssignmentsDbQuery
{
    Task<DepartmentSpecializationAssignment[]> ExecuteAsync(int specializationId, string[] departmentIds,
        CancellationToken cancellationToken);
}

internal record DepartmentSpecializationAssignment(string DepartmentId, bool IsUserFromDepartmentAssigned);

internal sealed class DepartmentsSpecializationAssignmentsDbQuery : IDepartmentsSpecializationAssignmentsDbQuery
{
    private readonly ITeamBoardDbContext _dbContext;

    public DepartmentsSpecializationAssignmentsDbQuery(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DepartmentSpecializationAssignment[]> ExecuteAsync(int specializationId,
        string[] departmentIds, CancellationToken cancellationToken)
    {
        var actualDepartments = await _dbContext.UserSpecializations
            .Include(specialization => specialization.User)
            .Where(userSpecialization => userSpecialization.User!.IsActive &&
                userSpecialization.SpecializationTemplateId == specializationId)
            .Select(userSpecialization => userSpecialization.User!.DepartmentId)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        return departmentIds.Select(department => new DepartmentSpecializationAssignment(
                department,
                IsUserFromDepartmentAssigned: actualDepartments.Contains(department)))
            .ToArray();
    }
}
