using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class UserAgreementTransactionEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public InquiriesUserEntity? User { get; set; }

    public DateTime TransactionDate { get; set; }
    public UserAgreementTransactionType TransactionType { get; set; }
    public string? Desctiption { get; set; }
    public decimal AmountOfDays { get; set; }

    public int? UserAgreementId { get; set; }
    public UserAgreementEntity? UserAgreement { get; set; }

    public int InquiryTemplateId { get; set; }
    public InquiryTemplateEntity? InquiryTemplate { get; set; }

    public int? UserInquiryId { get; set; }
    public UserInquiryEntity? UserInquiry { get; set; }

    public bool IsCreatedManually { get; set; }
}

internal class UserAgreementTransactionConfiguration : IEntityTypeConfiguration<UserAgreementTransactionEntity>
{
    public void Configure(EntityTypeBuilder<UserAgreementTransactionEntity> builder)
    {
        builder.ToTable("UserAgreementTransactions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.TransactionDate).IsRequired();

        builder.Property(x => x.TransactionType).IsRequired().HasConversion<string>();

        builder.Property(x => x.AmountOfDays).IsRequired();

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.UserAgreementId).IsRequired(false);

        builder.Property(x => x.UserInquiryId).IsRequired(false);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UserAgreement)
            .WithMany(x => x.UserAgreementTransactions)
            .HasForeignKey(x => x.UserAgreementId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UserInquiry)
            .WithMany(x => x.UserAgreementTransactions)
            .HasForeignKey(x => x.UserInquiryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsCreatedManually)
            .IsRequired(true)
            .HasDefaultValue(false);

        builder.HasIndex(x => new
        {
            x.UserId,
            x.TransactionType
        });
    }
}
