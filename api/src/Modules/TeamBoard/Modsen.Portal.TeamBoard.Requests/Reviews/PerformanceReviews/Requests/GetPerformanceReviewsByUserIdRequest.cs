using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests;

internal sealed class GetPerformanceReviewsByUserIdRequest
    : IQueryHandler<GetPerformanceReviewsByUserIdRequest.Query, PageableListOfItems<OdpPerformanceReview>>
{
    private readonly ITeamBoardDbContext _dbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IPerformanceReviewDateTypeCalculator _reviewDateTypeCalculator;
    private readonly ITeamBoardPortalSettingsAccessor _teamBoardPortalSettingsAccessor;

    public GetPerformanceReviewsByUserIdRequest(ITeamBoardDbContext dbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IPerformanceReviewDateTypeCalculator reviewDateTypeCalculator,
        ITeamBoardPortalSettingsAccessor teamBoardPortalSettingsAccessor)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _reviewDateTypeCalculator = reviewDateTypeCalculator;
        _teamBoardPortalSettingsAccessor = teamBoardPortalSettingsAccessor;
        _dbContext = dbContext;
    }

    public async Task<PageableListOfItems<OdpPerformanceReview>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _dbContext.UserPerformanceReviews
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.User!.Position != PortalRoleNames.Intern)
            .Select(OdpPerformanceReview.MapPerformanceReview);

        if (_authenticatedUserContext.GetRoles().IsEmployee()
            || _authenticatedUserContext.GetRoles().IsSystemAdministrator())
        {
            query = query.Where(x => x.UserId == _authenticatedUserContext.UserId);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            query = query.Where(x => x.UserDepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            query = query.Where(x => x.UserResourceGroupId == _authenticatedUserContext.ResourceGroupId
                || x.UserId == _authenticatedUserContext.UserId);
        }

        var queryOptions = request.QueryOptions;

        var total = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpPerformanceReview>()
            .CountAsync(cancellationToken);

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpPerformanceReview>.Value;
        }

        var settings = await _teamBoardPortalSettingsAccessor.GetSettings(cancellationToken);

        var queryResult = queryOptions
            .ApplyTo(query)
            .Cast<OdpPerformanceReview>();

        var result = GetPerformanceReviewDateTypes(queryResult, request.OverdueDaysCount,
            settings.UpcomingIminentBorderDaysCount);

        var usersPerfReviews = result?.Where(x => x.UserId == _authenticatedUserContext.UserId);

        foreach (var performanceReview in usersPerfReviews ?? [])
        {
            performanceReview.Theses = null;
            performanceReview.EmployeeTheses = null;
        }

        return (result ?? []).ToPageable(queryOptions, total);
    }

    private List<OdpPerformanceReview>? GetPerformanceReviewDateTypes(IQueryable<OdpPerformanceReview> request,
        int overdueDaysCount,
        int upcomingIminentBorderDaysCount)
    {
        var isEmployeeOrSysAdmin = _authenticatedUserContext.GetRoles().IsEmployee()
            || _authenticatedUserContext.GetRoles().IsSystemAdministrator();

        return request?.Select(x => new OdpPerformanceReview
        {
            Id = x.Id,
            ConversationId = x.ConversationId,
            UserId = x.UserId,
            Agreements = x.Agreements,
            Theses = isEmployeeOrSysAdmin ? null : x.Theses,
            EmployeeTheses = isEmployeeOrSysAdmin ? null : x.EmployeeTheses,
            PerformanceReviewDate = x.PerformanceReviewDate,
            Status = x.Status,
            PerformanceReviewDateType = x.Status == UserPerformanceReviewStatus.Passed.GetDescription()
                ? PerformanceReviewDateType.Passed.GetDescription()
                : _reviewDateTypeCalculator.Calculate(x.PerformanceReviewDate!.Value.Date, overdueDaysCount,
                    upcomingIminentBorderDaysCount),
            OdpConversationDocumentLinks = x.OdpConversationDocumentLinks,
            OdpConversationFiles = x.OdpConversationFiles
        }).ToList() ?? [];
    }

    internal sealed record Query(int OverdueDaysCount, ODataQueryOptions<OdpPerformanceReview> QueryOptions)
        : IQuery<PageableListOfItems<OdpPerformanceReview>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
