using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

internal class ColorPicker
{
    public int Id { get; set; }
    public required string Color { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}

internal class ColorPickerConfiguration : IEntityTypeConfiguration<ColorPicker>
{
    public void Configure(EntityTypeBuilder<ColorPicker> builder)
    {
        builder.ToTable("ColorPicker", IdentityModuleDbContext.Schema);

        builder.HasKey(e => e.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Color).IsRequired().HasMaxLength(7).IsFixedLength();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(x => new { x.UserId, x.Color })
            .IsUnique();
    }
}
