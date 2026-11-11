using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Composers;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers
{
    internal interface IRejectInquiriesAfterUnassignResolver
    {
        Task<List<UserInquiryEntity>> ResolveAsync(
            int[] unassignUserIds,
            int inquiryTemplateId,
            int unassignById,
            CancellationToken cancellationToken);

        Task<List<UserInquiryEntity>> ResolveAsync(
            int[] unassignUserIds,
            int inquiryTemplateId,
            CancellationToken cancellationToken);

        Task<List<UserInquiryEntity>> ResolveAsync(
            int unassignUserId,
            int[] inquiryTemplateIds,
            int unassignById,
            CancellationToken cancellationToken);
    }

    internal class RejectInquiriesAfterUnassignResolver : IRejectInquiriesAfterUnassignResolver
    {
        private readonly IInquiriesDbContext _dbContext;
        private readonly IInquiryRejectedStatusResolver _inquiryRejectedStatusResolver;
        private readonly IUserInquiryAcceptStatusComposer _inquiryAcceptStatusComposer;
        private readonly IAuthenticatedUserContext _authenticatedUserContext;

        public RejectInquiriesAfterUnassignResolver(
            IInquiriesDbContext inquiriesDbContext,
            IInquiryRejectedStatusResolver inquiryRejectedStatusResolver,
            IUserInquiryAcceptStatusComposer userInquiryAcceptStatusComposer,
            IAuthenticatedUserContext authenticatedUserContext)
        {
            _dbContext = inquiriesDbContext;
            _inquiryRejectedStatusResolver = inquiryRejectedStatusResolver;
            _inquiryAcceptStatusComposer = userInquiryAcceptStatusComposer;
            _authenticatedUserContext = authenticatedUserContext;
        }

        public async Task<List<UserInquiryEntity>> ResolveAsync(
            int[] unassignUserIds,
            int inquiryTemplateId,
            int unassignById,
            CancellationToken cancellationToken)
        {
            if (unassignUserIds.Length == 0)
            {
                return [];
            }

            var inquiriesToReject = await _dbContext.UserInquiries
                .Where(x => unassignUserIds.Contains(x.UserId))
                .Where(x => x.InquiryTemplateId == inquiryTemplateId)
                .Where(x => x.Status == UserInquiryStatus.Open)
                .Include(x => x.UserInquiryAccepts)
                .Include(x => x.InquiryTemplate)
                .ToListAsync(cancellationToken);

            return await RejectInquiriesAsync(inquiriesToReject, unassignById, cancellationToken);
        }

        public Task<List<UserInquiryEntity>> ResolveAsync(
            int[] unassignUserIds,
            int inquiryTemplateId,
            CancellationToken cancellationToken)
        {
            return ResolveAsync(unassignUserIds, inquiryTemplateId, _authenticatedUserContext.GetRequiredUserId(), cancellationToken);
        }

        public async Task<List<UserInquiryEntity>> ResolveAsync(
            int unassignUserId,
            int[] inquiryTemplateIds,
            int unassignById,
            CancellationToken cancellationToken)
        {
            var inquiriesToReject = await _dbContext.UserInquiries
                .Where(x => unassignUserId == x.UserId)
                .Where(x => inquiryTemplateIds.Contains(x.InquiryTemplateId))
                .Where(x => x.Status == UserInquiryStatus.Open)
                .Include(x => x.UserInquiryAccepts)
                .Include(x => x.InquiryTemplate)
                .ToListAsync(cancellationToken);

            return await RejectInquiriesAsync(inquiriesToReject, unassignById, cancellationToken);
        }

        private async Task<List<UserInquiryEntity>> RejectInquiriesAsync(
            List<UserInquiryEntity> inquiriesToReject,
            int unassignById,
            CancellationToken cancellationToken)
        {
            if (inquiriesToReject.Count == 0)
            {
                return [];
            }

            foreach (var inquiry in inquiriesToReject)
            {
                var pendingAccepts = inquiry.UserInquiryAccepts
                    .Where(accept =>
                        accept.Status == UserInquiryAcceptStatus.Pending ||
                        accept.Status == UserInquiryAcceptStatus.WaitingForApprove)
                    .ToList();

                if (pendingAccepts.Count != 0)
                {
                    pendingAccepts.ForEach(accept => accept.Status = UserInquiryAcceptStatus.Cancelled);
                    _inquiryAcceptStatusComposer.Compose(
                        pendingAccepts.OrderBy(x => x.Id).First(),
                        unassignById,
                        UserInquiryAcceptStatus.Rejected);
                }

                await _inquiryRejectedStatusResolver.ResolveAsync(inquiry, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return inquiriesToReject;
        }
    }
}
