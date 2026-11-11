using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class UserInquiryDateEntity
{
    public int Id { get; set; }

    public int? UserInquiryCalendarDetailsId { get; set; }
    public UserInquiryEntity? UserInquiryAgreementAssign { get; set; }

    public DateOnly Date { get; set; }
    public decimal RequestedDays { get; set; }
    public DateType DateType { get; set; }

    public int? CalendarExclusionId { get; set; }
    public CalendarExclusionEntity? CalendarExclusion { get; set; }
}

internal class UserInquiryDateConfiguration : IEntityTypeConfiguration<UserInquiryDateEntity>
{
    public void Configure(EntityTypeBuilder<UserInquiryDateEntity> builder)
    {
        builder.ToTable("UserInquiryDates");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Date).IsRequired();

        builder.Property(x => x.RequestedDays).IsRequired();

        builder.Property(x => x.DateType).IsRequired().HasConversion<string>();

        builder.Property(x => x.UserInquiryCalendarDetailsId).IsRequired(false);

        builder.Property(x => x.CalendarExclusionId).IsRequired(false);

        builder.HasOne(x => x.UserInquiryAgreementAssign)
            .WithMany(x => x.UserInquiryDates)
            .HasForeignKey(x => x.UserInquiryCalendarDetailsId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CalendarExclusion)
            .WithMany(x => x.UserInquiryDates)
            .HasForeignKey(x => x.CalendarExclusionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
