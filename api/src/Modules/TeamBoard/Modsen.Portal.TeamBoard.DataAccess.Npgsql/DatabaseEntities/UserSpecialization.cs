using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class UserSpecialization : IPortalEntity
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }

    public int SpecializationTemplateId { get; set; }
    public SpecializationTemplate? SpecializationTemplate { get; set; }

    public string? SeniorityLevelId { get; set; }
    public SeniorityLevel? SeniorityLevel { get; set; }

    public bool IsPrimary { get; set; }
    public bool IsVisibleSpecialization { get; set; }
    public bool IsVisibleSeniorityLevel { get; set; }

    public int? CreatedById { get; set; }
    public TeamBoardUser? CreatedByUser { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public List<UserSpecializationHardSkill> SpecializationHardSkills { get; set; } = [];
}

internal class UserSpecializationConfiguration : IEntityTypeConfiguration<UserSpecialization>
{
    public void Configure(EntityTypeBuilder<UserSpecialization> builder)
    {
        builder.ToTable("UserSpecializations");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasIndex(x => new
        {
            x.UserId,
            x.SpecializationTemplateId
        }).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserSpecializations)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SpecializationTemplate)
            .WithMany(x => x.UserSpecializations)
            .HasForeignKey(x => x.SpecializationTemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SeniorityLevel)
            .WithMany()
            .HasForeignKey(x => x.SeniorityLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.IsPrimary).IsRequired();

        builder.Property(x => x.IsVisibleSpecialization).IsRequired();

        builder.Property(x => x.IsVisibleSeniorityLevel).IsRequired();

        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
