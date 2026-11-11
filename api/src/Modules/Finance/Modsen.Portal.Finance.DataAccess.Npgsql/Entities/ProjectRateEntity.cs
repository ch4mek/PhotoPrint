using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

internal class ProjectRateEntity
{
    public int Id { get; set; }
    public required int UserId { get; set; }
    public FinanceUserEntity? User { get; set; }
    public required int ProjectId { get; set; }
    public FinanceProjectEntity? Project { get; set; }
    public required string HourlyRate { get; set; }
    public required string CurrencyId { get; set; }
    public CurrencyEntity? Currency { get; set; }
    public required DateOnly RateFrom { get; set; }
    public DateOnly? RateTo { get; set; }
    public bool IsDeleted { get; set; }
}

internal class ProjectRateConfiguration : IEntityTypeConfiguration<ProjectRateEntity>
{
    public void Configure(EntityTypeBuilder<ProjectRateEntity> builder)
    {
        builder.ToTable("ProjectRates");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.UserId).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.ProjectRates)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ProjectId).IsRequired();
        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.HourlyRate).IsRequired();

        builder.Property(x => x.CurrencyId).IsRequired().HasMaxLength(3);
        builder.HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.CurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.RateFrom).IsRequired();
        builder.Property(x => x.RateTo).IsRequired(false);

        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
    }
}
