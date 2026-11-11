using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingUsersToEquipmentMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly EquipmentDbContext _equipmentDbContext;

    public PublishExistingUsersToEquipmentMigrationTask(
        IdentityModuleDbContext identityModuleDbContext,
        EquipmentDbContext equipmentDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _equipmentDbContext = equipmentDbContext;
    }

    public string TaskName => "PublishExistingUsersToEquipment";

    public void Execute()
    {
        var page = 0;
        bool hasAnyUsers;

        var equipmentUserIds = new List<int>();

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

            var equipmentUsers = new List<EquipmentUser>();

            page++;

            foreach (var user in users)
            {
                var equipmentUser = _equipmentDbContext.EquipmentUsers.FirstOrDefault(x => x.UserId == user.Id);
                if (equipmentUser == null)
                {
                    equipmentUser = new EquipmentUser { UserId = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email!
                    };

                    CopyFields(equipmentUser, user);
                    _equipmentDbContext.Add(equipmentUser);
                }
                else
                {
                    CopyFields(equipmentUser, user);
                }

                equipmentUsers.Add(equipmentUser);
                equipmentUserIds.Add(equipmentUser.UserId);
            }

            _equipmentDbContext.SaveChanges();

            foreach (var equipmentUser in equipmentUsers)
            {
                _equipmentDbContext.EquipmentUsers.Entry(equipmentUser).State = EntityState.Detached;
            }

            hasAnyUsers = _identityModuleDbContext.Users
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * MigratorTaskConstants.BatchSize)
                .Any();
        } while (hasAnyUsers);

        _equipmentDbContext.EquipmentUsers
            .Where(equipmentUser => !equipmentUserIds.Contains(equipmentUser.UserId) && equipmentUser.Email != "ModsenCompany@modsen-software.com")
            .ExecuteUpdate(setPropertyCalls => setPropertyCalls.SetProperty(
                equipmentUser => equipmentUser.IsActive, false));

        _equipmentDbContext.SaveChanges();
    }

    private static void CopyFields(EquipmentUser equipmentUser, User user)
    {
        equipmentUser.Email = user.Email!;
        equipmentUser.LastName = user.LastName;
        equipmentUser.FirstName = user.FirstName;
        equipmentUser.DepartmentId = user.DepartmentId;
        equipmentUser.IsActive = user.IsActive;
        equipmentUser.ResourceGroupId = user.ResourceGroupId;

        var role = user.UserRoles.Select(x => x.Role?.Name).FirstOrDefault();
        equipmentUser.Position = role;
    }
}
