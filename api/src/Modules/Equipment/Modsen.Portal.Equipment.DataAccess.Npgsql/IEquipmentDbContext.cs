using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

[assembly: InternalsVisibleTo("Modsen.Portal.IntegrationTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.Equipment.Requests")]
namespace Modsen.Portal.Equipment.DataAccess.Npgsql;

internal interface IEquipmentDbContext
{
    public DbSet<EquipmentEntity> Equipments { get; }
    public DbSet<EquipmentAssignEntity> EquipmentAssigns { get; }
    public DbSet<EquipmentUser> EquipmentUsers { get; }
    public DbSet<EquipmentTypeEntity> EquipmentTypes { get; }
    public DbSet<EquipmentReport> EquipmentReports { get; }
    public DbSet<EquipmentReportRelevancePeriod> EquipmentReportRelevancePeriods { get; }
    public DbSet<EquipmentDepartmentEntity> EquipmentDepartments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
