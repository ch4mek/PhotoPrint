using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class UserSkillsSnapshot : IAuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }
    public bool IsDeleted { get; set; }
    public DateOnly SnapshotDate { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public int? UpdatedById { get; set; }
    public TeamBoardUser? UpdatedByUser { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<UserSkillsSnapshotSpecialization> Specializations { get; set; } = [];
}

internal class UserSkillsSnapshotConfiguration : IEntityTypeConfiguration<UserSkillsSnapshot>
{
    public void Configure(EntityTypeBuilder<UserSkillsSnapshot> builder)
    {
        builder.ToTable("UserSkillsSnapshots", TeamBoardDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);
    }
}
