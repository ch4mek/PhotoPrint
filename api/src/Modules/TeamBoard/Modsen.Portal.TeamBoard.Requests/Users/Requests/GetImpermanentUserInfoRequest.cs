using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.Contract;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Users.Requests
{
    internal sealed class GetImpermanentUserInfoRequest : IQueryHandler<GetImpermanentUserInfoRequest.Query, ImpermanentUserInfoDto>
    {
        private readonly ITeamBoardDbContext _teamBoardDbContext;
        private readonly IInquiriesContract _inquiriesContract;
        private readonly IClockService _clockService;

        public GetImpermanentUserInfoRequest(
            ITeamBoardDbContext teamBoardDbContext,
            IClockService clockService,
            IInquiriesContract inquiriesContract)
        {
            _teamBoardDbContext = teamBoardDbContext;
            _clockService = clockService;
            _inquiriesContract = inquiriesContract;
        }

        public async Task<ImpermanentUserInfoDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _teamBoardDbContext.TeamBoardUsers
                .Include(x => x.UserPerformanceReviews)
                .FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken)
                ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

            var userInquiries = await _inquiriesContract.GetUserInquiriesAsync(user.UserId, cancellationToken);
            var days = await _inquiriesContract.GetUserBalanceDays(user.UserId, cancellationToken);

            var result = new ImpermanentUserInfoDto
            {
                FuturePerformanceReviewDate = user.UserPerformanceReviews
                    .OrderBy(x => x.InterviewDateFromUtc)
                    .FirstOrDefault(x => x.InterviewDateFromUtc > _clockService.TodayStartOfDayUtc())?
                    .InterviewDateFromUtc,

                OpenRequests = userInquiries.Select(x => new UserInquiryDto
                {
                    DateFrom = x.DateFrom,
                    DateTo = x.DateTo,
                    InquiryTemplateName = x.InquiryTemplateName,
                    InquiryType = x.InquiryType,
                    Status = x.Status,
                }).Where(x => x.Status == UserInquiryStatus.Open).ToList(),

                BalanceDays = days
                .Where(x => x.InquiryType != InquiryType.Documents)
                .Select(x => new UserBalanceDaysDto
                {
                    InquiryTemplateName = x.InquiryTemplateName,
                    InquiryType = x.InquiryType,
                    Balance = x.Balance,
                }).ToList(),
            };

            return result;
        }

        internal sealed record Query(string Email) : IQuery<ImpermanentUserInfoDto>;
    }
}
