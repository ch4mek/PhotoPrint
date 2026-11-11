using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingUsersToInquiriesMigrationTask : IMigrationTask
{
    private readonly InquiriesDbContext _inquiriesDbContext;
    private readonly IdentityModuleDbContext _identityModuleDbContext;

    public PublishExistingUsersToInquiriesMigrationTask(
        InquiriesDbContext inquiriesDbContext,
        IdentityModuleDbContext identityModuleDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _identityModuleDbContext = identityModuleDbContext;
    }

    public string TaskName => "PublishExistingUsersToInquiries";

    public void Execute()
    {
        var page = 0;
        var size = MigratorTaskConstants.BatchSize;
        bool hasAnyUsers;

        var inquiriesUserIds = new List<int>();

        do
        {
            var users = _identityModuleDbContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .AsNoTracking()
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Take(size)
                .ToList();

            var inquiriesUsers = new List<InquiriesUserEntity>();

            page++;

            foreach (var user in users)
            {
                var inquiriesUser = _inquiriesDbContext.InquiriesUsers.FirstOrDefault(x => x.UserId == user.Id);
                if (inquiriesUser == null)
                {
                    inquiriesUser = new InquiriesUserEntity { UserId = user.Id,
                        Email = "test",
                        FirstName = "test",
                        LastName = "test",
                    };

                    CopyFields(inquiriesUser, user);
                    _inquiriesDbContext.Add(inquiriesUser);
                }
                else
                {
                    CopyFields(inquiriesUser, user);
                }

                inquiriesUsers.Add(inquiriesUser);
                inquiriesUserIds.Add(inquiriesUser.UserId);
            }

            _inquiriesDbContext.SaveChanges();

            foreach (var inquiriesUser in inquiriesUsers)
            {
                _inquiriesDbContext.InquiriesUsers.Entry(inquiriesUser).State = EntityState.Detached;
            }

            hasAnyUsers = _identityModuleDbContext.Users
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Any();
        } while (hasAnyUsers);

        _inquiriesDbContext.InquiriesUsers
            .Where(inquiriesUser => !inquiriesUserIds.Contains(inquiriesUser.UserId) && inquiriesUser.Email != "ModsenCompany@modsen-software.com")
            .ExecuteDelete();

        _inquiriesDbContext.SaveChanges();
    }

    private static void CopyFields(InquiriesUserEntity inquiriesUser, User user)
    {
        inquiriesUser.Email = user.Email!;
        inquiriesUser.LastName = user.LastName;
        inquiriesUser.FirstName = user.FirstName;
        inquiriesUser.DepartmentId = user.DepartmentId;
        inquiriesUser.RegisteredLocationId = user.RegisteredLocationId;
        inquiriesUser.AvatarFileId = user.AvatarFileId;
        inquiriesUser.ResourceGroupId = user.ResourceGroupId;
        inquiriesUser.IsActive = user.IsActive;
        inquiriesUser.Color = user.Color;
        inquiriesUser.HireDate = user.HireDate;

        inquiriesUser.Position = user.UserRoles.Select(x => x.Role!.Name).FirstOrDefault();
        inquiriesUser.PositionOrderWeight = user.UserRoles.Select(x => x.Role!.OrderWeight).FirstOrDefault();
    }
}
