using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class UserRole : IdentityUserRole<int>
{
    public User? User { get; set; }
    public Role? Role { get; set; }
}

internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("AspNetUserRoles");
        builder
            .HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId);
        builder
            .HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId);
    }
}
