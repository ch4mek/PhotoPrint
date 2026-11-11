using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal class PublishExistingUsersToAttendanceMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly AttendanceDbContext _attendanceDbContext;

    public PublishExistingUsersToAttendanceMigrationTask(AttendanceDbContext attendanceDbContext,
        IdentityModuleDbContext identityModuleDbContext)
    {
        _attendanceDbContext = attendanceDbContext;
        _identityModuleDbContext = identityModuleDbContext;
    }

    public string TaskName => "PublishExistingUsersToAttendance";

    public void Execute()
    {
        var page = 0;
        bool hasAnyUsers;

        var attendanceUsersIds = new List<int>();

        do
        {
            var users = _identityModuleDbContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .AsNoTracking()
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Take(MigratorTaskConstants.BatchSize)
                .ToList();

            var attendanceUsers = new List<Attendance.DataAccess.Npgsql.Entities.UserEntity>();

            page++;

            foreach (var user in users)
            {
                var attendanceUser = _attendanceDbContext.Users.FirstOrDefault(x => x.UserId == user.Id);
                if (attendanceUser == null)
                {
                    attendanceUser = new Attendance.DataAccess.Npgsql.Entities.UserEntity
                    {
                        UserId = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email!
                    };

                    CopyFields(attendanceUser, user);
                    _attendanceDbContext.Add(attendanceUser);
                }
                else
                {
                    CopyFields(attendanceUser, user);
                }

                attendanceUsers.Add(attendanceUser);
                attendanceUsersIds.Add(attendanceUser.UserId);
            }

            _attendanceDbContext.SaveChanges();

            foreach (var attendanceUser in attendanceUsers)
            {
                _attendanceDbContext.Users.Entry(attendanceUser).State = EntityState.Detached;
            }

            hasAnyUsers = _identityModuleDbContext.Users
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Any();
        } while (hasAnyUsers);

        _attendanceDbContext.Users
            .Where(attendanceUser =>
                !attendanceUsersIds.Contains(attendanceUser.UserId) &&
                attendanceUser.Email != "ModsenCompany@modsen-software.com")
            .ExecuteUpdate(setPropertyCalls => setPropertyCalls
                .SetProperty(attendanceUser => attendanceUser.IsActive, false));

        _attendanceDbContext.SaveChanges();
    }

    private static void CopyFields(Attendance.DataAccess.Npgsql.Entities.UserEntity attendanceUser, User user)
    {
        attendanceUser.Email = user.Email!;
        attendanceUser.FirstName = user.FirstName;
        attendanceUser.LastName = user.LastName;
        attendanceUser.FirstNameRu = user.FirstNameRu;
        attendanceUser.LastNameRu = user.LastNameRu;
        attendanceUser.PatronymicRu = user.PatronymicRu;
        attendanceUser.DepartmentId = user.DepartmentId;
        attendanceUser.IsActive = user.IsActive;
        attendanceUser.ResourceGroupId = user.ResourceGroupId;
        attendanceUser.Position = user.UserRoles.Select(x => x.Role?.Name).FirstOrDefault();
    }
}
