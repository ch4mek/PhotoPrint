using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

internal interface IInquiryAcceptedStatusResolver
{
    Task ResolveAsync(UserInquiryEntity userInquiry, CancellationToken cancellationToken);
}

internal class InquiryAcceptedStatusResolver : IInquiryAcceptedStatusResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IHistoricalRemainingBalanceDaysHoursResolver _remainingBalanceDaysHoursResolver;
    private readonly IIdentityContract _identityContract;
    private readonly IClockService _clockService;

    public InquiryAcceptedStatusResolver(IInquiriesDbContext inquiriesDbContext,
        IHistoricalRemainingBalanceDaysHoursResolver remainingBalanceDaysHoursResolver,
        IIdentityContract identityContract, IClockService clockService)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _remainingBalanceDaysHoursResolver = remainingBalanceDaysHoursResolver;
        _identityContract = identityContract;
        _clockService = clockService;
    }

    public async Task ResolveAsync(UserInquiryEntity userInquiry, CancellationToken cancellationToken)
    {
        var inquiryType = userInquiry.InquiryTemplate!.InquiryType;
        if (UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(inquiryType))
        {
            var usingTransaction = new UserAgreementTransactionEntity
            {
                UserInquiryId = userInquiry.Id,
                InquiryTemplateId = userInquiry.InquiryTemplateId,
                UserId = userInquiry.UserId,
                UserAgreementId = null,
                TransactionDate = _clockService.UtcNow,
                AmountOfDays = -userInquiry.TotalAccountantDays!.Value,
                TransactionType = UserAgreementTransactionType.Using,
            };

            await _inquiriesDbContext.UserAgreementTransactions.AddAsync(usingTransaction, cancellationToken);
        }

        userInquiry.Status = UserInquiryStatus.Approved;
        await _remainingBalanceDaysHoursResolver.ResolveAsync(userInquiry, cancellationToken);

        var managerId = (await _identityContract
            .GetHigherManagerIdsIncludingChiefs(userInquiry.UserId, cancellationToken))
            .FirstOrDefault();

        var manager = await _inquiriesDbContext.InquiriesUsers
            .FirstOrDefaultAsync(x => x.UserId == managerId, cancellationToken);

        userInquiry.HistoricalManagerId = managerId != 0 ? managerId : null;
        userInquiry.HistoricalManagerPosition = manager?.Position;
        userInquiry.HistoricalUserPosition = userInquiry.User?.Position;
        userInquiry.HistoricalCreatedByUserPosition = userInquiry.CreatedByUser?.Position;

    }
}
