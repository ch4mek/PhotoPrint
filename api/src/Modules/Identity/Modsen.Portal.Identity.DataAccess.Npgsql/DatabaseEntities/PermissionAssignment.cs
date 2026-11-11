using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class PermissionAssignment
{
    public int Id { get; set; }
    public int PermissionDefinitionId { get; set; }
    public int UserId { get; set; }
    public string? ResourceType { get; set; }
    public string? ResourceId { get; set; }
    public string? ResourceName { get; set; }

    public PermissionDefinition? Permission { get; set; }
    public User? User { get; set; }
}

internal class PermissionAssignmentConfiguration : IEntityTypeConfiguration<PermissionAssignment>
{
    public void Configure(EntityTypeBuilder<PermissionAssignment> builder)
    {
        builder.ToTable("PermissionAssignments", IdentityModuleDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ResourceId)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.ResourceType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.ResourceName)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasOne(x => x.Permission)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.PermissionDefinitionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.PermissionAssignments)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.UserId, x.PermissionDefinitionId, x.ResourceId, x.ResourceType })
            .HasDatabaseName("IX_PermissionAssignments_UserId_PermissionDefinitionId_ResourceId_ResourceType")
            .IsUnique();
    }
}
