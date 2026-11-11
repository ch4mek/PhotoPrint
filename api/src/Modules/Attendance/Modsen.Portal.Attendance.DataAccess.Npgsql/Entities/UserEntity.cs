using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core;

namespace Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;

internal class UserEntity
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? PatronymicRu { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public int? ResourceGroupId { get; set; }
    public string? Position { get; set; }
    public bool IsActive { get; set; }

    public ResourceGroupEntity? ResourceGroup { get; set; }

    public ICollection<AttendanceEntity>? Attendances { get; set; }
}

internal class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    private const int NameMaxLenth = 250;

    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.UserId);
        builder.Property(user => user.UserId)
            .ValueGeneratedNever();

        builder.Property(user => user.FirstName)
            .HasMaxLength(NameMaxLenth)
            .IsRequired();

        builder.Property(user => user.LastName)
            .HasMaxLength(NameMaxLenth)
            .IsRequired();

        builder.Property(user => user.FirstNameRu)
            .HasMaxLength(NameMaxLenth)
            .IsRequired(false);

        builder.Property(user => user.LastNameRu)
            .HasMaxLength(NameMaxLenth)
            .IsRequired(false);

        builder.Property(user => user.PatronymicRu)
            .HasMaxLength(NameMaxLenth)
            .IsRequired(false);

        builder.Property(user => user.Email)
            .HasMaxLength(PortalSharedConstants.EmailMaxLength)
            .IsRequired();

        builder.Property(user => user.DepartmentId)
            .HasMaxLength(64)
            .IsRequired(false);

        builder.Property(user => user.ResourceGroupId).IsRequired(false);

        builder.Property(user => user.IsActive)
            .HasDefaultValue(value: true)
            .IsRequired();

        builder.Property(x => x.Position)
            .HasMaxLength(64)
            .IsRequired(false);

        builder.HasMany(x => x.Attendances).WithOne(x => x.User);
        builder.HasOne(x => x.ResourceGroup).WithMany(x => x.Users).HasForeignKey(x => x.ResourceGroupId);
    }
}
