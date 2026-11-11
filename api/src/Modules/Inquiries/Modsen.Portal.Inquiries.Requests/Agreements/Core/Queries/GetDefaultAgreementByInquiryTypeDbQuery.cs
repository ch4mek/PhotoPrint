using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Core.Queries;

internal interface IGetDefaultAgreementByInquiryTypeDbQuery
{
    Task<InquiryTemplateAgreementEntity> ExecuteAsync(InquiryType inquiryType, CancellationToken cancellationToken);
}

internal class GetDefaultAgreementByInquiryTypeDbQuery : IGetDefaultAgreementByInquiryTypeDbQuery
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public GetDefaultAgreementByInquiryTypeDbQuery(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<InquiryTemplateAgreementEntity> ExecuteAsync(InquiryType inquiryType, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.InquiryTemplateAgreements
            .Where(x => !x.IsDeleted)
            .Include(x => x.InquiryTemplate)
            .FirstOrDefaultAsync(x =>
                x.InquiryTemplate!.IsDefault &&
                x.InquiryTemplate.IsActive &&
                x.InquiryTemplate.InquiryType == inquiryType &&
                x.IsDefault, cancellationToken)
            ?? throw new EntityNotFoundException($"Default agreement for type {inquiryType} not found");
    }
}
