using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class InquiryTemplateAgreementEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int DaysPerContractYear { get; set; }
    
    public bool IsDefault { get; set; }
    public AgreementType Type { get; set; }

    public InquiryTemplateEntity? InquiryTemplate { get; set; }
    public int InquiryTemplateId { get; set; }

    public List<UserAgreementEntity> UserAgreements { get; set; } = [];
    public List<UserInquiryEntity> UserInquiryAgreementAssigns { get; set; } = [];
    public bool IsDeleted { get; set; }
}

internal class InquiryTemplateAgreementConfiguration : IEntityTypeConfiguration<InquiryTemplateAgreementEntity>
{
    public void Configure(EntityTypeBuilder<InquiryTemplateAgreementEntity> builder)
    {
        builder.ToTable("InquiryTemplateAgreements");

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasKey(agreementTemplate => agreementTemplate.Id);
        builder.Property(agreementTemplate => agreementTemplate.Id).ValueGeneratedOnAdd();

        builder.Property(property => property.Type)
            .HasConversion<string>();

        builder.Property(agreementTemplate => agreementTemplate.DaysPerContractYear).IsRequired();
        builder.Property(agreementTemplate => agreementTemplate.IsDefault).IsRequired();

        builder.HasOne(agreementTemplate => agreementTemplate.InquiryTemplate)
           .WithMany(inquiryTemplate => inquiryTemplate.InquiryTemplateAgreements)
           .HasForeignKey(agreementTemplate => agreementTemplate.InquiryTemplateId)
           .OnDelete(DeleteBehavior.Restrict);
    }
}
