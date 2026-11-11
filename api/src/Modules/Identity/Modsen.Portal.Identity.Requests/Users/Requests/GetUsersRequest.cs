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

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetUsersRequest : IQueryHandler<GetUsersRequest.Query, PageableListOfItems<UserCoreDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;

    public GetUsersRequest(IAuthenticatedUserContext authenticatedUserContext, IIdentityDbContext identityDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityDbContext = identityDbContext;
    }

    public async Task<PageableListOfItems<UserCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var pageableUsers = await GetPageableUsersAsync(
            request.QueryOptions,
            _authenticatedUserContext.DepartmentId,
            _authenticatedUserContext.ResourceGroupId,
            _authenticatedUserContext.GetRoles(),
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
        IReadOnlyCollection<string> roleName,
        CancellationToken cancellationToken)
    {
        var query = _identityDbContext.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
            .Include(u => u.Department)
            .Include(u => u.ResourceGroup)
            .Select(UserCoreDto.MapUser);

        query = roleName.IsHeadOfDepartment() ? query.Where(x => x.DepartmentId == departmentId) : query;

        query = roleName.IsGroupManager() ? query.Where(x => x.ResourceGroupId == resourceGroupId) : query;

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
