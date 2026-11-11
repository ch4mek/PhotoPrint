using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class InquiryTemplateAcceptPolicyEntity : AuditableEntity
{
    public int Id { get; set; }

    public int InquiryTemplateId { get; set; }
    public InquiryTemplateEntity? InquiryTemplate { get; set; }

    public int OrderWeight { get; set; }
    public InquiryAcceptPolicyScopes Scope { get; set; }
    public int? UserId { get; set; }
    public InquiriesUserEntity? User { get; set; }

    public InquiryAcceptPolicyScopes? SubstituteScope { get; set; }
    public int? SubstituteUserId { get; set; }
    public InquiriesUserEntity? SubstituteUser { get; set; }

    public InquiriesUserEntity? CreatedByUser { get; set; }
    public InquiriesUserEntity? UpdatedByUser { get; set; }
}

internal class InquiryTemplateAcceptPolicyConfiguration : IEntityTypeConfiguration<InquiryTemplateAcceptPolicyEntity>
{
    public void Configure(EntityTypeBuilder<InquiryTemplateAcceptPolicyEntity> builder)
    {
        builder.ToTable("InquiryTemplateAcceptPolicies");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.OrderWeight).IsRequired();

        builder.Property(x => x.SubstituteScope).IsRequired(false).HasConversion<string>();
        builder.Property(x => x.Scope).IsRequired().HasConversion<string>();

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.Property(x => x.InquiryTemplateId).IsRequired();

        builder.Property(x => x.UserId).IsRequired(false);

        builder.Property(x => x.SubstituteUserId).IsRequired(false);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InquiryTemplate)
            .WithMany(x => x.InquiryTemplateAcceptPolicyPolicies)
            .HasForeignKey(x => x.InquiryTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SubstituteUser)
            .WithMany()
            .HasForeignKey(x => x.SubstituteUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
