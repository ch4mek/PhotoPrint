using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Clients;

internal class GetPageableClientsDbQuery : IGetPageableClientsDbQuery
{
    private readonly ITimeTrackDbContext _dbContext;

    public GetPageableClientsDbQuery(ITimeTrackDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PageableListOfItems<PageableClientDbModel>> ExecuteAsync(
        GetPageableClientsDbRequest request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Clients.Select(Client.MapClient).AsQueryable();

        if (request.DepartmentId is not null)
        {
            var ids = await _dbContext
                .Clients
                .Where(x => x.Projects.Any(project => project.CreatedById == request.ClientHasProjectCreatedByUserId)
                    || x.Projects.Any(p => p.IsPublic
                        || p.ProjectUsers.Any(u => u.User != null && u.User.DepartmentId == request.DepartmentId)))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            query = query.Where(x => ids.Contains(x.Id));
        }

        if (request.ClientHasProjectWithUsersFromResourceGroupId is not null)
        {
            var ids = await _dbContext
                .Clients
                .Where(x => x.Projects.Any(project => project.CreatedById == request.ClientHasProjectCreatedByUserId)
                    || x.Projects.Any(p => p.IsPublic
                        || p.ProjectUsers.Any(u => u.User != null && u.User.ResourceGroupId == request.ClientHasProjectWithUsersFromResourceGroupId)))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            query = query.Where(x => ids.Contains(x.Id));
        }

        var result = await request.QueryOptions
            .ApplyTo(query)
            .Cast<OdpClient>()
            .ToListAsync(cancellationToken);

        var totalCount = await request.QueryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpClient>()
            .CountAsync(cancellationToken);

        var clients = result.Select(x => new PageableClientDbModel
            (
                x.Id,
                x.Name,
                x.Color,
                x.Comments,
                x.FactTrackedTimeInMinutes,
                x.BillableTrackedTimeInMinutes,
                x.IsActive,
                x.IsProjectExist,
                x.HasActiveProject,
                x.RegisteredLocation,
                x.SalesManagerId
            ))
            .ToList();

        return clients.ToPageable(request.QueryOptions, totalCount);
    }
}
