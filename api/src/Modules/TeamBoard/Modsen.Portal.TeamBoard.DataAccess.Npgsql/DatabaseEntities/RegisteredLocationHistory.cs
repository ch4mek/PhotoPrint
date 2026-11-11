using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class RegisteredLocationHistory
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public TeamBoardUser? TeamBoardUser { get; set; }

    public int RegisteredLocationId { get; set; }
    public Country? RegisteredLocation { get; set; }

    public DateOnly DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}

internal class RegisteredLocationHistoryConfiguration : IEntityTypeConfiguration<RegisteredLocationHistory>
{
    public void Configure(EntityTypeBuilder<RegisteredLocationHistory> builder)
    {
        builder.ToTable("RegisteredLocationHistory");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.TeamBoardUser)
            .WithMany(x => x.RegisteredLocations)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.RegisteredLocation)
            .WithMany()
            .HasForeignKey(x => x.RegisteredLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.DateFrom).IsRequired();
        builder.Property(x => x.DateTo).IsRequired(false);
    }
}
