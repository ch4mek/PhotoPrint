using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class UserInquiryAcceptEntity : AuditableEntity
{
    public int Id { get; set; }

    public int? UserInquiryId { get; set; }
    public UserInquiryEntity? UserInquiryAgreementAssign { get; set; }

    public InquiryAcceptPolicyScopes AcceptPolicyScope { get; set; }
    public int AcceptUserId { get; set; }
    public InquiriesUserEntity? AcceptUser { get; set; }
    public string? AcceptUserPosition { get; set; }

    public UserInquiryAcceptStatus Status { get; set; }

    public int? AcceptedById { get; set; }
    public InquiriesUserEntity? AcceptedByUser { get; set; }
    public DateTime? AcceptedAtUtc { get; set; }

    public int? RejectedById { get; set; }
    public InquiriesUserEntity? RejectedByUser { get; set; }
    public DateTime? RejectedAtUtc { get; set; }

    public InquiriesUserEntity? CreatedByUser { get; set; }
    public InquiriesUserEntity? UpdatedByUser { get; set; }
}

internal class UserInquiryAcceptConfiguration : IEntityTypeConfiguration<UserInquiryAcceptEntity>
{
    public void Configure(EntityTypeBuilder<UserInquiryAcceptEntity> builder)
    {
        builder.ToTable("UserInquiryAccepts");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Status).IsRequired().HasConversion<string>();
        builder.Property(x => x.AcceptPolicyScope).IsRequired().HasConversion<string>();
        builder.Property(x => x.AcceptUserPosition).IsRequired();
        
        builder.Property(x => x.AcceptUserId).IsRequired();

        builder.Property(x => x.AcceptedById).IsRequired(false);
        builder.Property(x => x.AcceptedAtUtc).IsRequired(false);

        builder.Property(x => x.RejectedById).IsRequired(false);
        builder.Property(x => x.RejectedAtUtc).IsRequired(false);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.Property(x => x.UserInquiryId).IsRequired(false);
        
        builder.HasOne(x => x.AcceptUser)
            .WithMany()
            .HasForeignKey(x => x.AcceptUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AcceptedByUser)
            .WithMany()
            .HasForeignKey(x => x.AcceptedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RejectedByUser)
            .WithMany()
            .HasForeignKey(x => x.RejectedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UserInquiryAgreementAssign)
            .WithMany(x => x.UserInquiryAccepts)
            .HasForeignKey(x => x.UserInquiryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
