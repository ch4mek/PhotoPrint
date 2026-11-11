using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Clients.Requests;

internal sealed class GetClientByIdRequest : IQueryHandler<GetClientByIdRequest.Query, ClientDto>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetClientByIdRequest(
        ITimeTrackDbContext timeTrackDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<ClientDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var currentUserRoles = _authenticatedUserContext.GetRoles();
        var currentUserId = _authenticatedUserContext.GetRequiredUserId();
        var currentUserDepartmentId = _authenticatedUserContext.DepartmentId;
        var currentResourceGroupId = _authenticatedUserContext.ResourceGroupId;
        var isHasAccessToRegisteredLocation = _authenticatedUserContext.GetRoles().IsCeo() || _authenticatedUserContext.GetRoles().IsCto();
        var clients = _timeTrackDbContext.Clients.AsNoTracking();

        if (currentUserRoles.IsHeadOfDepartment())
        {
            clients = clients.Where(x => x.Projects.Any(c => c.ProjectUsers
                    .Any(v => v.User!.DepartmentId == currentUserDepartmentId)) ||
                        x.Projects.Any(c => c.IsPublic) ||
                        x.CreatedById == currentUserId);
        }

        if (currentUserRoles.IsGroupManager())
        {
            clients =  clients.Where(x => x.Projects.Any(c => c.ProjectUsers
                    .Any(v => v.User!.ResourceGroupId == currentResourceGroupId)) ||
                        x.Projects.Any(c => c.IsPublic) ||
                        x.CreatedById == currentUserId);
        }

        if (currentUserRoles.IsEmployee() || currentUserRoles.IsSystemAdministrator())
        {
            clients = clients.Where(x => x.Projects.Any(c => c.ProjectUsers
                .Any(v => v.UserId == currentUserId)));
        }

        var client = await clients
            .Where(client => client.Id == request.ClientId)
            .Select(client => new ClientDto
            {
                Id = client.Id,
                Name = client.Name,
                LegalEntity = client.LegalEntity,
                RegisteredLocation = isHasAccessToRegisteredLocation ? client.RegisteredLocation : null,
                Color = client.Color,
                Comments = client.Comments,
                HasActiveAssignements = null,
                HasActiveProject = null,
                IsActive = client.IsActive,
                BillableTrackedTimeInMinutes = client.BillableTrackedTime,
                FactTrackedTimeInMinutes = client.FactTrackedTime,
                IsProjectExist = client.Projects.Count != 0,
                SalesManagerId = client.SalesManagerId,
                SalesManagerFullName = client.SalesManagerUser == null
                ? null
                : client.SalesManagerUser.LastName + " " + client.SalesManagerUser.FirstName
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (client is null)
        {
            throw new EntityNotFoundException(PortalMessages.ClientNotFound);
        }

        return client;
    }

    internal sealed record Query(int ClientId) : IQuery<ClientDto>, ITimeTrackRequest;
}
