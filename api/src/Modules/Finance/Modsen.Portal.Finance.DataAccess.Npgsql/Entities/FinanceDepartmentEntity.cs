using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

internal class FinanceDepartmentEntity
{
    public required string DepartmentId { get; set; }
    public required string ShortName { get; set; }
    public required string FullName { get; set; }
    public int DepartmentOrderWeight { get; set; }
}

internal class FinanceDepartmentConfiguration : IEntityTypeConfiguration<FinanceDepartmentEntity>
{
    public void Configure(EntityTypeBuilder<FinanceDepartmentEntity> builder)
    {
        builder.ToTable("FinanceDepartments");

        builder.HasKey(x => x.DepartmentId);
        builder.Property(x => x.DepartmentId).ValueGeneratedNever();
        builder.Property(x => x.DepartmentId).HasMaxLength(64).IsRequired();

        builder.Property(x => x.ShortName).IsRequired().HasMaxLength(32);

        builder.Property(x => x.FullName).IsRequired().HasMaxLength(128);

        builder.Property(x => x.DepartmentOrderWeight).IsRequired();

    }
}
