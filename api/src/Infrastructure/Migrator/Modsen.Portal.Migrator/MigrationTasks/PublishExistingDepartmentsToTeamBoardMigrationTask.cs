using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingDepartmentsToTeamBoardMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly TeamBoardDbContext _teamBoardDbContext;

    public PublishExistingDepartmentsToTeamBoardMigrationTask(
        IdentityModuleDbContext identityModuleDbContext,
        TeamBoardDbContext teamBoardDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public string TaskName => "PublishExistingDepartmentsToTeamBoard";

    public void Execute()
    {
        var departments = _identityModuleDbContext.Departments
            .Include(x => x.DepartmentReportingToRole)
            .ToArray();
        var departmentIds = new List<string>();

        foreach (var department in departments)
        {
            var teamboardDepartment = _teamBoardDbContext.TeamBoardDepartments
                .FirstOrDefault(x => x.DepartmentId == department.Id);
            if (teamboardDepartment == null)
            {
                teamboardDepartment = new TeamboardDepartment
                {
                    DepartmentId = department.Id,
                    FullName = department.FullName,
                    ShortName = department.ShortName,
                    DepartmentReportingToRoleName = department.DepartmentReportingToRole!.Name!,
                };
                CopyFields(department, teamboardDepartment);
                _teamBoardDbContext.Add(teamboardDepartment);
            }
            else
            {
                CopyFields(department, teamboardDepartment);
                _teamBoardDbContext.Update(teamboardDepartment);
            }

            departmentIds.Add(department.Id);
        }

        _teamBoardDbContext.TeamBoardDepartments
            .Where(dep => !departmentIds.Contains(dep.DepartmentId))
            .ExecuteDelete();
        _teamBoardDbContext.SaveChanges();
    }

    private static void CopyFields(Department department, TeamboardDepartment teamboardDepartment)
    {
        teamboardDepartment.ShortName = department.ShortName;
        teamboardDepartment.FullName = department.FullName;
        teamboardDepartment.DepartmentOrderWeight = department.OrderWeight;
        teamboardDepartment.DepartmentReportingToRoleName = department.DepartmentReportingToRole!.Name;
    }
}
