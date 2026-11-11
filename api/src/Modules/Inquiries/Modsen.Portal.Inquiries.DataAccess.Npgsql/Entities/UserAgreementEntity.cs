using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class UserAgreementEntity : AuditableEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public InquiriesUserEntity? User { get; set; }

    public int InquiryAgreementTemplateId { get; set; }
    public InquiryTemplateAgreementEntity? InquiryTemplateAgreement { get; set; }

    public DateOnly DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }

    public List<UserAgreementTransactionEntity> UserAgreementTransactions { get; set; } = [];

    public InquiriesUserEntity? CreatedByUser { get; set; }
    public InquiriesUserEntity? UpdatedByUser { get; set; }
    public bool IsDeleted { get; set; }
}

internal class UserAgreementConfiguration : IEntityTypeConfiguration<UserAgreementEntity>
{
    public void Configure(EntityTypeBuilder<UserAgreementEntity> builder)
    {
        builder.ToTable("UserAgreements");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.DateFrom).IsRequired();

        builder.Property(x => x.DateTo).IsRequired(false);

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.InquiryAgreementTemplateId).IsRequired();

        builder.HasOne(x => x.InquiryTemplateAgreement)
           .WithMany(agreementTemplate => agreementTemplate.UserAgreements)
           .HasForeignKey(x => x.InquiryAgreementTemplateId)
           .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);



        builder.HasIndex(x => new
        {
            x.UserId,
            x.DateFrom,
            x.DateTo
        });
    }
}
