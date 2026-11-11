using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

internal interface IHistoricalRemainingBalanceDaysHoursResolver
{
    Task ResolveAsync(UserInquiryCoreDto userInquiryDto, CancellationToken cancellationToken);
    Task ResolveAsync(UserInquiryEntity userInquiry, CancellationToken cancellationToken);
}

internal class HistoricalRemainingBalanceDaysHoursResolver : IHistoricalRemainingBalanceDaysHoursResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IUserAgreementInquiryAvailableDaysCalculator _availableDaysCalculator;
    
    public HistoricalRemainingBalanceDaysHoursResolver(IInquiriesDbContext inquiriesDbContext,
        IUserAgreementInquiryAvailableDaysCalculator availableDaysCalculator)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _availableDaysCalculator = availableDaysCalculator;
    }

    public async Task ResolveAsync(UserInquiryCoreDto userInquiryDto, CancellationToken cancellationToken)
    {
        if (userInquiryDto.IsAgreement)
        {
            userInquiryDto.HistoricalRemainingDays = await _availableDaysCalculator.CalculateAsync(
                userInquiryDto.InquiryUserId, userInquiryDto.InquiryTemplateId, cancellationToken);
        }
        else if (userInquiryDto.TrackingUnitType == TrackingUnit.Days)
        {

            userInquiryDto.HistoricalBalanceDays = await _inquiriesDbContext.UserInquiries
                .Where(x => x.UserId == userInquiryDto.InquiryUserId &&
                    x.InquiryTemplateId == userInquiryDto.InquiryTemplateId &&
                    x.Status == UserInquiryStatus.Approved)
                .SumAsync(x => x.TotalAccountantDays!.Value, cancellationToken);
        }
        else
        {
            userInquiryDto.HistoricalBalanceHours = await _inquiriesDbContext.UserInquiries
                .Where(x => x.UserId == userInquiryDto.InquiryUserId &&
                    x.InquiryTemplateId == userInquiryDto.InquiryTemplateId &&
                    x.Status == UserInquiryStatus.Approved)
                .SumAsync(x => x.TotalHours!.Value, cancellationToken);
        }
    }

    public async Task ResolveAsync(UserInquiryEntity userInquiry, CancellationToken cancellationToken)
    {
        if (userInquiry.AgreementTemplateId.HasValue)
        {
            userInquiry.HistoricalRemainingDays = await _availableDaysCalculator.CalculateAsync(userInquiry.UserId,
                userInquiry.InquiryTemplateId, cancellationToken);

            if (userInquiry.Status == UserInquiryStatus.Rejected || userInquiry.Status == UserInquiryStatus.Canceled)
            {
                userInquiry.HistoricalRemainingDays += userInquiry.TotalAccountantDays!.Value;
            }
        }
        else if (userInquiry.InquiryTemplate!.TrackingUnitType == TrackingUnit.Days)
        {
            userInquiry.HistoricalBalanceDays = await _inquiriesDbContext.UserInquiries
                .Where(x => x.UserId == userInquiry.UserId &&
                    x.InquiryTemplateId == userInquiry.InquiryTemplateId &&
                    x.Status == UserInquiryStatus.Approved)
                .SumAsync(x => x.TotalAccountantDays!.Value, cancellationToken);

            if (userInquiry.Status == UserInquiryStatus.Approved)
            {
                userInquiry.HistoricalBalanceDays += userInquiry.TotalAccountantDays!.Value;
            }
        }
        else
        {
            userInquiry.HistoricalBalanceHours = await _inquiriesDbContext.UserInquiries
                .Where(x => x.UserId == userInquiry.UserId &&
                    x.InquiryTemplateId == userInquiry.InquiryTemplateId &&
                    x.Status == UserInquiryStatus.Approved)
                .SumAsync(x => x.TotalHours!.Value, cancellationToken);

            if (userInquiry.Status == UserInquiryStatus.Approved)
            {
                userInquiry.HistoricalBalanceHours += userInquiry.TotalHours!.Value;
            }
        }
    }
}
