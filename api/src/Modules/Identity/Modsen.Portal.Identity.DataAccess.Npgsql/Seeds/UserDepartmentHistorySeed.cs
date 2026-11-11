using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Seeds;

internal class UserDepartmentHistorySeed : ISeedEntitiesProvider<IdentityModuleDbContext>
{
    private readonly IdentityModuleDbContext _dbContext;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;
    public UserDepartmentHistorySeed(IdentityModuleDbContext dbContext, IHostEnvironmentContext hostEnvironmentContext)
    {
        _dbContext = dbContext;
        _hostEnvironmentContext = hostEnvironmentContext;
    }

    public void Seed()
    {
        if (_hostEnvironmentContext.IsProductionEnvironment())
        {
            return;
        }

        if (_hostEnvironmentContext.IsFunctionalTestEnvironment() ||
            _hostEnvironmentContext.IsIntegrationTestEnvironment() ||
            _hostEnvironmentContext.IsDevelopmentEnvironment() ||
            _hostEnvironmentContext.IsQaEnvironment() ||
            _hostEnvironmentContext.IsStagingEnvironment())
        {
            var assignsByUser = GetUserDepartmentAssigns().GroupBy(x => x.UserId);

            foreach (var userGroup in assignsByUser)
            {
                ProcessUser(userGroup.Key, userGroup.OrderBy(x => x.NewDepartmentAssign).ToList());
            }

            _dbContext.SaveChanges();
        }
    }

    private void ProcessUser(int userId, List<UserDepartmentAssign> assigns)
    {
        var histories = _dbContext.Set<UserDepartmentHistory>()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.DateFrom)
            .ToList();

        var user = _dbContext.Users.First(x => x.Id == userId);

        if (histories.Count == 0)
        {
            var initial = new UserDepartmentHistory
            {
                UserId = userId,
                DepartmentId = user.DepartmentId!,
                DateFrom = new DateOnly(2000, 1, 1),
                DateTo = null
            };

            _dbContext.Set<UserDepartmentHistory>().Add(initial);
            histories.Add(initial);
        }

        var current = histories
            .Where(x => x.DateTo == null)
            .OrderBy(x => x.DateFrom)
            .Last();

        foreach (var assign in assigns)
        {
            if (histories.Any(x =>
                    x.DepartmentId == assign.DepartmentId &&
                    x.DateFrom == assign.NewDepartmentAssign))
            {
                continue;
            }

            if (current.DepartmentId == assign.DepartmentId)
            {
                continue;
            }

            current.DateTo = assign.NewDepartmentAssign.AddDays(-1);

            var newHistory = new UserDepartmentHistory
            {
                UserId = userId,
                DepartmentId = assign.DepartmentId,
                DateFrom = assign.NewDepartmentAssign,
                DateTo = null
            };

            _dbContext.Set<UserDepartmentHistory>().Add(newHistory);

            histories.Add(newHistory);
            current = newHistory;
        }

        user.DepartmentId = current.DepartmentId;
    }

    private IEnumerable<UserDepartmentAssign> GetUserDepartmentAssigns()
    {
        var user1 = _dbContext.Users.First(x => x.Email == "dep.change.test1@modsen-software.com").Id;
        var user2 = _dbContext.Users.First(x => x.Email == "dep.change.test2@modsen-software.com").Id;
        var user3 = _dbContext.Users.First(x => x.Email == "dep.change.test3@modsen-software.com").Id;
        var user4 = _dbContext.Users.First(x => x.Email == "dep.change.test4@modsen-software.com").Id;
        var user5 = _dbContext.Users.First(x => x.Email == "dep.change.test5@modsen-software.com").Id;
        var user6 = _dbContext.Users.First(x => x.Email == "dep.change.test6@modsen-software.com").Id;
        var user7 = _dbContext.Users.First(x => x.Email == "dep.change.test7@modsen-software.com").Id;
        var user8 = _dbContext.Users.First(x => x.Email == "dep.change.test8@modsen-software.com").Id;
        var user9 = _dbContext.Users.First(x => x.Email == "dep.change.test9@modsen-software.com").Id;
        var user10 = _dbContext.Users.First(x => x.Email == "dep.change.test10@modsen-software.com").Id;

        yield return new UserDepartmentAssign(user1, "Net", new DateOnly(2026, 04, 07));

        yield return new UserDepartmentAssign(user2, "Net", new DateOnly(2026, 05, 01));

        yield return new UserDepartmentAssign(user3, "Net", new DateOnly(2026, 03, 31));

        yield return new UserDepartmentAssign(user4, "Net", new DateOnly(2025, 12, 15));

        yield return new UserDepartmentAssign(user5, "Net", new DateOnly(2024, 09, 22));
        yield return new UserDepartmentAssign(user5, "QA", new DateOnly(2024, 11, 29));

        yield return new UserDepartmentAssign(user6, "Net", new DateOnly(2024, 02, 02));
        yield return new UserDepartmentAssign(user6, "Java", new DateOnly(2026, 02, 02));

        yield return new UserDepartmentAssign(user7, "Net", new DateOnly(2024, 03, 08));
        yield return new UserDepartmentAssign(user7, "Java", new DateOnly(2024, 03, 27));

        yield return new UserDepartmentAssign(user8, "Net", new DateOnly(2026, 02, 20));
        yield return new UserDepartmentAssign(user8, "Java", new DateOnly(2026, 02, 20));

        yield return new UserDepartmentAssign(user9, "Net", new DateOnly(2026, 04, 29));

        yield return new UserDepartmentAssign(user10, "Net", new DateOnly(2026, 04, 02));
    }

    private sealed record UserDepartmentAssign(
        int UserId,
        string DepartmentId,
        DateOnly NewDepartmentAssign);
}
