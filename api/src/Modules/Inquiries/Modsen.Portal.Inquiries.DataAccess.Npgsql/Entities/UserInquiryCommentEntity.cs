using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class UserInquiryCommentEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string Comment { get; set; }

    public int? UserInquiryId { get; set; }
    public UserInquiryEntity? UserInquiryAgreementAssign { get; set; }

    public List<UserInquiryCommentFileEntity> UserInquiryCommentFiles { get; set; } = [];

    public InquiriesUserEntity? CreatedByUser { get; set; }
    public InquiriesUserEntity? UpdatedByUser { get; set; }
}

internal class UserInquiryCommentConfiguration : IEntityTypeConfiguration<UserInquiryCommentEntity>
{
    public void Configure(EntityTypeBuilder<UserInquiryCommentEntity> builder)
    {
        builder.ToTable("UserInquiryComments");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Comment).HasMaxLength(1024 * 8).IsRequired();

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
            .WithMany(x => x.UserInquiryComments)
            .HasForeignKey(x => x.UserInquiryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
