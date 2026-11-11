using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.DataAccess.Npgsql;

internal class InquiriesDbContext : DbContext, IInquiriesDbContext
{
    public const string Schema = "inquiries";

    public DbSet<InquiriesUserEntity> InquiriesUsers { get; set; }
    public DbSet<CalendarExclusionEntity> CalendarExclusions { get; set; }
    public DbSet<InquiryTemplateEntity> InquiryTemplates { get; set; }
    public DbSet<InquiryTemplateAcceptPolicyEntity> InquiryTemplateAcceptPolicies { get; set; }
    public DbSet<UserInquiryEntity> UserInquiries { get; set; }
    public DbSet<UserInquiryDateEntity> UserInquiryDates { get; set; }
    public DbSet<UserInquiryCommentEntity> UserInquiryComments { get; set; }
    public DbSet<UserInquiryFileEntity> UserInquiryFiles { get; set; }
    public DbSet<UserInquiryAcceptEntity> UserInquiryAccepts { get; set; }
    public DbSet<UserAgreementEntity> UserAgreements { get; set; }
    public DbSet<UserAgreementTransactionEntity> UserAgreementTransactions { get; set; }
    public DbSet<InquiryTemplateAgreementEntity> InquiryTemplateAgreements { get; set; }
    public DbSet<UserInquiryTemplateEntity> UserInquiryTemplates { get; set; }
    public DbSet<UserInquiryCommentFileEntity> UserInquiryCommentFiles { get; set; }
    public InquiriesDbContext(DbContextOptions<InquiriesDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InquiriesDbContext).Assembly);
    }
}
