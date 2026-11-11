using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class UserInquiryCommentFileEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string FileId { get; set; }
    public string? FileName { get; set; }
    public long FileSize { get; set; }

    public int UserInquiryCommentId { get; set; }
    public UserInquiryCommentEntity? UserInquiryComment { get; set; }

    public InquiriesUserEntity? CreatedByUser { get; set; }
    public InquiriesUserEntity? UpdatedByUser { get; set; }
}

internal class UserInquiryCommentFileConfiguration : IEntityTypeConfiguration<UserInquiryCommentFileEntity>
{
    public void Configure(EntityTypeBuilder<UserInquiryCommentFileEntity> builder)
    {
        builder.ToTable("UserInquiryCommentFiles");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.FileId).HasMaxLength(128).IsRequired();
        builder.Property(x => x.FileName).HasMaxLength(256).IsRequired(false);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.Property(x => x.UserInquiryCommentId).IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UserInquiryComment)
            .WithMany(x => x.UserInquiryCommentFiles)
            .HasForeignKey(x => x.UserInquiryCommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
