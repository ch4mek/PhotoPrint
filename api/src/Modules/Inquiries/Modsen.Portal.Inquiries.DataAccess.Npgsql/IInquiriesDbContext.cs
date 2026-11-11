using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

[assembly: InternalsVisibleTo("Modsen.Portal.Inquiries.Requests")]
namespace Modsen.Portal.Inquiries.DataAccess.Npgsql;


internal interface IInquiriesDbContext
{
    public DbSet<InquiriesUserEntity> InquiriesUsers { get; }
    public DbSet<CalendarExclusionEntity> CalendarExclusions { get; }
    public DbSet<InquiryTemplateEntity> InquiryTemplates { get; }
    public DbSet<InquiryTemplateAcceptPolicyEntity> InquiryTemplateAcceptPolicies { get; }
    public DbSet<UserInquiryEntity> UserInquiries { get; }
    public DbSet<UserInquiryDateEntity> UserInquiryDates { get; }
    public DbSet<UserInquiryCommentEntity> UserInquiryComments { get; }
    public DbSet<UserInquiryFileEntity> UserInquiryFiles { get; }
    public DbSet<UserInquiryAcceptEntity> UserInquiryAccepts { get; }
    public DbSet<UserAgreementEntity> UserAgreements { get; }
    public DbSet<UserAgreementTransactionEntity> UserAgreementTransactions { get; }
    public DbSet<InquiryTemplateAgreementEntity> InquiryTemplateAgreements { get; }
    public DbSet<UserInquiryTemplateEntity> UserInquiryTemplates { get; }
    public DbSet<UserInquiryCommentFileEntity> UserInquiryCommentFiles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
