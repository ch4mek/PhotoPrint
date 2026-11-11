using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

internal class FinanceClientEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? LegalEntity { get; set; }
    public bool IsActive { get; set; } = true;
    public int? SalesManagerId { get; set; }

    public List<FinanceProjectEntity> Projects { get; set; } = [];
}

internal class ClientConfiguration : IEntityTypeConfiguration<FinanceClientEntity>
{
    public virtual void Configure(EntityTypeBuilder<FinanceClientEntity> builder)
    {
        builder.ToTable("FinanceClients", FinanceDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.HasIndex(x => x.Name);

        builder.Property(x => x.LegalEntity).IsRequired(false).HasMaxLength(250);

        builder.Property(x => x.IsActive).IsRequired();

        builder.Property(x => x.SalesManagerId).IsRequired(false);
    }
}
