using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class InquiryTemplateEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public InquiryType InquiryType { get; set; }
    public TrackingUnit TrackingUnitType { get; set; }
    public ActivityType ActivityType { get; set; }
    public int InquiryTypeOrderWeight { get; set; }

    public bool IsActive { get; set; }

    public bool ProhibitOnProbationPeriod { get; set; }
    public bool RequireAttachments { get; set; }
    public bool IncludeHolidays { get; set; }
    public bool IncludeWeekends { get; set; }
    public AccrualType? AccrualType { get; set; }
    public bool? IsStaked { get; set; }
    public bool IsDefault { get; set; }
    public int? PeopleForceLeaveTypeId { get; set; }
    public string? Comment { get; set; }

    public List<InquiryTemplateAgreementEntity> InquiryTemplateAgreements { get; set; } = [];
    public List<UserInquiryEntity> UserInquiryAgreementAssigns { get; set; } = [];
    public List<UserInquiryTemplateEntity> InquiryTemplateAssigns { get; set; } = [];
    public List<InquiryTemplateAcceptPolicyEntity> InquiryTemplateAcceptPolicyPolicies { get; set; } = [];

    public InquiriesUserEntity? CreatedByUser { get; set; }
    public InquiriesUserEntity? UpdatedByUser { get; set; }
}

internal class InquiryTemplateConfiguration : IEntityTypeConfiguration<InquiryTemplateEntity>
{
    public void Configure(EntityTypeBuilder<InquiryTemplateEntity> builder)
    {
        builder.ToTable("InquiryTemplates");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);

        builder.Property(x => x.InquiryType).IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(InquiryType.Unknown);

        builder.Property(x => x.InquiryType).IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(InquiryType.Unknown);

        builder.Property(agreementTemplate => agreementTemplate.AccrualType)
            .IsRequired(false)
            .HasConversion<string>();

        builder.Property(agreementTemplate => agreementTemplate.IsStaked).IsRequired(false);

        builder.Property(x => x.TrackingUnitType).IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.ActivityType).IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.InquiryTypeOrderWeight)
            .IsRequired()
            .HasComputedColumnSql(
                """CASE "InquiryType" WHEN 'Vacation' THEN 100 WHEN 'SickDay' THEN 200 WHEN 'SickLeave' THEN 300 WHEN 'OwnExpense' THEN 400 WHEN 'HomeOffice' THEN 500 WHEN 'WeekendTransfer' THEN 600 WHEN 'MilitaryEnlistmentOffice' THEN 700 WHEN 'SocialVacation' THEN 800 WHEN 'Documents' THEN 900 WHEN 'OtherType' THEN 1000 WHEN 'Unknown' THEN 1100 ELSE 1100 END""",
                stored: true);

        builder.Property(x => x.IsActive).IsRequired();

        builder.Property(x => x.RequireAttachments).IsRequired();

        builder.Property(x => x.IncludeHolidays).IsRequired();

        builder.Property(x => x.IncludeWeekends).IsRequired();

        builder.Property(x => x.ProhibitOnProbationPeriod).IsRequired();

        builder.Property(x => x.PeopleForceLeaveTypeId).IsRequired(false);

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
    }
}
