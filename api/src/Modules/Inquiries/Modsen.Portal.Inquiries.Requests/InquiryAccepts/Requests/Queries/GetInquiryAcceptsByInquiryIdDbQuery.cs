using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests.Queries;

internal interface IGetInquiryAcceptsByInquiryIdDbQuery
{
    Task<List<UserInquiryAcceptEntity>> ExecuteAsync(int inquiryId, CancellationToken cancellationToken);
}

internal class GetInquiryAcceptsByInquiryIdDbQuery : IGetInquiryAcceptsByInquiryIdDbQuery
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public GetInquiryAcceptsByInquiryIdDbQuery(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<List<UserInquiryAcceptEntity>> ExecuteAsync(int inquiryId, CancellationToken cancellationToken)
    {
        return await _inquiriesDbContext.UserInquiryAccepts
            .Where(accept => accept.UserInquiryId == inquiryId)
            .ToListAsync(cancellationToken);
    }
}
