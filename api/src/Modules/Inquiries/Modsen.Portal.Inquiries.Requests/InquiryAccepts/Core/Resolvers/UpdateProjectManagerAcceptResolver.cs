using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers
{
    internal interface IUpdateProjectManagerAcceptResolver
    {
        Task ResolveAsync(int projectId, int? oldPmId, int? newPmId, bool isActive, CancellationToken cancellationToken);
    }

    internal class UpdateProjectManagerAcceptResolver : IUpdateProjectManagerAcceptResolver
    {
        private readonly ITimeTrackContract _timeTrackContract;
        private readonly IInquiriesDbContext _inquiriesDbContext;
        private readonly IUpdateUserInfoInquiryAcceptsResolver _updateUserInfoInquiryAcceptsResolver;

        public UpdateProjectManagerAcceptResolver(
            ITimeTrackContract timeTrackContract,
            IInquiriesDbContext inquiriesDbContext,
            IUpdateUserInfoInquiryAcceptsResolver updateUserInfoInquiryAcceptsResolver)
        {
            _timeTrackContract = timeTrackContract;
            _inquiriesDbContext = inquiriesDbContext;
            _updateUserInfoInquiryAcceptsResolver = updateUserInfoInquiryAcceptsResolver;
        }

        public async Task ResolveAsync(int projectId, int? oldPmId, int? newPmId, bool isActive,
            CancellationToken cancellationToken)
        {
            if (oldPmId == newPmId)
            {
                return;
            }

            var usersIds = await _timeTrackContract.GetUsersByProjectId(projectId, cancellationToken);

            var inquiriesToUpdate = await _inquiriesDbContext.UserInquiries
                .Where(x => usersIds.Contains(x.UserId))
                .Where(x => x.Status == UserInquiryStatus.Open)
                .Select(x => x.Id)
                .ToArrayAsync(cancellationToken);

            if (inquiriesToUpdate.Length > 0)
            {
                await _updateUserInfoInquiryAcceptsResolver.ResolveAsync(inquiriesToUpdate, cancellationToken);
            }
        }
    }
}
