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
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.OData;

namespace Modsen.Portal.Identity.Requests.ResourceGroups.Requests;

internal sealed class GetResourceGroupMembersRequest : IQueryHandler<GetResourceGroupMembersRequest.Query, PageableListOfItems<OdpGroupMember>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;

    public GetResourceGroupMembersRequest(IAuthenticatedUserContext authenticatedUserContext,
        IIdentityDbContext identityDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityDbContext = identityDbContext;
    }

    public async Task<PageableListOfItems<OdpGroupMember>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            var groupDepartment = await _identityDbContext.ResourceGroups
                    .Where(group => group.Id == request.Id)
                    .Select(group => group.DepartmentId)
                    .FirstOrDefaultAsync(cancellationToken)
                ?? throw new EntityNotFoundException();

            if (groupDepartment != _authenticatedUserContext.DepartmentId)
            {
                throw new ActionNotAllowedException();
            }
        }

        var query = _identityDbContext.Users
            .AsQueryable()
            .Where(user => user.IsActive && user.ResourceGroupId == request.Id)
            .Where(user => user.UserRoles.First().Role!.Name == PortalRoleNames.Employee)
            .Select(OdpGroupMember.Map);

        var total = await request.QueryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpGroupMember>()
            .CountAsync(cancellationToken);

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpGroupMember>.Value;
        }

        var result = await request.QueryOptions
            .ApplyTo(query)
            .Cast<OdpGroupMember>()
            .ToListAsync(cancellationToken);

        return result.ToPageable(request.QueryOptions, total);
    }

    internal sealed record Query(int Id, ODataQueryOptions<OdpGroupMember> QueryOptions)
        : IQuery<PageableListOfItems<OdpGroupMember>>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
