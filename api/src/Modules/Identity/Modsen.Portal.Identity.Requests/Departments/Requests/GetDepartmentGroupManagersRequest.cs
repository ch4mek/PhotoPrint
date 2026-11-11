using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Departments.Requests.Dto;

namespace Modsen.Portal.Identity.Requests.Departments.Requests;

internal sealed class GetDepartmentGroupManagersRequest :
    IQueryHandler<GetDepartmentGroupManagersRequest.Query, List<DepartmentGroupManagerDto>>
{
    private readonly IIdentityDbContext _identityDbContext;

    public GetDepartmentGroupManagersRequest(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public Task<List<DepartmentGroupManagerDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var query = _identityDbContext.Users
           .Where(user => user.IsActive &&
               user.DepartmentId == request.DepartmentId &&
               user.UserRoles.Any(r => r.Role!.Name == PortalRoleNames.GroupManager))
           .AsQueryable();

        query = request.HasActiveGroup switch
        {
            null => query,

            false => query
                .Include(user => user.ResourceGroup)
                .Where(user => user.ResourceGroupId == null || user.ResourceGroup!.IsActive == request.HasActiveGroup),

            true => query
                .Include(user => user.ResourceGroup)
                .Where(user => user.ResourceGroup!.IsActive == request.HasActiveGroup)
        };

        return query
            .Select(gm => new DepartmentGroupManagerDto
            {
                Id = gm.Id,
                FirstName = gm.FirstName,
                LastName = gm.LastName
            })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(string DepartmentId, bool? HasActiveGroup)
        : IQuery<List<DepartmentGroupManagerDto>>;
}
