using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.Database.Entities;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class UserEducationEntity : IAuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public TeamBoardUser? User { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public required string EducationalInstitution { get; set; }
    public required string Faculty { get; set; }
    public required string Specialization { get; set; }
    public StudyForm StudyForm { get; set; }
    public int GraduationYear { get; set; }
    public string? Comment { get; set; }

    public TeamBoardUser? CreatedByUser { get; set; }
    public int? CreatedById { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public TeamBoardUser? UpdatedByUser { get; set; }
    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public int EducationLevelOrderWeight { get; set; }
    public int StudyFormOrderWeight { get; set; }
}

internal class EducationConfiguration : IEntityTypeConfiguration<UserEducationEntity>
{
    public void Configure(EntityTypeBuilder<UserEducationEntity> builder)
    {
        builder.ToTable("UserEducations",
            table =>
            {
                table.HasCheckConstraint("CK_Educations_GraduationYear",
                    "\"GraduationYear\" <= 9999 AND \"GraduationYear\" > 0");
            });

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserEducations)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.EducationLevel).IsRequired();
        builder.Property(x => x.EducationLevel).HasConversion(new EnumConverter<EducationLevel>());

        builder.Property(x => x.EducationalInstitution).HasMaxLength(250).IsRequired();

        builder.Property(x => x.Faculty).HasMaxLength(250).IsRequired();

        builder.Property(x => x.Specialization).HasMaxLength(250).IsRequired();

        builder.Property(x => x.StudyForm).IsRequired();
        builder.Property(x => x.StudyForm).HasConversion(new EnumConverter<StudyForm>());

        builder.Property(x => x.GraduationYear).IsRequired();

        builder.Property(x => x.Comment).IsRequired(false).HasMaxLength(1000);

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

        builder.Property(x => x.EducationLevelOrderWeight).IsRequired();
        builder.Property(x => x.StudyFormOrderWeight).IsRequired();
    }
}



