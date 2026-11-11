using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Contract;
using Modsen.Portal.Inquiries.Contract.Models;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.BusinessRules;

namespace Modsen.Portal.Inquiries.Requests;

internal class InquiriesContract : IInquiriesContract
{
    private readonly IIsUserApproverOrSubstituteBusinessRule _isUserApproverOrSubstituteBusinessRule;
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly ISender _sender;

    public InquiriesContract(
        IIsUserApproverOrSubstituteBusinessRule isUserApproverOrSubstituteBusinessRule,
        IInquiriesDbContext inquiriesDbContext,
        ISender sender)
    {
        _isUserApproverOrSubstituteBusinessRule = isUserApproverOrSubstituteBusinessRule;
        _inquiriesDbContext = inquiriesDbContext;
        _sender = sender;
    }

    public Task<List<UserInquiryDto>> GetUserInquiriesAsync(int userId, CancellationToken cancellationToken)
    {
        return _inquiriesDbContext.UserInquiries.Where(x => x.UserId == userId)
            .Select(x => new UserInquiryDto
            {
                UserId = x.UserId,
                Id = x.Id,
                InquiryTemplateName = x.InquiryTemplate!.Name,
                DateFrom = x.DateFrom,
                DateTo = x.DateTo,
                Status = x.Status,
                InquiryType = x.InquiryTemplate!.InquiryType
            }).ToListAsync(cancellationToken);
    }

    public Task<bool> IsUserApproverOrSubstitute(int userId, CancellationToken cancellationToken)
    {
        return _isUserApproverOrSubstituteBusinessRule.IsCorrespondsToBusiness(
            new IsUserApproverOrSubstituteBusinessRuleModel(userId), cancellationToken);
    }

    public async Task<List<UserBalanceDaysDto>> GetUserBalanceDays(int userId, CancellationToken cancellationToken)
    {
        var balances = await _sender.Send(new GetUserInquiryBalancesRequest.Query(userId), cancellationToken);
        return balances.Select(x => new UserBalanceDaysDto
        {
            UserId = x.UserId,
            InquiryTemplateName = x.InquiryTemplateName,
            InquiryType = x.InquiryType,
            Balance = x.Balance,
        }).ToList();
    }

    public Task<List<UserInquiryDto>> GetUsersInquiriesAsync(int[] userIds, CancellationToken cancellationToken, InquiryType? type = null, UserInquiryStatus? status = null)
    {
        var query = _inquiriesDbContext.UserInquiries
            .AsQueryable()
            .AsNoTracking();

        if (type != null)
        {
            query = query.Where(x => x.InquiryTemplate!.InquiryType == type);
        }

        if (status != null)
        {
            query = query.Where(x => x.Status == status);
        }

        return query
            .Where(x => userIds.Contains(x.UserId))
            .Select(x => new UserInquiryDto
            {
                UserId = x.UserId,
                Id = x.Id,
                InquiryTemplateName = x.InquiryTemplate!.Name,
                DateFrom = x.DateFrom,
                DateTo = x.DateTo,
                Status = x.Status,
                InquiryType = x.InquiryTemplate!.InquiryType
            }).ToListAsync(cancellationToken);
    }

    public Task<List<CalendarExclusionDto>> GetCalendarExclusionAsync(int year, CancellationToken cancellationToken, int? countryId = null)
    {
        var query = _inquiriesDbContext.CalendarExclusions
            .AsQueryable()
            .AsNoTracking();

        if (countryId != null)
        {
            query = query.Where(x => x.CountryId == countryId);
        }

        return query
            .Where(x => x.Date.Year == year ||
                (x.NewDate != null && x.NewDate.Value.Year == year))
            .Select(x => new CalendarExclusionDto
            {
                Id = x.Id,
                Name = x.Name,
                Date = x.Date,
                NewDate = x.NewDate,
                DateType = x.DateType,
                CountryId = x.CountryId,
            })
            .ToListAsync(cancellationToken);
    }
}
