using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

internal class FinanceProjectEntity
{
    public int Id { get; set; }
    public required string ProjectName { get; set; }
    public required int ClientId { get; set; }
    public FinanceClientEntity? Client { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; }    
    public bool IsPublic { get; set; }

    public List<FinanceProjectUserEntity> ProjectUsers { get; set; } = [];
}

internal class FinanceProjectConfiguration : IEntityTypeConfiguration<FinanceProjectEntity>
{
    public void Configure(EntityTypeBuilder<FinanceProjectEntity> builder)
    {
        builder.ToTable("FinanceProjects");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ProjectName).HasMaxLength(250).IsRequired();

        builder.HasOne(x => x.Client)
            .WithMany(x => x.Projects)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Color).HasMaxLength(7).IsFixedLength();
    }
}
