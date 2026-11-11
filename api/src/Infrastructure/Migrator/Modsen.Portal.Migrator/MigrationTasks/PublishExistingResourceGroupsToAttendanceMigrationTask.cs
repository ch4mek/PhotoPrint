using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingResourceGroupsToAttendanceMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly AttendanceDbContext _attendanceDbContext;

    public PublishExistingResourceGroupsToAttendanceMigrationTask(
        IdentityModuleDbContext identityModuleDbContext,
        AttendanceDbContext attendanceDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _attendanceDbContext = attendanceDbContext;
    }

    public string TaskName => "PublishExistingResourceGroupsToAttendance";

    public void Execute()
    {
        var groups = _identityModuleDbContext.ResourceGroups.ToArray();
        var groupIds = new List<int>();

        foreach (var group in groups)
        {
            var attendanceResourceGroup = _attendanceDbContext.ResourceGroups
                .FirstOrDefault(x => x.ResourceGroupId == group.Id);
            if (attendanceResourceGroup == null)
            {
                attendanceResourceGroup = new ResourceGroupEntity { ResourceGroupId = group.Id, Name = group.Name, };
                _attendanceDbContext.Add(attendanceResourceGroup);
            }
            else
            {
                attendanceResourceGroup.Name = group.Name;
                _attendanceDbContext.Update(attendanceResourceGroup);
            }

            groupIds.Add(group.Id);
        }

        _attendanceDbContext.ResourceGroups
            .Where(g => !groupIds.Contains(g.ResourceGroupId))
            .ExecuteDelete();
        _attendanceDbContext.SaveChanges();
    }
}
