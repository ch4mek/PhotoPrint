using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

internal interface IAvailableInquiryAcceptsResolver
{
    Task<List<UserInquiryAcceptEntity>> ResolveForApprovalAsync(int inquiryId, CancellationToken cancellationToken);
    List<UserInquiryAcceptEntity> ResolveForApproval(UserInquiryEntity inquiry);

    Task<(List<UserInquiryAcceptEntity> AcceptsToReject, bool IsAbleToReject)> ResolveForRejectionAsync(int inquiryId, CancellationToken cancellationToken);
    (List<UserInquiryAcceptEntity> AcceptsToReject, bool IsAbleToReject) ResolveForRejection(UserInquiryEntity inquiry);
}

internal class AvailableInquiryAcceptsResolver : IAvailableInquiryAcceptsResolver
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public AvailableInquiryAcceptsResolver(
        IAuthenticatedUserContext authenticatedUserContext,
        IInquiriesDbContext inquiriesDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<List<UserInquiryAcceptEntity>> ResolveForApprovalAsync(int inquiryId, CancellationToken cancellationToken)
    {
        var inquiry = await _inquiriesDbContext.UserInquiries
            .AsNoTracking()
            .Where(x => x.Id == inquiryId)
            .Include(x => x.UserInquiryAccepts)
            .Where(x => x.Id == inquiryId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserInquiryNotFound);

        return ResolveForApproval(inquiry);
    }

    public List<UserInquiryAcceptEntity> ResolveForApproval(UserInquiryEntity inquiry)
    {
        var inquiryAcceptEntityGroup = inquiry.UserInquiryAccepts
            .Where(accept => accept.Status == UserInquiryAcceptStatus.WaitingForApprove);

        var isChief = _authenticatedUserContext.GetRoles().IsCeo() ||
            _authenticatedUserContext.GetRoles().IsCto() ||
            _authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name);

        if (isChief)
        {
            return inquiryAcceptEntityGroup.ToList();
        }

        return inquiryAcceptEntityGroup.Where(x => x.AcceptUserId == _authenticatedUserContext.UserId).ToList();
    }

    public async Task<(List<UserInquiryAcceptEntity> AcceptsToReject, bool IsAbleToReject)> ResolveForRejectionAsync(
        int inquiryId, CancellationToken cancellationToken)
    {
        var inquiry = await _inquiriesDbContext.UserInquiries
                .Where(x => x.Id == inquiryId)
                .Include(x => x.UserInquiryAccepts)
                .Where(x => x.Id == inquiryId)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserInquiryNotFound);

        return ResolveForRejection(inquiry);
    }

    public (List<UserInquiryAcceptEntity> AcceptsToReject, bool IsAbleToReject) ResolveForRejection(UserInquiryEntity inquiry)
    {
        var isInquiryApproved = inquiry.Status == UserInquiryStatus.Approved;

        var isChief = _authenticatedUserContext.GetRoles().IsCeo() ||
            _authenticatedUserContext.GetRoles().IsCto() ||
            _authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name);

        var isRequester = _authenticatedUserContext.UserId == inquiry.UserId;

        if (isChief && isInquiryApproved)
        {
            return (inquiry.UserInquiryAccepts.ToList(), true);
        }

        var inquiryAcceptEntityGroup = inquiry.UserInquiryAccepts
              .Where(accept => accept.Status == UserInquiryAcceptStatus.WaitingForApprove);

        var inquiryAcceptEntities = isRequester || isChief
            ? inquiryAcceptEntityGroup.ToList()
            : inquiryAcceptEntityGroup.Where(x => x.AcceptUserId == _authenticatedUserContext.UserId).ToList();

        return (inquiryAcceptEntities, inquiryAcceptEntities.Count != 0);
    }
}
