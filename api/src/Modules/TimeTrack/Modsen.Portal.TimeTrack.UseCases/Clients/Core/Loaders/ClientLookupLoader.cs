using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.TimeTrack.UseCases.Clients.Core.Loaders;

internal interface IClientLookupLoader
{
    Task<List<ClientLookupResponse>> LoadAsync(ClientLookupRequest request, CancellationToken cancellationToken);
}

internal sealed class ClientLookupLoader : IClientLookupLoader
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public ClientLookupLoader(ITimeTrackDbContext timeTrackDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
    }

    public Task<List<ClientLookupResponse>> LoadAsync(ClientLookupRequest request, CancellationToken cancellationToken)
    {
        var query = _timeTrackDbContext.Clients
            .AsNoTracking();

        if (!request.KeyWord.IsNullOrWhiteSpace())
        {
            query = query.Where(client => EF.Functions.Like(client.Name, $"{request.KeyWord}%"));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(client => client.IsActive == request.IsActive);
        }

        if (!request.ForceAll)
        {
            query = request.Roles switch
            {
                var roles when roles.IsCto() || roles.IsCeo() => query,

                var roles when (roles.IsHeadOfDepartment() || roles.IsEmployee()) && request.OnlyManagedClients.HasValue &&
                    request.OnlyManagedClients!.Value =>
                    query.Where(client => client.Projects.Any(project => project.ManagerUserId == request.CurrentUserId)),

                var roles when roles.IsGroupManager() =>
                    query.Where(client => client.Projects.Any(c => c.IsPublic) ||
                        client.Projects.Any(project => project.CreatedById == request.CurrentUserId) ||
                        client.Projects.Any(project => project.ProjectUsers
                            .Any(projectUser => projectUser.User!.ResourceGroupId == request.ResourceGroup))),

                var roles when roles.IsHeadOfDepartment() =>
                    query.Where(client => client.Projects.Any(c => c.IsPublic) ||
                        client.Projects.Any(project => project.CreatedById == request.CurrentUserId) ||
                        client.Projects.Any(project => project.ProjectUsers
                            .Any(projectUser => projectUser.User!.DepartmentId == request.DepartmentId))
                ),

                var roles when roles.IsEmployee() || roles.IsSystemAdministrator() =>
                    query.Where(client => client.Projects.Any(project => project.ProjectUsers
                        .Any(projectUser => projectUser.UserId == request.CurrentUserId))),

                _ => throw new InvalidOperationException(),
            };
        }

        return query.OrderBy(client => client.Id)
            .Select(x => new ClientLookupResponse(x.Id, x.Name))
            .ToListAsync(cancellationToken);
    }
}

internal sealed record ClientLookupRequest(
    IReadOnlyCollection<string> Roles,
    string? KeyWord,
    bool? IsActive,
    bool? OnlyManagedClients,
    int CurrentUserId,
    int? ResourceGroup,
    string? DepartmentId,
    bool ForceAll = false);

public record ClientLookupResponse(int Id, string Name);
