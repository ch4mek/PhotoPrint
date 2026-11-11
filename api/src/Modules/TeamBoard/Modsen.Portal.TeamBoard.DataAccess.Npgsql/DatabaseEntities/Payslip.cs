using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class Payslip
{
    public required string FileId { get; set; }
    public required string FileName { get; set; }
    public DateOnly ReportDate { get; set; }
    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

internal class PayslipConfiguration : IEntityTypeConfiguration<Payslip>
{
    public void Configure(EntityTypeBuilder<Payslip> builder)
    {
        builder.ToTable("Payslips");

        builder.HasKey(x => x.FileId);
        builder.Property(x => x.FileId)
            .HasMaxLength(64)
            .ValueGeneratedNever();

        builder.Property(x => x.FileName)
            .HasMaxLength(600)
            .IsRequired();

        builder.Property(x => x.ReportDate)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Payslips)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
    }
}
