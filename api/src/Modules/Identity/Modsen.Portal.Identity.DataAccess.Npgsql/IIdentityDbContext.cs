using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

[assembly: InternalsVisibleTo("Modsen.Portal.Identity.Requests")]
namespace Modsen.Portal.Identity.DataAccess.Npgsql;

internal interface IIdentityDbContext
{
    public DbSet<User> Users { get; }
    public DbSet<Role> Roles { get; }
    public DbSet<Department> Departments { get; }
    public DbSet<DepartmentClaim> DepartmentClaims { get; }
    public DbSet<UserRole> UserRoles { get; }
    public DbSet<ResourceGroup> ResourceGroups { get; }
    public DbSet<PermissionDefinition> PermissionDefinitions { get; }
    public DbSet<PermissionAssignment> PermissionAssignments { get; }
    public DbSet<RedirectRule> RedirectRules { get; set; }
    public DbSet<UserClaim> UserClaims { get; set; }
    public DbSet<RoleClaim> RoleClaims { get; set; }
    public DbSet<PortalSetting> PortalSettings { get; set; }
    public DbSet<CustomDetails> CustomDetails { get; set; }
    public DbSet<DigitalHrConversationSession> DigitalHrConversationSessions { get; set; }
    public DbSet<ClaimGroup> ClaimGroups { get; set; }
    public DbSet<ClaimEntity> Claims { get; set; }
    public DbSet<ColorPicker> ColorPickers { get; set; }
    public DbSet<UserMessage> UserMessages { get; set; }
    public DbSet<UserDepartmentHistory> UserDepartmentHistory { get; set; }
    public DbSet<MicrofrontendsConfigEntity> MicrofrontendsConfigs { get; set; }
    Task<int> SaveChanges(CancellationToken cancellationToken);
}

