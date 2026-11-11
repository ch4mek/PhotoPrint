using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Comments.Requests.Queries;

internal interface IGetInquiryCommentByIdDbQuery
{
    Task<UserInquiryCommentEntity> Get(int inquiryId, int commentId, CancellationToken cancellationToken);
}

internal class GetInquiryCommentByIdDbQuery : IGetInquiryCommentByIdDbQuery
{
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public GetInquiryCommentByIdDbQuery(IInquiriesDbContext inquiriesDbContext)
    {
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<UserInquiryCommentEntity> Get(int inquiryId, int commentId, CancellationToken cancellationToken)
    {
        var inquiryComment = await _inquiriesDbContext.UserInquiryComments
           .FirstOrDefaultAsync(x => x.UserInquiryId == inquiryId
           && x.Id == commentId, cancellationToken);

        return inquiryComment ?? throw new EntityNotFoundException($"Comment by id {commentId} in inquiry with id {inquiryId} not found");
    }
}
