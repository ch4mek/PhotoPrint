using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Outbox.Storage;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using UserSpecialization = Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities.UserSpecialization;

[assembly: InternalsVisibleTo("Modsen.Portal.Api")]
namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql;

internal class TeamBoardDbContext : DbContext, ITeamBoardDbContext
{
    public const string Schema = "teamboard";

    public DbSet<Country> Countries { get; set; }
    public DbSet<RegisteredLocationHistory> RegisteredLocationHistory { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<DevelopmentPlan> DevelopmentPlans { get; set; }
    public DbSet<EstimatedUserMonthPayload> EstimatedUserMonthPayloads { get; set; }
    public DbSet<EnglishLevel> EnglishLevels { get; set; }
    public DbSet<ExternalFeedback> ExternalFeedbacks { get; set; }
    public DbSet<HardSkill> HardSkills { get; set; }
    public DbSet<HardSkillGroup> HardSkillGroups { get; set; }
    public DbSet<ProjectExitClosingReason> ProjectExitClosingReasons { get; set; }
    public DbSet<ProjectExit> ProjectExits { get; set; }
    public DbSet<ProjectUser> ProjectUsers { get; set; }
    public DbSet<SeniorityLevel> SeniorityLevels { get; set; }
    public DbSet<SoftSkill> SoftSkills { get; set; }
    public DbSet<SpecializationTemplate> SpecializationTemplates { get; set; }
    public DbSet<SpecializationTemplateDepartment> SpecializationTemplateDepartments { get; set; }
    public DbSet<SpecializationTemplateHardSkill> SpecializationTemplateHardSkills { get; set; }
    public DbSet<TeamBoardProject> TeamBoardProjects { get; set; }
    public DbSet<TeamBoardUser> TeamBoardUsers { get; set; }
    public DbSet<UserContract> UserContracts { get; set; }
    public DbSet<TeamboardDepartment> TeamboardDepartments { get; set; }
    public DbSet<UserInterview> UserInterviews { get; set; }
    public DbSet<UserInterviewFeedback> UserInterviewFeedbacks { get; set; }
    public DbSet<UserPerformanceReview> UserPerformanceReviews { get; set; }
    public DbSet<UserPerformanceReviewGoal> UserPerformanceReviewGoals { get; set; }
    public DbSet<UserSalary> UserSalaries { get; set; }
    //add soft skills snapshot
    public DbSet<UserSoftSkillsSnapshot> UserSoftSkillsSnapshots { get; set; }
    public DbSet<UserSoftSkill> UserSoftSkills { get; set; }
    public DbSet<UserSpecialization> UserSpecializations { get; set; }
    public DbSet<UserSpecializationHardSkill> UserSpecializationHardSkills { get; set; }
    public DbSet<WorkingHoursPerMonth> WorkingHoursPerMonths { get; set; }

    public DbSet<UserSkillsSnapshot> UserSkillsSnapshots { get; set; }
    public DbSet<UserSkillsSnapshotSpecialization> UserSkillsSnapshotSpecializations { get; set; }
    public DbSet<UserSkillsSnapshotSpecializationHardSkill> UserSkillsSnapshotSpecializationHardSkills { get; set; }
    public DbSet<UserPayloadDetails> UserPayloadDetails { get; set; }
    public DbSet<TeamboardDepartment> TeamBoardDepartments { get; set; }
    public DbSet<TeamBoardResourceGroup> TeamBoardResourceGroups { get; set; }
    public DbSet<Payslip> Payslips { get; set; }
    public DbSet<TeamBoardClient> TeamBoardClients { get; set; }

    public DbSet<KpiColor> KpiColors { get; set; }
    public DbSet<UserEducationEntity> UserEducations { get; set; }

    public DbSet<ConversationDocumentLink> ConversationDocumentLinks { get; set; }
    public DbSet<ConversationFile> ConversationFiles { get; set; }
    public DbSet<InternshipDetail> InternshipDetails { get; set; }
    public DbSet<InternshipSpecialization> InternshipSpecializations { get; set; }
    public DbSet<InternshipSpecializationHardSkill> InternshipSpecializationHardSkills { get; set; }


    public DbSet<UserScore> UserScores { get; set; }

    public TeamBoardDbContext(DbContextOptions<TeamBoardDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TeamBoardDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new OutboxEventConfiguration("OutboxEvents", Schema));
        modelBuilder.HasDbFunction(
                typeof(TeamboardDbFunctions).GetMethod(nameof(TeamboardDbFunctions.GetUserGeneralSeniority),
                    [typeof(int)])!)
            .HasName("get_user_general_seniority");
    }
}
