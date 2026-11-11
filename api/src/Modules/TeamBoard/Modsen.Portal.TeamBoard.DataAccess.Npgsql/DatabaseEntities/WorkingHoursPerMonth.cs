using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class WorkingHoursPerMonth
{
    public int Id { get; set; }
    public int Year { get; set; }
    public int MonthOfYear { get; set; }

    public int CountryId { get; set; }
    public Country? Country { get; set; }

    public int Hours { get; set; }
}

internal class WorkingHoursPerMonthConfiguration : IEntityTypeConfiguration<WorkingHoursPerMonth>
{
    public void Configure(EntityTypeBuilder<WorkingHoursPerMonth> builder)
    {
        builder.ToTable("WorkingHoursPerMonths");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
            
        builder.HasOne(x => x.Country)
            .WithMany()
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Year).IsRequired();

        builder.Property(x => x.MonthOfYear).IsRequired();

        builder.Property(x => x.Hours).IsRequired();

        builder.HasIndex(x => new { x.Year, x.MonthOfYear, x.CountryId })
            .IsUnique();
    }
}
