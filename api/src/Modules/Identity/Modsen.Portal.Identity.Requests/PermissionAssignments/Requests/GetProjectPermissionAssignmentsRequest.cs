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
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.PermissionAssignments.Requests;

internal sealed class GetProjectPermissionAssignmentsRequest
    : IQueryHandler<GetProjectPermissionAssignmentsRequest.Query, PageableListOfItems<ProjectPermissionAssignmentCoreDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;

    public GetProjectPermissionAssignmentsRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityDbContext identityDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityDbContext = identityDbContext;
    }

    public async Task<PageableListOfItems<ProjectPermissionAssignmentCoreDto>> Handle(Query request,
        CancellationToken cancellationToken)
    {
        var userDepartmentId = _authenticatedUserContext.DepartmentId;

        var query = _identityDbContext.PermissionAssignments
            .AsNoTracking()
            .Include(a => a.Permission)
            .Include(a => a.User)
            .Where(a => a.ResourceType == nameof(PermissionResourceType.Project))
            .Where(a => a.User!.IsActive)
            .Select(ProjectPermissionAssignmentCoreDto.MapPermissionAssign);

        query = userDepartmentId is not null
            ? query.Where(x => x.UserDepartmentId == userDepartmentId)
            : query;

        var queryOptionsWithoutPagination = request.QueryOptions.RebuildWithoutPagination();
        var totalCount = await queryOptionsWithoutPagination
            .ApplyTo(query)
            .OfType<ProjectPermissionAssignmentCoreDto>()
            .CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return EmptyPageableListOfItems<ProjectPermissionAssignmentCoreDto>.Value;
        }

        var result = await request.QueryOptions
            .ApplyTo(query)
            .Cast<ProjectPermissionAssignmentCoreDto>()
            .ToListAsync(cancellationToken);
     
        return result.ToPageable(request.QueryOptions, totalCount);
    }

    internal sealed record Query(ODataQueryOptions<ProjectPermissionAssignmentCoreDto> QueryOptions)
        : IQuery<PageableListOfItems<ProjectPermissionAssignmentCoreDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
