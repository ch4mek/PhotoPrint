using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class UserInquiryTemplateEntity : AuditableEntity
{
    public int Id { get; set; }

    public DateOnly DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }

    public int AssignedToUserId { get; set; }
    public InquiriesUserEntity? AssignedToUser { get; set; }

    public int InquiryTemplateId { get; set; }
    public InquiryTemplateEntity? InquiryTemplate { get; set; }

    public InquiriesUserEntity? CreatedByUser { get; set; }
    public InquiriesUserEntity? UpdatedByUser { get; set; }
}

internal class UserInquiryTemplateConfiguration : IEntityTypeConfiguration<UserInquiryTemplateEntity>
{
    public void Configure(EntityTypeBuilder<UserInquiryTemplateEntity> builder)
    {
        builder.ToTable("UserInquiryTemplates");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.DateFrom).IsRequired();

        builder.Property(x => x.DateTo).IsRequired(false);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder
            .HasOne(x => x.AssignedToUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.InquiryTemplate)
            .WithMany(x => x.InquiryTemplateAssigns)
            .HasForeignKey(x => x.InquiryTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
