using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class UserContract : IAuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public TeamBoardUser? User { get; set; }

    public DateOnly ContractFrom { get; set; }
    public DateOnly? ContractTo { get; set; }

    public DateOnly? FireDate { get; set; }
    public FireType? FireType { get; set; }
    public string? FireReason { get; set; }
    public string? FireDescription { get; set; }

    public bool IsProbationPeriod { get; set; }
    public bool IsActive { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? UpdatedById { get; set; }
    public TeamBoardUser? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
}

internal class UserContractConfiguration : IEntityTypeConfiguration<UserContract>
{
    public void Configure(EntityTypeBuilder<UserContract> builder)
    {
        builder.ToTable("UserContracts");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany(x => x.Contracts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.ContractFrom).IsRequired();

        builder.Property(x => x.ContractTo).IsRequired(false);

        builder.Property(x => x.FireDate).IsRequired(false);

        builder.Property(x => x.FireType).IsRequired(false).HasConversion<string>();

        builder.Property(x => x.FireReason).IsRequired(false);

        builder.Property(x => x.FireDescription).IsRequired(false);

        builder.Property(x => x.IsProbationPeriod).IsRequired();

        builder.Property(x => x.IsActive);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
    }
}
