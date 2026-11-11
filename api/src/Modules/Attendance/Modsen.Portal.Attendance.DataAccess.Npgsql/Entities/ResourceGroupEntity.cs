using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;

internal class ResourceGroupEntity
{
    public int ResourceGroupId { get; set; }
    public required string Name { get; set; }

    public ICollection<UserEntity>? Users { get; set; }
}

internal class ResourceGroupEntityConfiguration : IEntityTypeConfiguration<ResourceGroupEntity>
{
    public void Configure(EntityTypeBuilder<ResourceGroupEntity> builder)
    {
        builder.ToTable("ResourceGroups");

        builder.HasKey(x => x.ResourceGroupId);
        builder.Property(x => x.ResourceGroupId)
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.HasMany(x => x.Users).WithOne(x => x.ResourceGroup);
    }
}
