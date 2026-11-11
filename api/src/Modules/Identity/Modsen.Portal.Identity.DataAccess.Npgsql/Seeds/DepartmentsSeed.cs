using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Seeds;

internal class DepartmentsSeed : ISeedEntitiesProvider<IdentityModuleDbContext>
{
    private readonly IdentityModuleDbContext _dbContext;
    private readonly ISeedClaimProvider<IdentityModuleDbContext, Department>[] _claimsSeedProviders;
    public DepartmentsSeed(IdentityModuleDbContext dbContext,
        IEnumerable<ISeedClaimProvider<IdentityModuleDbContext, Department>> claimsSeedProviders)
    {
        _dbContext = dbContext;
        _claimsSeedProviders = claimsSeedProviders
            .Where(x => x.Scope == nameof(DepartmentClaim))
            .ToArray();
    }

    public void Seed()
    {
        foreach (var department in GetDepartments())
        {
            CreateIfNotExists(department);
        }

        _dbContext.SaveChanges();

        var departments = _dbContext.Departments.ToArray();

        foreach (var department in departments)
        {
            foreach (var claimsSeedProvider in _claimsSeedProviders)
            {
                claimsSeedProvider.Seed(department);
                _dbContext.SaveChanges();
            }

        }
    }

    private void CreateIfNotExists(Department department)
    {
        var hasAny = _dbContext.Set<Department>().Any(x => x.Id == department.Id);
        if (!hasAny)
        {
            _dbContext.Set<Department>().Add(department);
        }
    }

    private IEnumerable<Department> GetDepartments()
    {
        var ceoId = _dbContext.Roles.First(x => x.Name == PortalRoleNames.Ceo).Id;
        var ctoId = _dbContext.Roles.First(x => x.Name == PortalRoleNames.Cto).Id;

        yield return Create("Js", "Js", "JavaScript", "JavaScript", 100, ctoId);
        yield return Create("Net", "Net", ".NET", ".NET", 200, ctoId);
        yield return Create("Java", "Java", "Java", "Java", 300, ctoId);
        yield return Create("QA", "QA", "QA", "QA", 400, ctoId);
        yield return Create("DevOps", "DevOps", "DevOps", "DevOps", 500, ctoId);
        yield return Create("Mobile", "Mobile", "Mobile", "Mobile", 600, ctoId);
        yield return Create("AIML", "AI ML", "AI ML", null, 700, ctoId);
        yield return Create("SAP", "SAP", "SAP", null, 800, ctoId);
        yield return Create("Python", "Python", "Python", "Python", 900, ctoId);
        yield return Create("Ruby", "Ruby", "Ruby", null, 1000, ctoId);
        yield return Create("PHP", "PHP", "PHP", null, 1100, ctoId);
        yield return Create("Design", "Design", "Design", "Design", 1200, ceoId);
        yield return Create("BA", "BA", "BA", "BA", 1300, ctoId);
        yield return Create("HR", "HR", "HR", "HR", 1400, ceoId);
        yield return Create("Recruitment", "Recruitment", "Recruitment", "Recruitment", 1500, ceoId);
        yield return Create("Marketing", "Marketing", "Marketing", "Marketing", 1600, ceoId);
        yield return Create("PMO", "PMO", "PMO", "PMO", 1700, ceoId);
        yield return Create("Sales", "Sales", "Sales", "Sales", 1800, ceoId);
        yield return Create("Administration", "Administration", "Administration", null, 1900, ceoId);
        yield return Create("SysAdmin", "System Administrators", "System Administrators", null, 2000, ctoId);
        yield return Create("1C", "1C", "1C", null, 2100, ctoId);
        yield return Create("LD", "LD", "LD", null, 2200, ctoId);
        yield return Create("Accounting", "Accounting", "Accounting", null, 2300, ceoId);
        yield break;

        static Department Create(string id, string shortName, string fullName, string? modsenApiValue, int orderWeight, int departmentReportingToRoleId)
        {
            return new Department
            {
                Id = id,
                ShortName = shortName,
                FullName = fullName,
                ModsenApiValue = modsenApiValue,
                OrderWeight = orderWeight,
                DepartmentReportingToRoleId = departmentReportingToRoleId,
            };
        }
    }
}
