using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class PermissionDefinition
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    public ICollection<PermissionAssignment> Assignments { get; set; } = [];
}

internal class PermissionDefinitionConfiguration : IEntityTypeConfiguration<PermissionDefinition>
{
    public void Configure(EntityTypeBuilder<PermissionDefinition> builder)
    {
        builder.ToTable("PermissionDefinitions", IdentityModuleDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(1024);
    }
}
