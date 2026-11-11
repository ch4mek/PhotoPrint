using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.IntegrationTests.Seeds;

internal class IntegrationTestResourceGroupsSeed : ISeedEntitiesProvider<IdentityModuleDbContext>
{
    private readonly IdentityModuleDbContext _dbContext;

    public IntegrationTestResourceGroupsSeed(IdentityModuleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        AddResourceGroup(new ResourceGroup
        {
            Name = ".NET Minsk - Group 1",
            DepartmentId = "Net",
            IsActive = true
        }, "modsen.portal.test.groupmanager@modsen-software.com");
        AddResourceGroup(new ResourceGroup
        {
            Name = "Java Minsk - Group 1",
            DepartmentId = "Java",
            IsActive = true
        }, "groupmanager.java.test@modsen-software.com");
        AddResourceGroup(new ResourceGroup
        {
            Name = "Java Minsk - Group 2",
            DepartmentId = "Java",
            IsActive = true
        }, null);
    }

    private void AddResourceGroup(ResourceGroup group, string groupManagerEmail)
    {
        if (!_dbContext.ResourceGroups.Any(x => x.Name == group.Name))
        {
            var groupManager = _dbContext.Users.FirstOrDefault(x => x.Email == groupManagerEmail);
            if (groupManager == null)
            {
                _dbContext.ResourceGroups.Add(group);
                _dbContext.SaveChanges();
            }
            else
            {
                _dbContext.ResourceGroups.Add(group);
                _dbContext.SaveChanges();

                groupManager.ResourceGroupId = group.Id;

                _dbContext.Users.Update(groupManager);
                _dbContext.SaveChanges();
            }
        }
    }
}
