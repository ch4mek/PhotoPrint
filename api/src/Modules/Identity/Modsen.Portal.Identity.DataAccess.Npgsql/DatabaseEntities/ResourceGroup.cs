using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class ResourceGroup
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string DepartmentId { get; set; }
    public bool IsActive { get; set; }

    public Department? Department { get; set; }
}

internal class ResourceGroupConfiguration : IEntityTypeConfiguration<ResourceGroup>
{
    public void Configure(EntityTypeBuilder<ResourceGroup> builder)
    {
        builder.ToTable("ResourceGroups", IdentityModuleDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(64)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(x => x.DepartmentId)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
