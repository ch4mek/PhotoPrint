using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Identity.DataAccess.Npgsql;

namespace Modsen.Portal.Identity.Requests.Departments.Requests;

internal sealed class GetUsersWithManagersRequest : IQueryHandler<GetUsersWithManagersRequest.Query, List<UserWithManagersDto>>
{
    private readonly IIdentityDbContext _identityDbContext;

    public GetUsersWithManagersRequest(IIdentityDbContext identityDbContext)
    {
        _identityDbContext = identityDbContext;
    }

    public async Task<List<UserWithManagersDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var groupManagers = _identityDbContext.Users.AsNoTracking()
            .Where(x => x.IsActive)
            .Where(x => x.UserRoles.Any(ur => ur.Role!.Name == PortalRoleNames.GroupManager))
            .Select(x => new
            {
                x.ResourceGroupId,
                x.Id
            });

        var chiefRoles = _identityDbContext.Users.AsNoTracking()
            .Where(x => x.IsActive)
            .Where(x => x.UserRoles.Any(x => x.Role!.Name == PortalRoleNames.Cto || x.Role.Name == PortalRoleNames.Ceo))
            .Select(x => new
            {
                UserId = x.Id,
                Position = x.UserRoles.First().Role!.Name,
                x.UserRoles.First().RoleId,
            });

        var headsOfDepartments = _identityDbContext.Users.AsNoTracking()
            .Where(x => x.IsActive)
            .Where(x => x.UserRoles.Any(ur => ur.Role!.Name == PortalRoleNames.HeadOfDepartment))
            .Select(x => new
            {
                x.DepartmentId,
                x.Id,
            });

        var departments = _identityDbContext.Departments.AsNoTracking()
            .GroupJoin(headsOfDepartments,
                d => d.Id,
                h => h.DepartmentId,
                (d, hs) => new
                {
                    d.Id,
                    d.DepartmentReportingToRoleId,
                    HeadOfDepartmentUserId = hs.Select(x => x.Id).FirstOrDefault()
                });

        var headsChiefs = departments
            .Join(chiefRoles,
                d => d.DepartmentReportingToRoleId,
                c => c.RoleId,
                (d, c) => new
                {
                    DepartmentId = d.Id,
                    d.HeadOfDepartmentUserId,
                    ChiefId = c.UserId
                });

        var usersQueryFor = _identityDbContext.Users.AsNoTracking()
            .Where(x => x.IsActive);

        if (request.UserIds != null)
        {
            usersQueryFor = usersQueryFor.Where(x => request.UserIds.Contains(x.Id));
        }

        var usersFor = await usersQueryFor.ToListAsync(cancellationToken);

        var ceoId = await chiefRoles
            .Where(x => x.Position == PortalRoleNames.Ceo)
            .Select(x => x.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        var userWithManagers = usersFor
            .GroupJoin(headsChiefs,
                u => u.DepartmentId,
                hc => hc.DepartmentId,
                (u, hcs) => new
                {
                    User = u,
                    HeadChiefs = hcs
                })
            .GroupJoin(groupManagers,
                uhc => uhc.User.ResourceGroupId,
                g => g.ResourceGroupId,
                (uhc, gs) => new
                {
                    uhc.User,
                    uhc.HeadChiefs,
                    GroupManagers = gs
                })
            .Select(x => new UserWithManagersDto
            {
                UserId = x.User.Id,
                ResourceGroupId = x.User.ResourceGroupId,
                DepartmentId = x.User.DepartmentId,

                HeadOfDepartmentUserId = x.HeadChiefs.Select(x => x.HeadOfDepartmentUserId).FirstOrDefault(),
                ChiefId = x.HeadChiefs.Select(x => x.ChiefId).FirstOrDefault(defaultValue: ceoId),
                GroupManagerId = x.GroupManagers.Select(x => x.Id).FirstOrDefault()
            })
            .ToList();

        foreach (var entry in userWithManagers)
        {
            if (entry.HeadOfDepartmentUserId == entry.UserId)
            {
                entry.HeadOfDepartmentUserId = 0;
            }

            if (entry.GroupManagerId == entry.UserId)
            {
                entry.GroupManagerId = 0;
            }
        }

        return userWithManagers;
    }

    internal sealed record Query(int[]? UserIds = null) : IQuery<List<UserWithManagersDto>>;
}
