using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class UserInquiryFileEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string FileId { get; set; }
    public string? FileName { get; set; }
    public long FileSize { get; set; }

    public int? UserInquiryId { get; set; }
    public UserInquiryEntity? UserInquiryAgreementAssign { get; set; }

    public InquiriesUserEntity? CreatedByUser { get; set; }
    public InquiriesUserEntity? UpdatedByUser { get; set; }
}

internal class UserInquiryFileConfiguration : IEntityTypeConfiguration<UserInquiryFileEntity>
{
    public void Configure(EntityTypeBuilder<UserInquiryFileEntity> builder)
    {
        builder.ToTable("UserInquiryFiles");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.FileId).HasMaxLength(128).IsRequired();
        builder.Property(x => x.FileName).HasMaxLength(256).IsRequired(false);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.Property(x => x.UserInquiryId).IsRequired(false);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UserInquiryAgreementAssign)
            .WithMany(x => x.UserInquiryFiles)
            .HasForeignKey(x => x.UserInquiryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
