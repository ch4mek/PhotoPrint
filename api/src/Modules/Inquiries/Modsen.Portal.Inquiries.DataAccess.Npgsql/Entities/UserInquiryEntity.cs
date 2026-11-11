using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class UserInquiryEntity : AuditableEntity
{
    public static readonly IReadOnlyCollection<InquiryType> InquiryTypesThatCanAccrue =
    [
        InquiryType.Vacation,
        InquiryType.SickDay,
    ];

    public static readonly IReadOnlyCollection<UserInquiryStatus> InquiryStatusesThatCanBeViewedOnCalendar =
    [
        UserInquiryStatus.Open,
        UserInquiryStatus.Approved
    ];

    public int Id { get; set; }

    public int UserId { get; set; }
    public InquiriesUserEntity? User { get; set; }

    public int? AgreementTemplateId { get; set; }
    public InquiryTemplateAgreementEntity? AgreementTemplate { get; set; }

    public int InquiryTemplateId { get; set; }
    public InquiryTemplateEntity? InquiryTemplate { get; set; }

    public int CountryId { get; set; }
    public UserInquiryStatus Status { get; set; }
    public int StatusOrderWeight { get; set; }
    public string? Notes { get; set; }
    public DateOnly DateFrom { get; set; }
    public DateOnly DateTo { get; set; }
    public decimal? TotalHours { get; set; }
    public decimal? TotalDays { get; set; }
    public decimal? TotalAccountantDays { get; set; }
    public decimal? CompensationAmount { get; set; }
    public int? PeopleForceId { get; set; }

    public decimal? HistoricalRemainingDays { get; set; }
    public decimal? HistoricalBalanceDays { get; set; }
    public decimal? HistoricalBalanceHours { get; set; }
    public int? HistoricalManagerId { get; set; }
    public InquiriesUserEntity? HistoricalManager { get; set; }
    public string? HistoricalManagerPosition { get; set; }
    public string? HistoricalUserPosition { get; set; }
    public string? HistoricalCreatedByUserPosition { get; set; }

    public List<UserInquiryDateEntity> UserInquiryDates { get; set; } = [];
    public List<UserInquiryCommentEntity> UserInquiryComments { get; set; } = [];
    public List<UserInquiryFileEntity> UserInquiryFiles { get; set; } = [];
    public List<UserInquiryAcceptEntity> UserInquiryAccepts { get; set; } = [];
    public List<UserAgreementTransactionEntity> UserAgreementTransactions { get; set; } = [];

    public InquiriesUserEntity? CreatedByUser { get; set; }
    public InquiriesUserEntity? UpdatedByUser { get; set; }
}

internal class UserInquiryConfiguration : IEntityTypeConfiguration<UserInquiryEntity>
{
    public void Configure(EntityTypeBuilder<UserInquiryEntity> builder)
    {
        builder.ToTable("UserInquiries");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.CountryId).IsRequired();

        builder.Property(x => x.Status).IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.StatusOrderWeight)
            .HasComputedColumnSql(
            "CASE `Status` WHEN 'Approved' THEN 0 WHEN 'Canceled' THEN 50 WHEN 'Rejected' THEN 100 WHEN 'Open' THEN 200 ELSE 200 END", true);

        builder.Property(x => x.Notes).IsRequired(false);

        builder.Property(x => x.DateFrom).IsRequired();

        builder.Property(x => x.DateTo).IsRequired();

        builder.Property(x => x.TotalHours).IsRequired(false);
        builder.Property(x => x.TotalDays).IsRequired(false);
        builder.Property(x => x.TotalAccountantDays).IsRequired(false);
        builder.Property(x => x.HistoricalRemainingDays).IsRequired(false);
        builder.Property(x => x.HistoricalBalanceDays).IsRequired(false);
        builder.Property(x => x.HistoricalBalanceHours).IsRequired(false);
        builder.Property(x => x.HistoricalManagerId).IsRequired(false);
        builder.Property(x => x.HistoricalManagerPosition).IsRequired(false);
        builder.Property(x => x.HistoricalCreatedByUserPosition).IsRequired(false);
        builder.Property(x => x.HistoricalUserPosition).IsRequired(false);

        builder.Property(x => x.CompensationAmount).IsRequired(false);

        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
        builder.Property(x => x.UpdatedById).IsRequired(false);

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.InquiryTemplateId).IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserInquiries)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.InquiryTemplate)
            .WithMany(x => x.UserInquiryAgreementAssigns)
            .HasForeignKey(x => x.InquiryTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AgreementTemplate)
            .WithMany(x => x.UserInquiryAgreementAssigns)
            .HasForeignKey(x => x.AgreementTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.HistoricalManager)
            .WithMany()
            .HasForeignKey(x => x.HistoricalManagerId)
            .IsRequired(false);

        builder.HasIndex(x => x.PeopleForceId);
    }
}
