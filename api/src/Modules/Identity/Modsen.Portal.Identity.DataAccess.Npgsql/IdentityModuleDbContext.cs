using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Outbox.Storage;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql
{
    internal class IdentityModuleDbContext :
        IdentityDbContext<User, Role, int, UserClaim, UserRole, IdentityUserLogin<int>, RoleClaim, IdentityUserToken<int>>,
        IIdentityDbContext,
        IDataProtectionKeyContext
    {
        public static string Schema => "identity";

        public DbSet<Department> Departments { get; set; }
        public DbSet<PermissionDefinition> PermissionDefinitions { get; set; }
        public DbSet<PermissionAssignment> PermissionAssignments { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<RedirectRule> RedirectRules { get; set; }
        public DbSet<ResourceGroup> ResourceGroups { get; set; }
        public DbSet<DepartmentClaim> DepartmentClaims { get; set; }
        public DbSet<PortalSetting> PortalSettings { get; set; }
        public DbSet<CustomDetails> CustomDetails { get; set; }
        public DbSet<DigitalHrConversationSession> DigitalHrConversationSessions { get; set; }

        public DbSet<ClaimGroup> ClaimGroups { get; set; }
        public DbSet<ClaimEntity> Claims { get; set; }

        public DbSet<ColorPicker> ColorPickers { get; set; }
        public DbSet<UserMessage> UserMessages { get; set; }

        public DbSet<UserDepartmentHistory> UserDepartmentHistory { get; set; }

        public DbSet<MicrofrontendsConfigEntity> MicrofrontendsConfigs { get; set; }

        public IdentityModuleDbContext(DbContextOptions<IdentityModuleDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema(Schema);

            builder.ApplyConfiguration(new DepartmentConfiguration());
            builder.ApplyConfiguration(new PermissionAssignmentConfiguration());
            builder.ApplyConfiguration(new PermissionDefinitionConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RedirectRuleConfiguration());
            builder.ApplyConfiguration(new DepartmentClaimConfiguration());
            builder.ApplyConfiguration(new RoleClaimConfiguration());
            builder.ApplyConfiguration(new UserClaimConfiguration());
            builder.ApplyConfiguration(new ClaimEntityConfiguration());
            builder.ApplyConfiguration(new PortalSettingConfiguration());
            builder.ApplyConfiguration(new CustomDetailsConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
            builder.ApplyConfiguration(new ColorPickerConfiguration());
            builder.ApplyConfiguration(new OutboxEventConfiguration("OutboxEvents", Schema));
            builder.ApplyConfiguration(new MicrofrontendsConfigConfiguration());
            builder.ApplyConfiguration(new UserDepartmentHistoryConfiguration());
            builder.ApplyConfiguration(new DigitalHrConversationSessionConfiguration());
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return SaveChangesAsync(cancellationToken);
        }
    }
}
