using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Interviews.Core.AuthenticationFilters;

namespace Modsen.Portal.TeamBoard.Requests.Interviews.Requests;

internal sealed class GetOdpInterviewsRequest : IQueryHandler<GetOdpInterviewsRequest.Query, PageableListOfItems<OdpInterviewDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IInterviewsAuthenticationFilter _interviewsAuthenticationFilter;

    public GetOdpInterviewsRequest(ITeamBoardDbContext teamBoardDbContext,
        IInterviewsAuthenticationFilter interviewsAuthenticationFilter)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _interviewsAuthenticationFilter = interviewsAuthenticationFilter;
    }

    public async Task<PageableListOfItems<OdpInterviewDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var mainInterviewQuery = _interviewsAuthenticationFilter.Apply(_teamBoardDbContext.UserInterviews.AsNoTracking());

        var mainInterviewsIds = await mainInterviewQuery
            .Where(x => x.IsActive)
            .GroupBy(x => x.GroupId)
            .Select(x => x.OrderByDescending(f => f.Id).Select(f => f.Id).First())
            .ToListAsync(cancellationToken);

        var mainInterviews = mainInterviewQuery.Where(x => mainInterviewsIds.Contains(x.Id));

        var filteredMainInterviews = request.QueryOptions.RebuildWithoutPagination()
            .ApplyTo(mainInterviews.Select(OdpInterview.Map))
            .OfType<OdpInterview>();

        var filteredMainInterviewsIds = await filteredMainInterviews.Select(x => x.GroupId).ToListAsync(cancellationToken);

        var total = filteredMainInterviewsIds.Count;

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpInterviewDto>.Value;
        }

        var resultMainInterviews = await request.QueryOptions.RebuildWithSortingAndPagination()
            .ApplyTo(filteredMainInterviews)
            .Cast<OdpInterview>()
            .ToListAsync(cancellationToken);

        var resultInterviewGroups = _teamBoardDbContext.UserInterviews
            .Where(x => x.IsActive)
            .Where(x => filteredMainInterviewsIds.Contains(x.GroupId))
            .AsNoTracking()
            .Select(OdpInterview.Map)
            .ToLookup(x => x.GroupId);

        return resultMainInterviews.Select(x => new OdpInterviewDto
        {
            Main = x,
            Stages = resultInterviewGroups[x.GroupId].OrderByDescending(s => s.Id).Skip(1).ToArray()
        }).ToPageable(request.QueryOptions, total);
    }

    internal sealed record Query(ODataQueryOptions<OdpInterview> QueryOptions)
        : IQuery<PageableListOfItems<OdpInterviewDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
