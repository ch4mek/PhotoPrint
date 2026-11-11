using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingDepartmentsToEquipmentMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly EquipmentDbContext _equipmentDbContext;

    public PublishExistingDepartmentsToEquipmentMigrationTask(
        IdentityModuleDbContext identityModuleDbContext,
        EquipmentDbContext equipmentDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _equipmentDbContext = equipmentDbContext;
    }

    public string TaskName => "PublishExistingDepartmentsToEquipment";

    public void Execute()
    {
        var departments = _identityModuleDbContext.Departments.ToArray();
        var departmentIds = new List<string>();

        foreach (var department in departments)
        {
            var equipmentDepartment = _equipmentDbContext.EquipmentDepartments
                .FirstOrDefault(x => x.DepartmentId == department.Id);
            if (equipmentDepartment == null)
            {
                equipmentDepartment = new EquipmentDepartmentEntity
                {
                    DepartmentId = department.Id,
                    FullName = department.FullName,
                    ShortName = department.ShortName
                };
                _equipmentDbContext.Add(equipmentDepartment);
            }
            else
            {
                equipmentDepartment.ShortName = department.ShortName;
                equipmentDepartment.FullName = department.FullName;
                _equipmentDbContext.Update(equipmentDepartment);
            }

            departmentIds.Add(department.Id);
        }

        _equipmentDbContext.EquipmentDepartments
            .Where(dep => !departmentIds.Contains(dep.DepartmentId))
            .ExecuteDelete();
        _equipmentDbContext.SaveChanges();
    }
}
