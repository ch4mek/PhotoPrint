using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetUsersTabRequest : IQueryHandler<GetUsersTabRequest.Query, PageableListOfItems<UserCoreDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly ITimeTrackContract _timeTrackContract;

    public GetUsersTabRequest(IAuthenticatedUserContext authenticatedUserContext, IIdentityDbContext identityDbContext,
        ITimeTrackContract timeTrackContract)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityDbContext = identityDbContext;
        _timeTrackContract = timeTrackContract;
    }

    public async Task<PageableListOfItems<UserCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var pageableUsers = await GetPageableUsersAsync(
            request.QueryOptions,
            _authenticatedUserContext.DepartmentId,
            _authenticatedUserContext.ResourceGroupId,
            cancellationToken);

        return pageableUsers;
    }

    internal sealed record Query(ODataQueryOptions<UserCoreDto> QueryOptions) : IQuery<PageableListOfItems<UserCoreDto>>, IIdentityRequest;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }

    private async Task<PageableListOfItems<UserCoreDto>> GetPageableUsersAsync(
        ODataQueryOptions<UserCoreDto> queryOptions,
        string? departmentId,
        int? resourceGroupId,
        CancellationToken cancellationToken)
    {
        var query = _identityDbContext.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
            .Include(u => u.Department)
            .Include(u => u.ResourceGroup)
            .Select(UserCoreDto.MapUser);

        query = query.Where(u => u.Id != _authenticatedUserContext.GetRequiredUserId());

        if (!_authenticatedUserContext.HasClaim(HrClaimTypeNames.ManageHrModule.Name) &&
            !_authenticatedUserContext.IsCeo() &&
            !_authenticatedUserContext.IsCto() &&
            !_authenticatedUserContext.HasClaim(AccountantClaimTypeNames.ManageAccountant.Name))
        {
            if (_authenticatedUserContext.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name) && _authenticatedUserContext.IsHeadOfDepartment())
            {
                var coordinatorManagedUserIds = await _timeTrackContract.GetUserIdsByProjectManagerAsync(_authenticatedUserContext.GetRequiredUserId(), cancellationToken);

                query = query.Where(u => coordinatorManagedUserIds.Contains(u.Id) || u.DepartmentId == departmentId);
            }
            else if (_authenticatedUserContext.HasClaim(CoordinatorClaimTypeNames.ManageProjectCoordinator.Name))
            {
                var coordinatorManagedUserIds = await _timeTrackContract.GetUserIdsByProjectManagerAsync(_authenticatedUserContext.GetRequiredUserId(), cancellationToken);

                query = query.Where(u => coordinatorManagedUserIds.Contains(u.Id));
            }
            else if (_authenticatedUserContext.IsHeadOfDepartment())
            {
                query = query.Where(x => x.DepartmentId == departmentId);
            }
            else
            {
                query = _authenticatedUserContext.IsGroupManager() ? query.Where(x => x.ResourceGroupId == resourceGroupId) : query;
            }
        }
        else if (_authenticatedUserContext.IsCto())
        {
            query = query.Where(x => x.Position != PortalRoleNames.Ceo);
        }

        var result = await queryOptions
            .ApplyTo(query)
            .Cast<UserCoreDto>()
            .ToListAsync(cancellationToken);

        var queryOptionsWithoutPagination = queryOptions
            .RebuildWithoutPagination();

        var total = await queryOptionsWithoutPagination
            .ApplyTo(query)
            .OfType<UserCoreDto>()
            .CountAsync(cancellationToken);

        return result.ToPageable(queryOptions, total);
    }
}
