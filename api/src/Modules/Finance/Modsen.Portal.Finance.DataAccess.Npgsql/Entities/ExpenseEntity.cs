using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

internal class ExpenseEntity
{
    public int Id { get; set; }
    public required int ExpenseCategoryId { get; set; }
    public ExpenseCategoryEntity? ExpenseCategory { get; set; }
    public required int Year { get; set; }
    public required int MonthOfYear { get; set; }
    public required string ExpenseAmount { get; set; }
    public required bool IsDeleted { get; set; }

}

internal class ExpenseConfiguration : IEntityTypeConfiguration<ExpenseEntity>
{
    public void Configure(EntityTypeBuilder<ExpenseEntity> builder)
    {
        builder.ToTable("Expenses");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.ExpenseCategory)
            .WithMany(x => x.Expenses)
            .HasForeignKey(x => x.ExpenseCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Year).IsRequired();
        builder.Property(x => x.MonthOfYear).IsRequired();

        builder.Property(x => x.ExpenseAmount).IsRequired();

        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
    }
}
