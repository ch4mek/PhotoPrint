using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.ExternalFeedback.Requests;

internal sealed class GetOdpExternalFeedbacksRequest : IQueryHandler<GetOdpExternalFeedbacksRequest.Query, PageableListOfItems<OdpExternalFeedback>>
{

    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    public GetOdpExternalFeedbacksRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<PageableListOfItems<OdpExternalFeedback>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _teamBoardDbContext.ExternalFeedbacks
            .Where(x => x.UserId != _authenticatedUserContext.UserId)
            .Where(x => x.User!.Position != PortalRoleNames.Intern)
            .Select(OdpExternalFeedback.MapExternalFeedback)
            .AsNoTracking();

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            query = query.Where(x => x.UserDepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            query = query.Where(x => x.UserResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        var result = await request.QueryOptions
            .ApplyTo(query)
            .Cast<OdpExternalFeedback>()
            .ToListAsync(cancellationToken);

        var total = await request.QueryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpExternalFeedback>()
            .CountAsync(cancellationToken);

        return result.ToPageable(request.QueryOptions, total);
    }

    internal sealed record Query(ODataQueryOptions<OdpExternalFeedback> QueryOptions)
        : IQuery<PageableListOfItems<OdpExternalFeedback>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
