using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

internal class TeamBoardUser
{
    public int UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? FirstNameRu { get; set; }
    public string? LastNameRu { get; set; }
    public string? PatronymicRu { get; set; }
    public string? Email { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? DepartmentId { get; set; }
    public TeamboardDepartment? Department { get; set; }
    public int? CountryId { get; set; }
    public Country? Country { get; set; }
    public bool IsActive { get; set; }
    public string? Location { get; set; }
    public string? Position { get; set; }
    public int? PositionOrderWeight { get; set; }
    public string? CurrentLocation { get; set; }

    public string? AvatarColor { get; set; }
    public string? AvatarFileId { get; set; }

    public string? SeniorityLevelId { get; set; }
    public SeniorityLevel? SeniorityLevel { get; set; }

    public string? EnglishLevelId { get; set; }
    public EnglishLevel? EnglishLevel { get; set; }

    public string? Comments { get; set; }
    public DateOnly? HireDate { get; set; }
    public DateOnly? CareerStartedAt { get; set; }
    public DateOnly? FirstProjectInCompanyStartedAtUtc { get; set; }

    public string? PersonalEmail { get; set; }
    public string? PersonalEmailSecond { get; set; }
    public string? PersonalPhone { get; set; }
    public string? Telegram { get; set; }
    public string? LinkedIn { get; set; }

    public MilitaryServiceStatus? MilitaryServiceStatus { get; set; }
    public bool? IsGraduateJobPlacement { get; set; }

    public int? ResourceGroupId { get; set; }
    public TeamBoardResourceGroup? ResourceGroup { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
    public int? UpdatedById { get; set; }
    public TeamBoardUser? UpdatedByUser { get; set; }

    public DateTime? SynchronizedAtUtc { get; set; }

    public int? CreatedById { get; set; }

    public DateOnly? NdaStartDate { get; set; }
    public DateOnly? NdaEndDate { get; set; }

    public List<UserSalary> SalaryDetails { get; set; } = [];
    public List<UserContract> Contracts { get; set; } = [];
    public List<UserSoftSkill> UserSoftSkills { get; set; } = [];
    public List<UserSoftSkillsSnapshot> UserSoftSkillsSnapshots { get; set; } = [];
    public List<UserSpecialization> UserSpecializations { get; set; } = [];
    public List<UserInterview> UserInterviews { get; set; } = [];
    public List<UserInterview> ManagedUserInterviews { get; set; } = [];
    public List<UserPerformanceReview> UserPerformanceReviews { get; set; } = [];
    public List<ProjectUser> UserProjects { get; set; } = [];
    public List<EstimatedUserMonthPayload> EstimatedUserMonthPayloads { get; set; } = [];
    public List<UserPayloadDetails> UserPayloadDetails { get; set; } = [];
    public List<RegisteredLocationHistory> RegisteredLocations { get; set; } = [];
    public List<UserEducationEntity> UserEducations { get; set; } = [];
    public List<Payslip> Payslips { get; set; } = [];
    public List<ProjectExit>  ProjectExits { get; set; } = [];
    public List<InternshipDetail> InternshipDetails { get; set; } = [];

    public UserScore? UserScore { get; set; }
}

internal class TeamBoardUserConfiguration : IEntityTypeConfiguration<TeamBoardUser>
{
    public void Configure(EntityTypeBuilder<TeamBoardUser> builder)
    {
        builder.ToTable("TeamBoardUsers");

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId).ValueGeneratedNever();

        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(250);
        builder.Property(x => x.FirstNameRu).IsRequired(false).HasMaxLength(250);

        builder.Property(x => x.LastName).IsRequired().HasMaxLength(250);
        builder.Property(x => x.LastNameRu).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.PatronymicRu).IsRequired(false).HasMaxLength(250);

        builder.Property(x => x.Email).IsRequired(false).HasMaxLength(550);

        builder.Property(x => x.DepartmentId).IsRequired(false).HasMaxLength(64);

        builder.Property(x => x.Position).IsRequired(false).HasMaxLength(64);

        builder.Property(x => x.PositionOrderWeight).IsRequired(false);

        builder.HasOne(x => x.SeniorityLevel)
            .WithMany()
            .HasForeignKey(x => x.SeniorityLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.EnglishLevel)
            .WithMany()
            .HasForeignKey(x => x.EnglishLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Comments).IsRequired(false).HasMaxLength(1024);

        builder.Property(x => x.CareerStartedAt).IsRequired(false);
        builder.Property(x => x.FirstProjectInCompanyStartedAtUtc).IsRequired(false);

        builder.Property(x => x.PersonalEmail).IsRequired(false).HasMaxLength(550);
        builder.Property(x => x.PersonalEmailSecond).IsRequired(false).HasMaxLength(550);

        builder.Property(x => x.PersonalPhone).IsRequired(false).HasMaxLength(15);

        builder.Property(x => x.Telegram).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.LinkedIn).IsRequired(false).HasMaxLength(550);

        builder.Property(x => x.UpdatedAtUtc).IsRequired(false);

        builder.Property(x => x.HireDate).IsRequired(false);

        builder.Property(x => x.DateOfBirth).IsRequired(false);

        builder.Property(x => x.CurrentLocation).IsRequired(false).HasMaxLength(250);
        builder.Property(x => x.IsGraduateJobPlacement).IsRequired(false);
        builder.Property(x => x.MilitaryServiceStatus).IsRequired(false).HasConversion<string>();

        builder.HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Location).HasConversion<string>();

        builder.Property(x => x.CreatedById).IsRequired(false);

        builder.Property(x => x.NdaStartDate).IsRequired(false);
        builder.Property(x => x.NdaEndDate).IsRequired(false);

        builder.HasIndex(x => new
        {
            x.IsActive,
            x.DepartmentId
            // x.ResourceGroupId
        });

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ResourceGroup)
            .WithMany()
            .HasForeignKey(x => x.ResourceGroupId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
