using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core;

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

internal class FinanceUserEntity
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? PatronymicRu { get; set; }
    public required string Email { get; set; }
    public string? DepartmentId { get; set; }
    public FinanceDepartmentEntity? Department { get; set; }
    public bool IsActive { get; set; }
    public int? ResourceGroupId { get; set; }

    public List<ProjectRateEntity> ProjectRates { get; set; } = [];
    public List<FinanceProjectUserEntity> UserProjects { get; set; } = [];
}

internal class FinanceUserConfiguration : IEntityTypeConfiguration<FinanceUserEntity>
{
    public void Configure(EntityTypeBuilder<FinanceUserEntity> builder)
    {
        builder.ToTable("FinanceUsers");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(250);

        builder.Property(x => x.LastName).IsRequired().HasMaxLength(250);

        builder.Property(x => x.Email).IsRequired().HasMaxLength(PortalSharedConstants.EmailMaxLength);

        builder.Property(x => x.DepartmentId).IsRequired(false).HasMaxLength(64);
        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
