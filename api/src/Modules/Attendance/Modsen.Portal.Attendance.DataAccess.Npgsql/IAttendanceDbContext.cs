using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;

[assembly: InternalsVisibleTo("Modsen.Portal.Attendance.Requests")]
[assembly: InternalsVisibleTo("Modsen.Portal.Attendance.UnitTests")]

namespace Modsen.Portal.Attendance.DataAccess.Npgsql;

internal interface IAttendanceDbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<AttendanceEntity> Attendances { get; set; }
    public DbSet<ResourceGroupEntity> ResourceGroups { get; set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
