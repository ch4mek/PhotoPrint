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
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Mappers;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests;

internal sealed class GetUsersPerformanceReviewRequest
    : IQueryHandler<GetUsersPerformanceReviewRequest.Query, PageableListOfItems<OdpUserPerformanceReviewDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IPageableOdpTeamboardUserToPageableOdpUserPerformanceReviewDtoMapper _mapper;

    public GetUsersPerformanceReviewRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IPageableOdpTeamboardUserToPageableOdpUserPerformanceReviewDtoMapper mapper)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _mapper = mapper;
    }

    public async Task<PageableListOfItems<OdpUserPerformanceReviewDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _teamBoardDbContext.TeamBoardUsers
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.IsActive && x.Position != PortalRoleNames.Ceo && x.Position != PortalRoleNames.Cto && x.Position != PortalRoleNames.Intern)
            .Where(x => x.UserId != _authenticatedUserContext.UserId)
            .Select(OdpTeamBoardUser.MapTeamBoardUser);

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            query = query.Where(x => x.DepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            query = query.Where(x => x.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        query = ApplyStatusFilters(query, request);

        var queryOptions = request.QueryOptions;

        var queryResult = await queryOptions
           .ApplyTo(query)
           .Cast<OdpTeamBoardUser>()
           .ToListAsync(cancellationToken);

        var totalCount = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpTeamBoardUser>()
            .CountAsync(cancellationToken);
        var pageableUserPerformanceReviews = queryResult.ToPageable(queryOptions, totalCount);

        return _mapper.Map(pageableUserPerformanceReviews, request.OverdueDaysCount);
    }

    private static IQueryable<OdpTeamBoardUser> ApplyStatusFilters(IQueryable<OdpTeamBoardUser> query, Query request)
    {
        if (request.IssuedOnly)
        {
            query = query.Where(
                user => user.OdpUserPerformanceReviews.Any(review =>
                    review.Status != UserPerformanceReviewStatus.Passed &&
                    review.InterviewDateFromUtc!.Value.Date < DateTime.UtcNow.Date)
                || !user.OdpUserPerformanceReviews.Any(review =>
                    review.Status != UserPerformanceReviewStatus.Passed &&
                    review.InterviewDateFromUtc!.Value.Date >= DateTime.UtcNow.Date));
        }

        return query;
    }

    internal sealed record Query(int OverdueDaysCount, bool IssuedOnly, ODataQueryOptions<OdpTeamBoardUser> QueryOptions)
        : IQuery<PageableListOfItems<OdpUserPerformanceReviewDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
