using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class UserSalary : IPortalEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }
    public DateOnly SalaryFrom { get; set; }
    public required string SalaryUsd { get; set; }
    public required string SalaryInCurrency { get; set; }
    public required string SalaryCurrencyId { get; set; }
    public Currency? SalaryCurrency { get; set; }
    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public UserSalarySource Source { get; set; }

    public int? ConversationId { get; set; }
    public Conversation? Conversation { get; set; }
}

internal class UserSalaryConfiguration : IEntityTypeConfiguration<UserSalary>
{
    public void Configure(EntityTypeBuilder<UserSalary> builder)
    {
        builder.ToTable("UserSalaries");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany(x => x.SalaryDetails)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.SalaryFrom).IsRequired();

        builder.Property(x => x.SalaryUsd).IsRequired();

        builder.Property(x => x.SalaryInCurrency).IsRequired();

        builder.Property(x => x.SalaryCurrencyId).IsRequired().HasMaxLength(3);
        builder.HasOne(x => x.SalaryCurrency)
            .WithMany()
            .HasForeignKey(x => x.SalaryCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.Property(x => x.Source).IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(UserSalarySource.Unknown);

        builder.HasOne(x => x.Conversation)
            .WithMany(x => x.UserSalaries)
            .HasForeignKey(x => x.ConversationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
