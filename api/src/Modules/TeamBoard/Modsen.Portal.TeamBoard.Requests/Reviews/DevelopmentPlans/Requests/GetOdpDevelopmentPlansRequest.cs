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
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests;

internal sealed class GetOdpDevelopmentPlansRequest : IQueryHandler<GetOdpDevelopmentPlansRequest.Query, PageableListOfItems<OdpDevelopmentPlan>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetOdpDevelopmentPlansRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PageableListOfItems<OdpDevelopmentPlan>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;

        var baseQuery = _teamBoardDbContext.DevelopmentPlans
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.Conversation)
            .Where(x => x.User!.Position != PortalRoleNames.Intern)
            .AsQueryable();

        if (request.ConversationId != null)
        {
            baseQuery = baseQuery.Where(x => x.ConversationId == request.ConversationId);
        }

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            baseQuery = baseQuery.Where(x => x.User!.DepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            baseQuery = baseQuery.Where(x => x.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee() || _authenticatedUserContext.GetRoles().IsSystemAdministrator())
        {
            baseQuery = baseQuery.Where(x => x.UserId == _authenticatedUserContext.UserId);
        }

        if (request.IsDone.HasValue)
        {
            baseQuery = baseQuery.Where(x =>
                (x.EndDate.HasValue && x.EndDate <= DateOnly.FromDateTime(DateTime.UtcNow)) == request.IsDone.Value);
        }

        baseQuery = request.Source.HasValue ? baseQuery.Where(x => x.Source == request.Source) : baseQuery;
        var query = baseQuery.Select(OdpDevelopmentPlan.MapDevelopmentPlan);

        var total = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpDevelopmentPlan>()
            .CountAsync(cancellationToken);

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpDevelopmentPlan>.Value;
        }

        var result = await queryOptions
            .ApplyTo(query)
            .Cast<OdpDevelopmentPlan>()
            .ToListAsync(cancellationToken);

        var usersPlans = result.Where(x => x.UserId == _authenticatedUserContext.GetRequiredUserId());

        foreach (var plan in usersPlans)
        {
            plan.Notes = null;
        }

        return result.ToPageable(queryOptions, total);
    }

    internal sealed record Query(DevelopmentPlanSource? Source, bool? IsDone, int? ConversationId, ODataQueryOptions<OdpDevelopmentPlan> QueryOptions)
        : IQuery<PageableListOfItems<OdpDevelopmentPlan>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }

}
