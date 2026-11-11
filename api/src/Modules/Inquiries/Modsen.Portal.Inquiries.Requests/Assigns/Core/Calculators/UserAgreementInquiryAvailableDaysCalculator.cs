using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Core.Calculators;

internal interface IUserAgreementInquiryAvailableDaysCalculator
{
    public Task<decimal> CalculateAsync(int userId, int templateId, CancellationToken cancellationToken);
}

internal sealed class UserAgreementInquiryAvailableDaysCalculator : IUserAgreementInquiryAvailableDaysCalculator
{
    private readonly IInquiriesDbContext _dbContext;

    public UserAgreementInquiryAvailableDaysCalculator(IInquiriesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<decimal> CalculateAsync(int userId, int templateId, CancellationToken cancellationToken)
    {
        var trackingUnitCount = await _dbContext.UserAgreementTransactions
            .Where(agreementTransaction => agreementTransaction.UserId == userId &&
                agreementTransaction.InquiryTemplate!.Id == templateId)
            .SumAsync(agreementTransaction => agreementTransaction.AmountOfDays, cancellationToken);

        var daysBlockedByOpenRequests = await _dbContext.UserInquiries
            .Where(inquiry => inquiry.UserId == userId &&
                inquiry.InquiryTemplate!.Id == templateId &&
                inquiry.Status == UserInquiryStatus.Open)
            .SumAsync(inquiry => inquiry.TotalAccountantDays, cancellationToken);

        return trackingUnitCount - (daysBlockedByOpenRequests ?? 0);
    }
}
