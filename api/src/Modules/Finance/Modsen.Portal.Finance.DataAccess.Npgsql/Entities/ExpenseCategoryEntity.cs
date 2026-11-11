using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

internal class ExpenseCategoryEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int OrderWeight { get; set; }
    public required bool IsDeleted { get; set; }

    public List<ExpenseEntity> Expenses { get; set; } = [];

}

internal class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategoryEntity>
{
    public void Configure(EntityTypeBuilder<ExpenseCategoryEntity> builder)
    {
        builder.ToTable("ExpenseCategories");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");

        builder.Property(x => x.OrderWeight).IsRequired();

        builder.Property(x => x.IsDeleted).IsRequired().HasDefaultValue(false);
    }
}
