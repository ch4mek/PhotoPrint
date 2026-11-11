using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Requests;

internal sealed class GetUserAssignsRequest : IQueryHandler<GetUserAssignsRequest.Query, List<UserAssignRequestDto>>
{
    private readonly IUserAgreementInquiryAvailableDaysCalculator _availableDaysCalculator;
    private readonly ICanAccessUserAssignsBusinessRule _canAccessUserAssignsBusinessRule;
    private readonly IInquiriesDbContext _dbContext;
    private readonly IClockService _clockService;
    public GetUserAssignsRequest(IInquiriesDbContext dbContext,
        ICanAccessUserAssignsBusinessRule canAccessUserAssignsBusinessRule,
        IClockService clockService,
        IUserAgreementInquiryAvailableDaysCalculator availableDaysCalculator)
    {
        _availableDaysCalculator = availableDaysCalculator;
        _dbContext = dbContext;
        _canAccessUserAssignsBusinessRule = canAccessUserAssignsBusinessRule;
        _clockService = clockService;
    }

    public async Task<List<UserAssignRequestDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        const AccessOptions accessOptions = AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;
        var canAssignsBeAccessed = await _canAccessUserAssignsBusinessRule.IsCorrespondsToBusiness(
            request.UserId, accessOptions, cancellationToken);

        if (!canAssignsBeAccessed)
        {
            throw new ActionNotAllowedException();
        }

        var result = new List<UserAssignRequestDto>();

        var userAgreements = await _dbContext.UserAgreements
            .Where(x => !x.IsDeleted)
            .Where(userAgreement =>
                userAgreement.UserId == request.UserId &&
                (_clockService.TodayUtc() <= userAgreement.DateTo || userAgreement.DateTo == null))
            .Where(userAgreement => userAgreement.DateFrom <= _clockService.TodayUtc())
            .Include(x => x.InquiryTemplateAgreement)
                .ThenInclude(x => x!.InquiryTemplate)
            .OrderBy(x => x.InquiryTemplateAgreement!.InquiryTemplate!.InquiryTypeOrderWeight)
                .ThenByDescending(x => x.InquiryTemplateAgreement!.InquiryTemplate!.Id)
            .ToListAsync(cancellationToken);

        foreach (var userAgreement in userAgreements)
        {
            var templateId = userAgreement.InquiryTemplateAgreement!.InquiryTemplate!.Id;

            var totalAvailableDays = await _availableDaysCalculator
                .CalculateAsync(request.UserId, templateId, cancellationToken);

            result.Add(new UserAssignRequestDto
            {
                TrackingUnitCount = (int)totalAvailableDays,
                IsAgreement = true,
                TrackingUnitType = TrackingUnit.Days,
                Type = userAgreement.InquiryTemplateAgreement.InquiryTemplate.InquiryType,
                Name = userAgreement.InquiryTemplateAgreement.InquiryTemplate.Name,
                InquiryTemplateId = templateId,
                AgreementTemplateId = userAgreement.InquiryAgreementTemplateId,
                DateFrom = userAgreement.DateFrom,
            });
        }

        var userInquiries = await _dbContext.UserInquiryTemplates
            .Where(userInquiry =>
                userInquiry.AssignedToUserId == request.UserId &&
                (_clockService.TodayUtc() <= userInquiry.DateTo || userInquiry.DateTo == null))
            .Where(userInquiry => userInquiry.DateFrom <= _clockService.TodayUtc())
            .Include(x => x.InquiryTemplate)
                .ThenInclude(x => x!.InquiryTemplateAgreements)
            .OrderBy(x => x.InquiryTemplate!.InquiryTypeOrderWeight)
                .ThenByDescending(x => x.InquiryTemplate!.Id)
            .ToListAsync(cancellationToken);


        foreach (var userInquiry in userInquiries)
        {
            var query = _dbContext.UserInquiries
                .Where(x => x.UserId == request.UserId)
                .Where(x => x.InquiryTemplateId == userInquiry.InquiryTemplateId)
                .Where(x => x.Status == UserInquiryStatus.Approved);
            var trackingUnitCount = userInquiry.InquiryTemplate!.TrackingUnitType == TrackingUnit.Days
                ? await query.SumAsync(x => x.TotalAccountantDays!.Value, cancellationToken)
                : await query.SumAsync(x => x.TotalHours!.Value, cancellationToken);

            result.Add(new UserAssignRequestDto
            {
                TrackingUnitCount = (int)trackingUnitCount,
                IsAgreement = false,
                TrackingUnitType = userInquiry.InquiryTemplate.TrackingUnitType,
                Type = userInquiry.InquiryTemplate.InquiryType,
                Name = userInquiry.InquiryTemplate.Name,
                InquiryTemplateId = userInquiry.InquiryTemplateId,
                AgreementTemplateId = null,
                DateFrom = userInquiry.DateFrom,
            });
        }

        return result.ToList();
    }

    public sealed record Query(int UserId) : IQuery<List<UserAssignRequestDto>>;
}
