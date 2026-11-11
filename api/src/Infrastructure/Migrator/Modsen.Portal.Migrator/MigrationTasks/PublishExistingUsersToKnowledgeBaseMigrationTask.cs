using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingUsersToKnowledgeBaseMigrationTask : IMigrationTask
{
    private readonly KnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IdentityModuleDbContext _identityModuleDbContext;

    public PublishExistingUsersToKnowledgeBaseMigrationTask(
        KnowledgeBaseDbContext knowledgeBaseDbContext,
        IdentityModuleDbContext identityModuleDbContext)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _identityModuleDbContext = identityModuleDbContext;
    }

    public string TaskName => "PublishExistingUsersToKnowledgeBase";

    public void Execute()
    {
        var page = 0;
        var size = MigratorTaskConstants.BatchSize;
        bool hasAnyUsers;

        var knowledgeBaseUserIds = new List<int>();

        do
        {
            var users = _identityModuleDbContext.Users
                .AsNoTracking()
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Take(size)
                .ToList();

            var knowledgeBaseUsers = new List<KnowledgeBaseUserEntity>();

            page++;

            foreach (var user in users)
            {
                var knowledgeBaseUser = _knowledgeBaseDbContext.KnowledgeBaseUsers.FirstOrDefault(x => x.UserId == user.Id);
                if (knowledgeBaseUser == null)
                {
                    knowledgeBaseUser = new KnowledgeBaseUserEntity { UserId = user.Id, FirstName = "test", LastName = "test", Email = "test"};

                    CopyFields(knowledgeBaseUser, user);
                    _knowledgeBaseDbContext.Add(knowledgeBaseUser);
                }
                else
                {
                    CopyFields(knowledgeBaseUser, user);
                }

                knowledgeBaseUsers.Add(knowledgeBaseUser);
                knowledgeBaseUserIds.Add(knowledgeBaseUser.UserId);
            }

            _knowledgeBaseDbContext.SaveChanges();

            foreach (var knowledgeBaseUser in knowledgeBaseUsers)
            {
                _knowledgeBaseDbContext.KnowledgeBaseUsers.Entry(knowledgeBaseUser).State = EntityState.Detached;
            }

            hasAnyUsers = _identityModuleDbContext.Users
                .Where(x => !x.UserRoles.Any(y => y.Role!.Name == PortalRoleNames.Intern))
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Any();
        } while (hasAnyUsers);

        _knowledgeBaseDbContext.KnowledgeBaseUsers
            .Where(knowledgeBaseUser => !knowledgeBaseUserIds.Contains(knowledgeBaseUser.UserId) && knowledgeBaseUser.Email != "ModsenCompany@modsen-software.com")
            .ExecuteDelete();

        _knowledgeBaseDbContext.SaveChanges();
    }

    private static void CopyFields(KnowledgeBaseUserEntity knowledgeBaseUser, User user)
    {
        knowledgeBaseUser.Email = user.Email!;
        knowledgeBaseUser.LastName = user.LastName;
        knowledgeBaseUser.FirstName = user.FirstName;
        knowledgeBaseUser.DepartmentId = user.DepartmentId;
    }
}
