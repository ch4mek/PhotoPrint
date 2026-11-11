using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers
{
    internal interface IUpdateUserAcceptsResolver
    {
        Task ResolveAsync(int userId, CancellationToken cancellationToken);
    }

    internal class UpdateUserAcceptsResolver : IUpdateUserAcceptsResolver
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IInquiriesDbContext _inquiriesDbContext;
        private readonly IUpdateUserInfoInquiryAcceptsResolver _updateUserInfoInquiryAcceptsResolver;

        public UpdateUserAcceptsResolver(
            IInquiriesDbContext inquiriesDbContext,
            IUpdateUserInfoInquiryAcceptsResolver updateUserInfoInquiryAcceptsResolver,
            IConnectionFactory connectionFactory)
        {
            _inquiriesDbContext = inquiriesDbContext;
            _updateUserInfoInquiryAcceptsResolver = updateUserInfoInquiryAcceptsResolver;
            _connectionFactory = connectionFactory;
        }

        public async Task ResolveAsync(int userId, CancellationToken cancellationToken)
        {
            _ = await _inquiriesDbContext.InquiriesUsers
                    .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken)
                ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

            var userInquiries = await _inquiriesDbContext.UserInquiries
                .Where(x => x.UserId == userId)
                .Where(x => x.Status == UserInquiryStatus.Open)
                .Select(x => x.Id)
                .ToArrayAsync(cancellationToken);

            if (userInquiries.Length > 0)
            {
                await _updateUserInfoInquiryAcceptsResolver.ResolveAsync(userInquiries, cancellationToken);
                await _inquiriesDbContext.SaveChangesAsync(cancellationToken);
                await _connectionFactory.CommitCurrentTransaction(cancellationToken);
            }
        }
    }
}
