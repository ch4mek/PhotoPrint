using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

internal interface IInquiryRejectedStatusResolver
{
    Task<UserInquiryEntity> ResolveAsync(UserInquiryEntity userInquiry, CancellationToken cancellationToken);
    Task<UserInquiryEntity> ResolveOwnInquiryAsync(UserInquiryEntity userInquiry, CancellationToken cancellationToken);
}

internal class InquiryRejectedStatusResolver : IInquiryRejectedStatusResolver
{
    private readonly IHistoricalRemainingBalanceDaysHoursResolver _remainingBalanceDaysHoursResolver;
    private readonly IIdentityContract _identityContract;
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IClockService _clockService;

    public InquiryRejectedStatusResolver(IHistoricalRemainingBalanceDaysHoursResolver remainingBalanceDaysHoursResolver,
        IIdentityContract identityContract,
        IInquiriesDbContext inquiriesDbContext,
        IClockService clockService)
    {
        _remainingBalanceDaysHoursResolver = remainingBalanceDaysHoursResolver;
        _identityContract = identityContract;
        _inquiriesDbContext = inquiriesDbContext;
        _clockService = clockService;
    }

    public async Task<UserInquiryEntity> ResolveAsync(UserInquiryEntity userInquiry, CancellationToken cancellationToken)
    {
        return await ResolveStatusAsync(userInquiry, UserInquiryStatus.Rejected, cancellationToken);        
    }

    public async Task<UserInquiryEntity> ResolveOwnInquiryAsync(UserInquiryEntity userInquiry, CancellationToken cancellationToken)
    {   
        return await ResolveStatusAsync(userInquiry, UserInquiryStatus.Canceled, cancellationToken);
    }

    private async Task<UserInquiryEntity> ResolveStatusAsync(UserInquiryEntity userInquiry, UserInquiryStatus status, CancellationToken cancellationToken)
    {
        var prevStatus = userInquiry.Status;
        userInquiry.Status = status;

        if (prevStatus == UserInquiryStatus.Approved)
        {
            CompensateTheUsedTransaction(userInquiry);
        }

        await _remainingBalanceDaysHoursResolver.ResolveAsync(userInquiry, cancellationToken);
        var managerId = (await _identityContract
            .GetHigherManagerIdsIncludingChiefs(userInquiry.UserId, cancellationToken))
            .FirstOrDefault();

        var manager = await _inquiriesDbContext.InquiriesUsers
            .FirstOrDefaultAsync(x => x.UserId == managerId, cancellationToken);

        userInquiry.HistoricalManagerId = manager?.UserId;
        userInquiry.HistoricalManagerPosition = manager?.Position;
        userInquiry.HistoricalUserPosition = userInquiry.User?.Position;
        userInquiry.HistoricalCreatedByUserPosition = userInquiry.CreatedByUser?.Position;

        return userInquiry;
    }

    private void CompensateTheUsedTransaction(UserInquiryEntity userInquiry)
    {
        var inquiryType = userInquiry.InquiryTemplate!.InquiryType;
        if (!UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(inquiryType))
        {
            return;
        }

        var accrueTransaction = new UserAgreementTransactionEntity
        {
            UserInquiryId = userInquiry.Id,
            InquiryTemplateId = userInquiry.InquiryTemplateId,
            UserId = userInquiry.UserId,
            UserAgreementId = null,
            TransactionDate = _clockService.UtcNow,
            AmountOfDays = userInquiry.TotalAccountantDays!.Value,
            TransactionType = UserAgreementTransactionType.Accrual,
            Desctiption = "Correction: balance refunded due to rejection of an approved absence request",
        };

        _inquiriesDbContext.UserAgreementTransactions.Add(accrueTransaction);
    }
}
