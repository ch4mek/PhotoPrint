using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class User : IdentityUser<int>, IAuditableEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? PatronymicRu { get; set; }
    public string? Color { get; set; }
    public string? AvatarColor { get; set; }
    public string? AvatarFileId { get; set; }
    public string? TimeZone { get; set; }
    public string? SignatureFullName { get; set; }
    public string? SignaturePosition { get; set; }
    public string? SignaturePhone { get; set; }

    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public LocationType? Location { get; set; }

    public bool IsActive { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateOnly? HireDate { get; set; }
    public int? CountryId { get; set; }
    public int RegisteredLocationId { get; set; }
    public string? CurrentLocation { get; set; }
    public Department? Department { get; set; }
    public ResourceGroup? ResourceGroup { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<PermissionAssignment> PermissionAssignments { get; set; } = [];
    public ICollection<UserClaim> Claims { get; set; } = [];
    public ICollection<UserDepartmentHistory> UserDepartmentHistory { get; set; } = [];
}

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public virtual void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", IdentityModuleDbContext.Schema);

        builder.HasKey(e => e.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
        builder.HasIndex(x => x.IsActive);

        builder.Property(x => x.TimeZone).HasMaxLength(64).IsRequired(false);

        builder.Property(x => x.SignatureFullName).IsRequired(false).HasMaxLength(500);
        builder.Property(x => x.SignaturePosition).IsRequired(false).HasMaxLength(250);

        builder.Property(x => x.FirstName).HasMaxLength(250);
        builder.Property(x => x.LastName).HasMaxLength(250);

        builder.Property(x => x.FirstNameRu).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.LastNameRu).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.PatronymicRu).IsRequired(false).HasMaxLength(250);

        builder.Property(x => x.Color).HasMaxLength(7).IsFixedLength();
        builder.Property(x => x.AvatarColor).HasMaxLength(7).IsFixedLength();

        builder.Property(x => x.Email).HasMaxLength(PortalSharedConstants.EmailMaxLength);

        builder.Property("NormalizedEmail").HasMaxLength(PortalSharedConstants.EmailMaxLength);
        builder.Property("NormalizedUserName").HasMaxLength(PortalSharedConstants.EmailMaxLength);

        builder.Property(x => x.AvatarFileId).IsRequired(false).HasMaxLength(128);

        builder.Property(x => x.SignaturePhone).IsRequired(false).HasMaxLength(15);

        builder.Property(x => x.RegisteredLocationId).IsRequired();

        builder.HasMany(x => x.UserRoles)
            .WithOne()
            .HasForeignKey(x => x.UserId);

        builder.Property(x => x.Location).HasConversion<string>();

        builder.Property(x => x.DepartmentId).HasMaxLength(64);
        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .IsRequired(required: false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ResourceGroup)
            .WithMany()
            .HasForeignKey(x => x.ResourceGroupId)
            .IsRequired(required: false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);
    }
}
