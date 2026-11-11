using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Attendance.DataAccess.Npgsql;

internal class AttendanceDbContext : DbContext, IAttendanceDbContext
{
    public const string Schema = "attendance";

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<AttendanceEntity> Attendances { get; set; }
    public DbSet<ResourceGroupEntity> ResourceGroups { get; set; }

    public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(Schema)
            .ApplyConfigurationsFromAssembly(typeof(AttendanceDbContext).Assembly);
    }
}
