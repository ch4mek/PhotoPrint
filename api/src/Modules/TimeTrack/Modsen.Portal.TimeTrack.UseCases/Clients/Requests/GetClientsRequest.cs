using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Clients.Requests;

internal sealed class GetClientsRequest : IQueryHandler<GetClientsRequest.Query, PageableListOfItems<ClientDto>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    public GetClientsRequest(
        ITimeTrackDbContext timeTrackDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PageableListOfItems<ClientDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var clientHasProjectCreatedByUserId = _authenticatedUserContext.GetRoles().IsHeadOfDepartment() ||
            _authenticatedUserContext.GetRoles().IsGroupManager()
            ? _authenticatedUserContext.GetRequiredUserId()
            : 0;

        var clientHasProjectWithUsersFromResourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager()
            ? _authenticatedUserContext.ResourceGroupId
            : null;

        var clientHasProjectWithUsersFromDepartmentId = _authenticatedUserContext.GetRoles().IsHeadOfDepartment()
            ? _authenticatedUserContext.DepartmentId
            : null;

        var clientsPages = await GetPageableClients(
            clientHasProjectWithUsersFromDepartmentId,
            clientHasProjectCreatedByUserId,
            clientHasProjectWithUsersFromResourceGroupId,
            request.QueryOptions,
            cancellationToken);

        var clientItems = clientsPages.Items.ToList();

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment() || _authenticatedUserContext.GetRoles().IsGroupManager())
        {
            var activeAssignmentsClientsIds =  await _timeTrackDbContext.ProjectUsers
                .Include(x => x.User)
                .Include(x => x.Project)
                .ThenInclude(x => x!.Client)
                .Where(x =>
                    x.Project!.IsActive &&
                    (_authenticatedUserContext.DepartmentId == null || x.User!.DepartmentId == _authenticatedUserContext.DepartmentId) &&
                    (_authenticatedUserContext.ResourceGroupId == null || x.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId)
                )
                .Select(x => x.Project!.ClientId)
                .ToListAsync(cancellationToken);

            clientItems.ForEach(client =>
            {
                client.HasActiveAssignements =  activeAssignmentsClientsIds.Contains(client.Id);
                client.FactTrackedTimeInMinutes = null;
                client.BillableTrackedTimeInMinutes = null;
                client.RegisteredLocation = null;
            });
        }

        return clientItems.ToPageable()
            .WithPaging(clientsPages.Page, clientsPages.PageSize)
            .WithTotal(clientsPages.Total);
    }


    private async Task<PageableListOfItems<ClientDto>> GetPageableClients(
        string? departmentId,
        int clientHasProjectCreatedByUserId,
        int? clientHasProjectWithUsersFromResourceGroupId,
        ODataQueryOptions<OdpClient> queryOptions,
        CancellationToken cancellationToken)
    {
        var query = _timeTrackDbContext.Clients.Select(Client.MapClient).AsQueryable();

        if (departmentId is not null)
        {
            var ids = await _timeTrackDbContext
                .Clients
                .Where(x => x.Projects.Any(project => project.CreatedById == clientHasProjectCreatedByUserId)
                    || x.Projects.Any(p => p.IsPublic
                        || p.ProjectUsers.Any(u => u.User != null && u.User.DepartmentId == departmentId)))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            query = query.Where(x => ids.Contains(x.Id));
        }

        if (clientHasProjectWithUsersFromResourceGroupId is not null)
        {
            var ids = await _timeTrackDbContext
                .Clients
                .Where(x => x.Projects.Any(project => project.CreatedById == clientHasProjectCreatedByUserId)
                    || x.Projects.Any(p => p.IsPublic
                        || p.ProjectUsers.Any(u => u.User != null && u.User.ResourceGroupId == clientHasProjectWithUsersFromResourceGroupId)))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            query = query.Where(x => ids.Contains(x.Id));
        }

        var clients = await queryOptions
            .ApplyTo(query)
            .Cast<OdpClient>()
            .Select(x => new ClientDto
            {
                Id = x.Id,
                Name = x.Name,
                LegalEntity = x.LegalEntity,
                Color = x.Color,
                Comments = x.Comments,
                FactTrackedTimeInMinutes = x.FactTrackedTimeInMinutes,
                BillableTrackedTimeInMinutes = x.BillableTrackedTimeInMinutes,
                IsActive = x.IsActive,
                IsProjectExist = x.IsProjectExist,
                HasActiveProject = x.HasActiveProject,
                RegisteredLocation = x.RegisteredLocation,
                SalesManagerId = x.SalesManagerId
            })
            .ToListAsync(cancellationToken);

        var totalCount = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpClient>()
            .CountAsync(cancellationToken);

        return clients.ToPageable(queryOptions, totalCount);
    }

    internal sealed record Query(ODataQueryOptions<OdpClient> QueryOptions)
        : IQuery<PageableListOfItems<ClientDto>>, ITimeTrackRequest;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
