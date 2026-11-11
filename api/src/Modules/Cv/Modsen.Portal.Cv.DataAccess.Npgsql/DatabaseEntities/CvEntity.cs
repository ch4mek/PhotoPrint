using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Cv;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Database.Entities;

namespace Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;

internal class CvEntity : AuditableEntity
{
    public int Id { get; set; }
    public required string CvName { get; set; }

    public int UserId { get; set; }
    public CvUserEntity? CvUser { get; set; }

    public string? ProjectId { get; set; }
    public CvProjectEntity? CvProject { get; set; }

    public string? ClientId { get; set; }
    public CvClientEntity? CvClient { get; set; }

    public CvStatus Status { get; set; }
    public int StatusOrderWeight { get; set; }
    public bool IsVisibleToUser { get; set; }

    public required string UserFirstName { get; set; }
    public required string UserLastName { get; set; }
    public string? UserProjectPosition { get; set; }
    public string? UserLocation { get; set; }
    public string? UserEducation { get; set; }
    public string? UserSkills { get; set; }
    public string? UserAbout { get; set; }

    public string? Website { get; set; }
    public bool IsCorporateDesign { get; set; }

    public bool IsDeleted { get; set; }

    public CvUserEntity? CreatedByUser { get; set; }
    public CvUserEntity? UpdatedByUser { get; set; }

    public ICollection<WorkExperienceEntity> WorkExperiences { get; set; } = [];
    public ICollection<LanguageEntity> Languages { get; set; } = [];
    public ICollection<ProjectExperienceEntity> ProjectExperiences { get; set; } = [];
    public ICollection<SkillGroupEntity> SkillGroups { get; set; } = [];
}

internal class CvConfiguration : IEntityTypeConfiguration<CvEntity>
{
    public void Configure(EntityTypeBuilder<CvEntity> builder)
    {
        builder.ToTable("Cvs");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.CvName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.ProjectId)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(c => c.ClientId)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.StatusOrderWeight)
            .IsRequired()
            .HasReverseOrderEnumComputedColumn<CvEntity, CvStatus>(c => c.Status);

        builder.Property(c => c.IsVisibleToUser)
            .IsRequired();

        builder.Property(c => c.UserFirstName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(c => c.UserLastName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(c => c.UserProjectPosition)
            .IsRequired(false)
            .HasMaxLength(250);

        builder.Property(c => c.UserLocation)
            .IsRequired(false)
            .HasMaxLength(250);

        builder.Property(c => c.UserEducation)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(c => c.UserSkills)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(c => c.UserAbout)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(c => c.Website)
            .IsRequired(false)
            .HasMaxLength(250);

        builder.Property(c => c.IsCorporateDesign)
            .IsRequired();

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();

        builder.Property(c => c.UpdatedAtUtc)
            .IsRequired(false);

        builder.HasOne(kit => kit.CreatedByUser)
            .WithMany()
            .HasForeignKey(kit => kit.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(kit => kit.UpdatedByUser)
            .WithMany()
            .HasForeignKey(kit => kit.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CvUser)
            .WithMany(u => u.Cvs)
            .HasForeignKey(c => c.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CvProject)
            .WithMany(p => p.Cvs)
            .HasForeignKey(c => c.ProjectId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.CvClient)
            .WithMany(c => c.Cvs)
            .HasForeignKey(c => c.ClientId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
