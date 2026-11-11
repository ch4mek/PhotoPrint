using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class InquiriesUserEntity
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? PatronymicRu { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public int? RegisteredLocationId { get; set; }
    public string? AvatarFileId { get; set; }
    public string? Color { get; set; }
    public string? Position { get; set; }
    public int? PositionOrderWeight { get; set; }
    public int? ResourceGroupId { get; set; }
    public int? PeopleForceEmployeeId { get; set; }
    public DateTime? SynchronizedAtUtc { get; set; }
    public bool IsActive { get; set; }
    public DateOnly? HireDate { get; set; }

    public List<UserInquiryEntity> UserInquiries { get; set; } = [];
}

internal class InquiriesUserConfiguration : IEntityTypeConfiguration<InquiriesUserEntity>
{
    public void Configure(EntityTypeBuilder<InquiriesUserEntity> builder)
    {
        builder.ToTable("InquiriesUsers");

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(250);

        builder.Property(x => x.LastName).IsRequired().HasMaxLength(250);

        builder.Property(x => x.Email).IsRequired().HasMaxLength(550);

        builder.Property(x => x.DepartmentId).IsRequired(false).HasMaxLength(64);

        builder.Property(x => x.RegisteredLocationId).IsRequired(false);

        builder.Property(x => x.AvatarFileId).IsRequired(false);

        builder.Property(x => x.Color).IsRequired(false);

        builder.Property(x => x.Position).IsRequired(false);

        builder.Property(x => x.PositionOrderWeight).IsRequired(false);

        builder.Property(x => x.ResourceGroupId).IsRequired(false);

        builder.Property(x => x.PeopleForceEmployeeId).IsRequired(false);

        builder.Property(x => x.SynchronizedAtUtc).IsRequired(false);

        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

        builder.Property(x => x.HireDate).IsRequired(false);

        builder.HasIndex(x => x.PeopleForceEmployeeId);
    }
}
