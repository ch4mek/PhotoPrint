using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using UserSpecialization = Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities.UserSpecialization;

[assembly: InternalsVisibleTo("Modsen.Portal.IntegrationTests")]
[assembly: InternalsVisibleTo("Modsen.Portal.TeamBoard.Requests")]
namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql;

internal interface ITeamBoardDbContext
{
    public DbSet<Country> Countries { get; }
    public DbSet<RegisteredLocationHistory> RegisteredLocationHistory { get; }
    public DbSet<Conversation> Conversations { get; }
    public DbSet<ConversationDocumentLink> ConversationDocumentLinks { get; }
    public DbSet<ConversationFile> ConversationFiles { get; }
    public DbSet<Currency> Currencies { get; }
    public DbSet<DevelopmentPlan> DevelopmentPlans { get; }
    public DbSet<EstimatedUserMonthPayload> EstimatedUserMonthPayloads { get; }
    public DbSet<EnglishLevel> EnglishLevels { get; }
    public DbSet<ExternalFeedback> ExternalFeedbacks { get; }
    public DbSet<HardSkill> HardSkills { get; }
    public DbSet<HardSkillGroup> HardSkillGroups { get; }
    public DbSet<ProjectExitClosingReason> ProjectExitClosingReasons { get; }
    public DbSet<ProjectExit> ProjectExits { get; }
    public DbSet<ProjectUser> ProjectUsers { get; }
    public DbSet<SeniorityLevel> SeniorityLevels { get; }
    public DbSet<SoftSkill> SoftSkills { get; }
    public DbSet<SpecializationTemplate> SpecializationTemplates { get; }
    public DbSet<SpecializationTemplateDepartment> SpecializationTemplateDepartments { get; }
    public DbSet<SpecializationTemplateHardSkill> SpecializationTemplateHardSkills { get; }
    public DbSet<TeamBoardProject> TeamBoardProjects { get; }
    public DbSet<TeamBoardUser> TeamBoardUsers { get; }
    public DbSet<TeamboardDepartment> TeamboardDepartments { get; }
    public DbSet<UserContract> UserContracts { get; }
    public DbSet<UserInterview> UserInterviews { get; }
    public DbSet<UserInterviewFeedback> UserInterviewFeedbacks { get; }
    public DbSet<UserPerformanceReview> UserPerformanceReviews { get; }
    public DbSet<UserPerformanceReviewGoal> UserPerformanceReviewGoals { get; }
    public DbSet<UserSalary> UserSalaries { get; }
    public DbSet<UserSoftSkillsSnapshot> UserSoftSkillsSnapshots { get; set; }
    public DbSet<UserSoftSkill> UserSoftSkills { get; }
    public DbSet<UserSpecialization> UserSpecializations { get; }
    public DbSet<UserSpecializationHardSkill> UserSpecializationHardSkills { get; }
    public DbSet<WorkingHoursPerMonth> WorkingHoursPerMonths { get; }
    public DbSet<UserSkillsSnapshotSpecializationHardSkill> UserSkillsSnapshotSpecializationHardSkills { get; set; }
    public DbSet<TeamBoardResourceGroup> TeamBoardResourceGroups { get; set; }
    public DbSet<Payslip> Payslips { get; set; }

    public DbSet<UserSkillsSnapshot> UserSkillsSnapshots { get; set; }
    public DbSet<UserSkillsSnapshotSpecialization> UserSkillsSnapshotSpecializations { get; set; }

    public DbSet<UserPayloadDetails> UserPayloadDetails { get; set; }

    public DbSet<KpiColor> KpiColors { get; set; }
    public DbSet<UserEducationEntity> UserEducations { get; set; }

    public DbSet<TeamBoardClient> TeamBoardClients { get; set; }

    public DbSet<InternshipDetail> InternshipDetails { get; set; }

    public DbSet<InternshipSpecialization> InternshipSpecializations { get; set; }
    public DbSet<InternshipSpecializationHardSkill> InternshipSpecializationHardSkills { get; set; }

    public DbSet<UserScore> UserScores { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
