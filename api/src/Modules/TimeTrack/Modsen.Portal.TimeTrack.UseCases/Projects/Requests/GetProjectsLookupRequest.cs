using System.Globalization;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Projects.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetProjectsLookupRequest : IQueryHandler<GetProjectsLookupRequest.Query, List<ProjectLookupDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICanUserBeProjectManagerBusinessRule _canUserBeProjectManagerBusinessRule;
    private readonly IProjectQueryBuilder _projectQueryBuilder;
    public GetProjectsLookupRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ICanUserBeProjectManagerBusinessRule canUserBeProjectManagerBusinessRule,
        IProjectQueryBuilder projectQueryBuilder)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _canUserBeProjectManagerBusinessRule = canUserBeProjectManagerBusinessRule;
        _projectQueryBuilder = projectQueryBuilder;
    }

    public async Task<List<ProjectLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (_authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ViewProjectExits.Name)
            && !_authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name)
            && !_authenticatedUserContext.IsCeo()
            && !_authenticatedUserContext.IsCto()
            && !_authenticatedUserContext.IsHeadOfDepartment()
            && !_authenticatedUserContext.IsGroupManager())
        {
            return [];
        }

        if (!_authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name))
        {
            await ValidateRequestAsync(request, cancellationToken);
        }

        var projects = await GetProjectsAsync(request, cancellationToken);

        foreach (var project in projects)
        {
            project.IsCurrentUserAbleToSeeBillableTrackedTime = _authenticatedUserContext.IsManager ||
                _authenticatedUserContext.HasClaimWithValue(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject, project.Id.ToString(CultureInfo.InvariantCulture));
        }

        return projects;
    }

    private Task<List<ProjectLookupDto>> GetProjectsAsync(
        Query request,
        CancellationToken cancellationToken)
    {
        var queryBase = _projectQueryBuilder
            .WithKeyWord(request.KeyWord)
            .WithActiveStatus(request.IsActive);

        var query = _authenticatedUserContext.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name)
            ? queryBase.WithPublicStatus(request.IsPublic).Build()
            : _authenticatedUserContext.GetRoles() switch
            {
                var roles when roles.IsCeo() || roles.IsCto() =>
                    queryBase
                    .WithPublicStatus(request.IsPublic)
                    .Build(),

                var roles when
                    (roles.IsHeadOfDepartment() || roles.IsEmployee())
                    && request.OnlyManagedProjects.HasValue && request.OnlyManagedProjects!.Value =>
                    queryBase
                        .WithPublicStatus(request.IsPublic)
                        .WithManagerId(_authenticatedUserContext.UserId)
                        .Build(),

                var roles when roles.IsHeadOfDepartment() =>
                        HeadFilter(request, queryBase.Build()),

                var roles when roles.IsGroupManager() =>
                    GmFilter(request, queryBase.Build()),


                var roles when roles.IsSystemAdministrator() || roles.IsEmployee() =>
                    SysAdminFilter(request, queryBase.Build()),

                _ => throw new InvalidOperationException()
            };

        return query.OrderBy(project => project.Id)
            .Select(project => new ProjectLookupDto
            {
                Id = project.Id,
                Name = project.Name,
                ClientName = project.Client!.Name,
                IsBillable = project.IsBillable,
                IsCurrentUserAbleToSeeBillableTrackedTime = false,
                Color = project.Color
            }).ToListAsync(cancellationToken);
    }

    private IQueryable<Project> HeadFilter(Query request, IQueryable<Project> projects)
    {
        Expression<Func<Project, bool>> filterExpression;
        if (request.IsPublic.HasValue)
        {
            filterExpression = (bool)request.IsPublic ?
                project => project.IsPublic :
                project => !project.IsPublic &&
                (project.CreatedById == _authenticatedUserContext.UserId ||
                 project.ProjectUsers.Any(pu => pu.User!.DepartmentId == _authenticatedUserContext.DepartmentId!));
        }
        else
        {
            filterExpression = project =>
                project.IsPublic
                || project.CreatedById == _authenticatedUserContext.UserId
                || project.ProjectUsers.Any(pu => pu.User!.DepartmentId == _authenticatedUserContext.DepartmentId!);
        }
        return projects
            .Where(filterExpression);
    }

    private IQueryable<Project> GmFilter(Query request, IQueryable<Project> projects)
    {
        if (request.IsPublic.HasValue)
        {
            return projects.Where(project => request.IsPublic.Value ?
                project.IsPublic : !project.IsPublic &&
                (project.CreatedById == _authenticatedUserContext.UserId ||
                 project.ProjectUsers.Any(pu => pu.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId!.Value)));
        }
        else
        {
            return projects.Where(project =>
                project.IsPublic
                || project.CreatedById == _authenticatedUserContext.UserId
                || project.ProjectUsers.Any(pu => pu.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId!.Value));
        }
    }

    private IQueryable<Project> SysAdminFilter(Query request, IQueryable<Project> projects)
    {
        Expression<Func<Project, bool>> filterExpression;
        if (request.IsPublic.HasValue)
        {
            filterExpression = (bool)request.IsPublic ?
                project => project.IsPublic :
                project => !project.IsPublic &&
                project.ProjectUsers.Any(pu => pu.UserId == _authenticatedUserContext.GetRequiredUserId());
        }
        else
        {
            filterExpression = project =>
                project.IsPublic
                || project.ProjectUsers.Any(pu => pu.UserId == _authenticatedUserContext.GetRequiredUserId());
        }
        return projects
            .Where(filterExpression);
    }


    private async Task ValidateRequestAsync(Query request, CancellationToken cancellationToken)
    {
        if (!_authenticatedUserContext.GetRoles().IsEmployee() &&
            !_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            request.OnlyManagedProjects != null)
        {
            throw new ActionNotAllowedException(PortalMessages.OnlyManagedUsersUsageRestriction);
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee() && request.OnlyManagedProjects is true)
        {
            var canUserBeManager = await _canUserBeProjectManagerBusinessRule
                .IsCorrespondsToBusiness(_authenticatedUserContext.GetRequiredUserId(), cancellationToken);
            if (!canUserBeManager)
            {
                throw new ActionNotAllowedException(PortalMessages.UserCantBeManager);
            }
        }
    }

    internal sealed record Query(string? KeyWord, bool? IsPublic, bool? IsActive, bool? OnlyManagedProjects)
        : IQuery<List<ProjectLookupDto>>, ITimeTrackRequest;
}
