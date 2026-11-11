using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;

internal class DesignTimeKnowledgeBaseDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<KnowledgeBaseDbContext>
{
    public KnowledgeBaseDbContext CreateDbContext(string[] args)
    {
        return new KnowledgeBaseDbContext(new DbContextOptionsBuilder<KnowledgeBaseDbContext>().UsePortalMySql(GetConnectionString()).Options);
    }
}
