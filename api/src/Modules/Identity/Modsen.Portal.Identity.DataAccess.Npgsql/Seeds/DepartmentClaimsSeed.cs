using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.Seeds;

internal class DepartmentClaimsSeed : ISeedClaimProvider<IdentityModuleDbContext, Department>
{
    public DepartmentClaimsSeed(IdentityModuleDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public IdentityModuleDbContext DbContext { get; }
    public string Scope => nameof(DepartmentClaim);

    public void Seed(Department department)
    {
        switch (department.Id)
        {
            case "Administration":
                AddDepartmentClaim(department.Id, EquipmentClaimTypeNames.AvailableEquipmentsWithoutAssigners,
                    department.Id, AllowedValueType.DepartmentId, EquipmentClaimTypeNames.GroupId);
                break;
            case "PMO":
                AddDepartmentClaim(department.Id, IdentityClaimTypeNames.AvailableProjectManagerRole,
                    department.Id, AllowedValueType.DepartmentId, IdentityClaimTypeNames.GroupId);
                break;
            case "Js":
                AddDepartmentClaim(department.Id, BookingClaimTypeNames.BookWorkplaceInSpace,
                    "MPN186-S404", AllowedValueType.DepartmentId, BookingClaimTypeNames.GroupId);
                AddDepartmentClaim(department.Id, BookingClaimTypeNames.BookWorkplaceInSpace,
                    "MPN186-S405", AllowedValueType.DepartmentId, BookingClaimTypeNames.GroupId);
                break;
            case "LD":
                AddDepartmentClaim(department.Id, BookingClaimTypeNames.BookWorkplaceInSpace,
                    "MPN186-S406", AllowedValueType.DepartmentId, BookingClaimTypeNames.GroupId);
                break;
            case "Java":
                AddDepartmentClaim(department.Id, BookingClaimTypeNames.BookWorkplaceInSpace,
                    "MPN186-S406", AllowedValueType.DepartmentId, BookingClaimTypeNames.GroupId);
                break;
            case "QA":
                AddDepartmentClaim(department.Id, BookingClaimTypeNames.BookWorkplaceInSpace,
                    "MPN186-S411", AllowedValueType.DepartmentId, BookingClaimTypeNames.GroupId);
                break;
            case "Sales":
                AddDepartmentClaim(department.Id, StaffingClaimTypeNames.ManageInterviews.Name,
                    department.Id, AllowedValueType.DepartmentId, StaffingClaimTypeNames.GroupId);
                break;
        }
    }
    private void AddDepartmentClaim(
        string departmentId,
        string claimType,
        string claimValue,
        AllowedValueType allowedValueType,
        string? groupId = null)
    {
        if (!DbContext.DepartmentClaims.Any(x => x.DepartmentId == departmentId && x.ClaimType == claimType && x.ClaimValue == claimValue))
        {
            if (!DbContext.Claims.Any(x => x.ClaimType == claimType))
            {
                DbContext.DepartmentClaims.Add(new DepartmentClaim
                {
                    DepartmentId = departmentId,
                    ClaimType = claimType,
                    Claim = new ClaimEntity
                    {
                        ClaimType = claimType,
                        Group = GetGroup(groupId),
                        AllowedValueType = allowedValueType,
                    },
                    ClaimValue = claimValue,
                });
            }
            else
            {
                DbContext.DepartmentClaims.Add(new DepartmentClaim
                {
                    DepartmentId = departmentId,
                    ClaimType = claimType,
                    ClaimValue = claimValue,
                });
            }

            DbContext.SaveChanges();
        }
    }

    private ClaimGroup GetGroup(string? groupName)
    {
        var group = DbContext.ClaimGroups.FirstOrDefault(x => x.Id == groupName);
        return group!;
    }
}
