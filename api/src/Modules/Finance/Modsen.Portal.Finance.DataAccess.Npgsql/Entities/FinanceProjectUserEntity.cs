using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modsen.Portal.Finance.DataAccess.Npgsql.Entities;

internal class FinanceProjectUserEntity
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public FinanceProjectEntity? Project { get; set; }
    public int UserId { get; set; }
    public FinanceUserEntity? User { get; set; }    

    public DateOnly UserOnProjectFrom { get; set; }
    public DateOnly? UserOnProjectTo { get; set; }

    public bool IsTimeOnProjectBillable { get; set; }
}

internal class FinanceProjectUserConfiguration : IEntityTypeConfiguration<FinanceProjectUserEntity>
{
    public void Configure(EntityTypeBuilder<FinanceProjectUserEntity> builder)
    {
        builder.ToTable("FinanceProjectUsers", FinanceDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.ProjectId).IsRequired();
        builder.HasOne(x => x.Project)
            .WithMany(x => x.ProjectUsers)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UserId).IsRequired();
        builder.HasOne(x => x.User)
            .WithMany(x => x.UserProjects)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UserOnProjectFrom).IsRequired();
        builder.Property(x => x.UserOnProjectTo).IsRequired(false);

        builder.Property(x => x.IsTimeOnProjectBillable).IsRequired();
    }
}
