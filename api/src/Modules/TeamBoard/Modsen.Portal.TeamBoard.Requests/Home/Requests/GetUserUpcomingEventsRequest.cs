using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Inquiries.Contract;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;

namespace Modsen.Portal.TeamBoard.Requests.Home.Requests;

internal sealed class GetUserUpcomingEventsRequest :
    IQueryHandler<GetUserUpcomingEventsRequest.Query, List<UpcomingEventDto>>
{
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canEventsBeAccessedByUser;
    private readonly ITeamBoardPortalSettingsAccessor _teamBoardPortalSettingsAccessor;
    private readonly IPerformanceReviewDateTypeCalculator _reviewDateTypeCalculator;
    private readonly ITeamBoardDbContext _dbContext;
    private readonly IInquiriesContract _inquiriesContract;

    private CancellationToken _token;

    public GetUserUpcomingEventsRequest(ICanTeamBoardUserBeRetrievedBusinessRule canEventsBeAccessedByUser,
        ITeamBoardPortalSettingsAccessor teamBoardPortalSettingsAccessor,
        IPerformanceReviewDateTypeCalculator reviewDateTypeCalculator,
        ITeamBoardDbContext dbContext,
        IInquiriesContract inquiriesContract)
    {
        _teamBoardPortalSettingsAccessor = teamBoardPortalSettingsAccessor;
        _canEventsBeAccessedByUser = canEventsBeAccessedByUser;
        _reviewDateTypeCalculator = reviewDateTypeCalculator;
        _dbContext = dbContext;
        _inquiriesContract = inquiriesContract;
    }

    public async Task<List<UpcomingEventDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        _token = cancellationToken;

        var canEventsBeAccessed = await _canEventsBeAccessedByUser.IsCorrespondsToBusiness(request.UserId, _token);
        if (!canEventsBeAccessed)
        {
            throw new ActionNotAllowedException();
        }

        var result = new List<UpcomingEventDto>();

        var performanceReviews = await GetPerformanceReviewsAsync(request);
        result.AddRange(performanceReviews);

        var contracts = await GetContractsAsync(request);
        result.AddRange(contracts);

        var devPlans = await GetDevPlansAsync(request);
        result.AddRange(devPlans);

        var inquiries = await GetUserInquiriesAsync(request);
        result.AddRange(inquiries);

        return result.OrderBy(eventDto => eventDto.StartDate).ThenBy(eventDto => eventDto.Type).ToList();
    }

    private async Task<List<UpcomingEventDto>> GetPerformanceReviewsAsync(Query request)
    {
        var settings = await _teamBoardPortalSettingsAccessor.GetSettings(_token);
        var upcomingIminentBorderDaysCount = settings.UpcomingIminentBorderDaysCount;

        var performanceReviews = await _dbContext.UserPerformanceReviews
            .Where(review =>
                review.UserId == request.UserId &&                
                review.Status != UserPerformanceReviewStatus.Passed)
            .ToListAsync(_token);

        var performanceReviewEvents = performanceReviews
            .Select(review => new UpcomingEventDto
            {
                Type = UpcomingEventType.PerformanceReview,
                Name = null,
                StartDate = DateOnly.FromDateTime(review.InterviewDateFromUtc!.Value),
                EndDate = null,
                Status = null,
                PerformanceReviewDateType = _reviewDateTypeCalculator.Calculate(review.InterviewDateFromUtc.Value,
                    request.OverdueDaysCount,
                    upcomingIminentBorderDaysCount),
                InquiryType = null
            })
            .ToList();

        return performanceReviewEvents;
    }

    private Task<List<UpcomingEventDto>> GetContractsAsync(Query request)
    {
        return _dbContext.UserContracts
            .Where(
                contract =>
                    contract.UserId == request.UserId &&
                    contract.ContractTo.HasValue &&
                    contract.ContractTo.Value >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
            .Select(contract => new UpcomingEventDto
            {
                Type = UpcomingEventType.Contract,
                Name = null,
                StartDate = contract.ContractTo!.Value,
                EndDate = null,
                Status = null,
                PerformanceReviewDateType = null,
                InquiryType = null,
            })
            .ToListAsync(_token);
    }

    private Task<List<UpcomingEventDto>> GetDevPlansAsync(Query request)
    {
        return _dbContext.DevelopmentPlans
            .Where(plan =>
                plan.UserId == request.UserId &&
                plan.DeadlineDate.HasValue &&
                plan.DeadlineDate.Value >= DateOnly.FromDateTime(DateTime.UtcNow.Date) &&
                plan.Status != DevelopmentPlanStatusType.Approved)
            .Select(plan => new UpcomingEventDto
            {
                Type = UpcomingEventType.DevelopmentPlan,
                Name = null,
                StartDate = plan.DeadlineDate!.Value,
                EndDate = null,
                Status = null,
                PerformanceReviewDateType = null,
                InquiryType = null
            })
            .ToListAsync(_token);
    }

    private async Task<List<UpcomingEventDto>> GetUserInquiriesAsync(Query request)
    {
        return (await _inquiriesContract.GetUserInquiriesAsync(request.UserId, _token))
            .Where(x => x.DateTo >= DateOnly.FromDateTime(DateTime.UtcNow))
            .Select(x => new UpcomingEventDto
            {
                Name = x.InquiryTemplateName,
                StartDate = x.DateFrom,
                EndDate = x.DateTo,
                PerformanceReviewDateType = null,
                Status = x.Status.GetDescription(),
                Type = UpcomingEventType.Absence,
                InquiryId = x.Id,
                InquiryType = x.InquiryType
            }).ToList();
    }
    internal sealed record Query(int UserId, int OverdueDaysCount) : IQuery<List<UpcomingEventDto>>;
}
