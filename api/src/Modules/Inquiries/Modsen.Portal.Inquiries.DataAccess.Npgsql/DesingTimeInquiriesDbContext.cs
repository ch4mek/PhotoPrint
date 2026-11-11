using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql;

internal class DesingTimeInquiriesDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<InquiriesDbContext>
{
    public InquiriesDbContext CreateDbContext(string[] args)
    {
        return new InquiriesDbContext(new DbContextOptionsBuilder<InquiriesDbContext>().UseNpgsql(GetConnectionString()).Options);
    }
}
