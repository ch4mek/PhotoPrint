using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Inquiries;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

internal class CalendarExclusionEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public DateOnly Date { get; set; }
    public DateOnly? NewDate { get; set; }
    public DateType DateType { get; set; }
    public int CountryId { get; set; }

    public List<UserInquiryDateEntity> UserInquiryDates { get; set; } = [];
}

internal class CalendarExclusionConfiguration : IEntityTypeConfiguration<CalendarExclusionEntity>
{
    public void Configure(EntityTypeBuilder<CalendarExclusionEntity> builder)
    {
        builder.ToTable("CalendarExclusions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired(false).HasMaxLength(250);

        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.NewDate).IsRequired(false);
        builder.Property(x => x.DateType).IsRequired().HasConversion<string>();

        builder.Property(x => x.CountryId).IsRequired();

        builder.HasIndex(x => new
        {
            x.CountryId,
            x.Date
        }).IsUnique();
    }
}
