using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingDepartmentsToFinanceMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly FinanceDbContext _financeDbContext;

    public PublishExistingDepartmentsToFinanceMigrationTask(
        IdentityModuleDbContext identityModuleDbContext,
        FinanceDbContext financeDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _financeDbContext = financeDbContext;
    }

    public string TaskName => "PublishExistingDepartmentsToFinance";

    public void Execute()
    {
        var departments = _identityModuleDbContext.Departments.ToArray();
        var departmentIds = new List<string>();

        foreach (var department in departments)
        {
            var financeDepartment = _financeDbContext.FinanceDepartments
                .FirstOrDefault(x => x.DepartmentId == department.Id);
            if (financeDepartment == null)
            {
                financeDepartment = new FinanceDepartmentEntity
                {
                    DepartmentId = department.Id,
                    FullName = department.FullName,
                    ShortName = department.ShortName,
                    DepartmentOrderWeight = department.OrderWeight,
                };
                _financeDbContext.Add(financeDepartment);
            }
            else
            {
                financeDepartment.ShortName = department.ShortName;
                financeDepartment.FullName = department.FullName;
                financeDepartment.DepartmentOrderWeight = department.OrderWeight;
                _financeDbContext.Update(financeDepartment);
            }

            departmentIds.Add(department.Id);
        }

        _financeDbContext.FinanceDepartments
            .Where(dep => !departmentIds.Contains(dep.DepartmentId))
            .ExecuteDelete();
        _financeDbContext.SaveChanges();
    }
}

