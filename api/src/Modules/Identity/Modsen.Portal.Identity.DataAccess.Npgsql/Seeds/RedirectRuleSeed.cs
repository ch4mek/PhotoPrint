using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Seeds;

internal class RedirectRuleSeed : ISeedEntitiesProvider<IdentityModuleDbContext>
{
    private readonly IdentityModuleDbContext _dbContext;

    public RedirectRuleSeed(IdentityModuleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        UpsertRule("e969d114", "https://modsen-software.peopleforce.io/");
        UpsertRule("4ff2b2c0", "https://modsen-software.peopleforce.io/knowledge_base/categories/13259");
        UpsertRule("563fa16d", "https://modsen-software.peopleforce.io/knowledge_base/articles/25088-employee-handbook");
        UpsertRule("72f35c8a", "https://modsen.atlassian.net/servicedesk/customer/portals");

        _dbContext.SaveChanges();
    }

    private void UpsertRule(string key, string redirectUrl)
    {
        var rule = _dbContext.RedirectRules.FirstOrDefault(x => x.RedirectKey == key);
        if (rule == null)
        {
            rule = new RedirectRule
            {
                RedirectKey = key,
                RedirectTo = redirectUrl
            };
            _dbContext.RedirectRules.Add(rule);
        }

        rule.RedirectTo = redirectUrl;
    }
}
