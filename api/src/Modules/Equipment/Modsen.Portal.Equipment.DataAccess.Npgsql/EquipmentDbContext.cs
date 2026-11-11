using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql;

internal class EquipmentDbContext : DbContext, IEquipmentDbContext
{
    public const string Schema = "equipment";

    public DbSet<EquipmentEntity> Equipments { get; set; }
    public DbSet<EquipmentAssignEntity> EquipmentAssigns { get; set; }
    public DbSet<EquipmentUser> EquipmentUsers { get; set; }
    public DbSet<EquipmentTypeEntity> EquipmentTypes { get; set; }
    public DbSet<EquipmentReport> EquipmentReports { get; set; }
    public DbSet<EquipmentReportRelevancePeriod> EquipmentReportRelevancePeriods { get; set; }
    public DbSet<EquipmentDepartmentEntity> EquipmentDepartments { get; set; }

    public EquipmentDbContext(DbContextOptions<EquipmentDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema(Schema)
            .ApplyConfigurationsFromAssembly(typeof(EquipmentDbContext).Assembly);
    }
}
