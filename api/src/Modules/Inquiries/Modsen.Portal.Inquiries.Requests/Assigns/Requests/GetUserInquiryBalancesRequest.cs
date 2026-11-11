using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Calculators;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Requests;

internal sealed class GetUserInquiryBalancesRequest :
    IQueryHandler<GetUserInquiryBalancesRequest.Query, List<UserInquiryBalance>>
{
    private readonly IUserAgreementInquiryAvailableDaysCalculator _availableDaysCalculator;
    private readonly IInquiriesDbContext _dbContext;
    private readonly IClockService _clockService;

    public GetUserInquiryBalancesRequest(IInquiriesDbContext dbContext,
        IClockService clockService,
        IUserAgreementInquiryAvailableDaysCalculator availableDaysCalculator)
    {
        _availableDaysCalculator = availableDaysCalculator;
        _dbContext = dbContext;
        _clockService = clockService;
    }

    public async Task<List<UserInquiryBalance>> Handle(Query request, CancellationToken cancellationToken)
    {
        var result = new List<UserInquiryBalance>();

        var user = await _dbContext.InquiriesUsers
            .Where(x => x.IsActive)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.UserNotFound);

        var userAgreements = await _dbContext.UserAgreements
            .Where(x => !x.IsDeleted)
            .Where(x => x.UserId == request.UserId)
            .Where(x => x.DateFrom <= _clockService.TodayUtc())
            .Where(x => _clockService.TodayUtc() <= x.DateTo || x.DateTo == null)
            .Include(x => x.User)
            .Include(x => x.InquiryTemplateAgreement)
                .ThenInclude(x => x!.InquiryTemplate)
            .OrderBy(x => x.InquiryTemplateAgreement!.InquiryTemplate!.InquiryTypeOrderWeight)
                .ThenByDescending(x => x.InquiryTemplateAgreement!.InquiryTemplate!.Id)
            .ToListAsync(cancellationToken);

        foreach (var inquiryTemplate in userAgreements.Select(x => x.InquiryTemplateAgreement!.InquiryTemplate!))
        {
            var availableDays = await _availableDaysCalculator
                .CalculateAsync(request.UserId, inquiryTemplate.Id, cancellationToken);

            result.Add(new UserInquiryBalance
            {
                UserId = request.UserId,
                FullName = user.LastName + ' ' + user.FirstName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FirstNameRu = user.FirstNameRu,
                LastNameRu = user.LastNameRu,
                FullNameRu = (user.LastNameRu + ' ' + user.FirstNameRu).Trim(),
                Email = user.Email,

                Balance = availableDays,
                TrackingUnit = TrackingUnit.Days,

                InquiryTemplateId = inquiryTemplate.Id,
                InquiryType = inquiryTemplate.InquiryType,
                InquiryTemplateName = inquiryTemplate.Name,
            });
        }

        var userInquiryTemplates = await _dbContext.UserInquiryTemplates
            .Where(x => x.AssignedToUserId == request.UserId)
            .Where(x => _clockService.TodayUtc() <= x.DateTo || x.DateTo == null)
            .Where(x => x.DateFrom <= _clockService.TodayUtc())
            .Include(x => x.AssignedToUser)
            .Include(x => x.InquiryTemplate)
                .ThenInclude(x => x!.InquiryTemplateAgreements)
            .OrderBy(x => x.InquiryTemplate!.InquiryTypeOrderWeight)
                .ThenByDescending(x => x.InquiryTemplate!.Id)
            .ToListAsync(cancellationToken);

        foreach (var userInquiry in userInquiryTemplates)
        {
            var approvedInquiries = _dbContext.UserInquiries
                .Where(x => x.UserId == request.UserId &&
                    x.InquiryTemplateId == userInquiry.InquiryTemplateId &&
                    x.Status == UserInquiryStatus.Approved);

            var trackingUnitCount = userInquiry.InquiryTemplate!.TrackingUnitType == TrackingUnit.Days
                ? await approvedInquiries.SumAsync(x => x.TotalAccountantDays!.Value, cancellationToken)
                : await approvedInquiries.SumAsync(x => x.TotalHours!.Value, cancellationToken);

            result.Add(new UserInquiryBalance
            {
                UserId = request.UserId,
                FullName = user.LastName + ' ' + user.FirstName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,

                Balance = trackingUnitCount,
                TrackingUnit = userInquiry.InquiryTemplate!.TrackingUnitType,

                InquiryTemplateId = userInquiry.InquiryTemplateId,
                InquiryType = userInquiry.InquiryTemplate.InquiryType,
                InquiryTemplateName = userInquiry.InquiryTemplate.Name,
            });
        }

        return result.ToList();
    }

    public sealed record Query(int UserId) : IQuery<List<UserInquiryBalance>>;
}
