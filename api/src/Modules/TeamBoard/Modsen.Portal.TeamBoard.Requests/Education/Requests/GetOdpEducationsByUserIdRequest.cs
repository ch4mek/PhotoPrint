using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Education.Core.BusinessRules;

namespace Modsen.Portal.TeamBoard.Requests.Education.Requests;

internal sealed class GetOdpEducationsByUserIdRequest
    : IQueryHandler<GetOdpEducationsByUserIdRequest.Query, PageableListOfItems<OdpUserEducation>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ICanGetUserEducationBusinessRule _canGetEducationBusinessRule;

    public GetOdpEducationsByUserIdRequest(ITeamBoardDbContext teamBoardDbContext, IAuthenticatedUserContext authenticatedUserContext,
        ICanGetUserEducationBusinessRule canGetEducationBusinessRule)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _canGetEducationBusinessRule = canGetEducationBusinessRule;
    }

    public async Task<PageableListOfItems<OdpUserEducation>> Handle(Query request, CancellationToken cancellationToken)
    {
        var isCorrespondsToBusiness = await _canGetEducationBusinessRule.IsCorrespondsToRuleAsync(request.UserId,
            cancellationToken);

        if (!isCorrespondsToBusiness)
        {
            throw new ActionNotAllowedException("You are not allowed to Get a user education");
        }

        var page = request.QueryOptions.Skip.Value / request.QueryOptions.Top.Value + 1;
        var pageSize = request.QueryOptions.Top.Value;

        var shouldReturnEmpty = await ShouldReturnEmptyAsync(request, cancellationToken);
        if (shouldReturnEmpty)
        {
            return EmptyPageableListOfItems<OdpUserEducation>.Value;
        }

        var query = _teamBoardDbContext.UserEducations
            .Where(education => education.UserId == request.UserId)
            .Select(OdpUserEducation.MapUserEducation);

        var result = await request.QueryOptions
            .ApplyTo(query)
            .Cast<OdpUserEducation>()
            .ToListAsync(cancellationToken);

        var totalItems = await request.QueryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpUserEducation>()
            .CountAsync(cancellationToken);

        if (totalItems == 0)
        {
            return EmptyPageableListOfItems<OdpUserEducation>.Value;
        }

        MakeEmptyCommentSection(request, result);

        return result.ToPageable()
            .WithPaging(page: page, pageSize: pageSize)
            .WithTotal(totalItems);
    }

    private void MakeEmptyCommentSection(Query request, List<OdpUserEducation> result)
    {
        var userRoles = _authenticatedUserContext.GetRoles();
        var isRequestFromUserProfile = _authenticatedUserContext.UserId == request.UserId && (userRoles.IsEmployee() || userRoles.IsSystemAdministrator());
        if (isRequestFromUserProfile)
        {
            foreach (var education in result.ToList())
            {
                education.Comment = string.Empty;
            }
        }
    }

    private async Task<bool> ShouldReturnEmptyAsync(Query request, CancellationToken cancellationToken)
    {
        var currentUserId = _authenticatedUserContext.UserId;
        var roles = _authenticatedUserContext.GetRoles();
        var isEmployeeOrSysAdmin = roles.IsEmployee() || roles.IsSystemAdministrator();

        if (_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name))
        {
            return false;
        }

        if (currentUserId != request.UserId && isEmployeeOrSysAdmin)
        {
            return true;
        }

        var userDepartmentAndGroup = await _teamBoardDbContext.TeamBoardUsers
            .Where(x => x.UserId == request.UserId)
            .Select(user => new { Department = user.DepartmentId, ResourceGroup = user.ResourceGroupId })
            .FirstOrDefaultAsync(cancellationToken);

        var isUserFromAnotherDepartment = roles.IsHeadOfDepartment() &&
            userDepartmentAndGroup?.Department != _authenticatedUserContext.DepartmentId;

        var isUserFromAnotherGroup = roles.IsGroupManager()
            && userDepartmentAndGroup?.ResourceGroup != _authenticatedUserContext.ResourceGroupId
            && request.UserId != currentUserId;

        if (isUserFromAnotherDepartment || isUserFromAnotherGroup)
        {
            return true;
        }

        return false;
    }

    internal sealed record Query(int UserId, ODataQueryOptions<OdpUserEducation> QueryOptions)
        : IQuery<PageableListOfItems<OdpUserEducation>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
