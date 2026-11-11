using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.Attendance.DataAccess.Npgsql;

internal class DesignTimeAttendanceDbContext : DesignTimeBaseDbContext, IDesignTimeDbContextFactory<AttendanceDbContext>
{
    public AttendanceDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AttendanceDbContext>();
        builder.UsePortalMySql(GetConnectionString());

        return new AttendanceDbContext(builder.Options);
    }
}
