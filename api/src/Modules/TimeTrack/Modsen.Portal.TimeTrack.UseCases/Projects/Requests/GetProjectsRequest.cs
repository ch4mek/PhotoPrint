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
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetProjectsRequest : IQueryHandler<GetProjectsRequest.Query, PageableListOfItems<ProjectWithUserInfoAndTimeDto>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetProjectsRequest(
        ITimeTrackDbContext timeTrackDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PageableListOfItems<ProjectWithUserInfoAndTimeDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var resourceGroupId = _authenticatedUserContext.ResourceGroupId;
        var userId = _authenticatedUserContext.GetRequiredUserId();
        var departmentId = _authenticatedUserContext.DepartmentId;

        var projects = await GetProjectsAsync(
            resourceGroupId,
            userId,
            departmentId,
            request.QueryOptions,
            cancellationToken);

        if (projects.Total == 0)
        {
            return EmptyPageableListOfItems<ProjectWithUserInfoAndTimeDto>.Value;
        }

        var roles = _authenticatedUserContext.GetRoles();
        var isHodOrGm = roles.IsHeadOfDepartment() || roles.IsGroupManager();

        var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

        return projects.Items
            .Select(project => new ProjectWithUserInfoAndTimeDto
            {
                Id = project.Id,
                Name = project.Name,
                Color = project.Color,
                ClientId = project.ClientId,
                ClientName = project.ClientName,
                FactTrackedTime = isHodOrGm ? null : project.FactTrackedTime,
                BillableTrackedTime = isHodOrGm ? null : project.BillableTrackedTime,
                IsPublic = project.IsPublic,
                IsBillable = project.IsBillable,
                IsActive = project.IsActive,
                ProjectUsers = project.ProjectUsers
                    .Where(user => (departmentId == null || user.DepartmentId == departmentId)
                        && (resourceGroupId == null || user.ResourceGroupId == resourceGroupId))
                    .Where(user => user.IsActive && (!user.UserOnProjectTo.HasValue || dateNow <= user.UserOnProjectTo))
                    .Select(projectUser => new ProjectUserInfoDto
                    {
                        Id = projectUser.UserId,
                        FirstName = projectUser.FirstName,
                        LastName = projectUser.LastName,
                        DepartmentId = projectUser.DepartmentId
                    })
                    .ToList()
            })
            .ToPageable()
            .WithPaging(projects.Page, projects.PageSize)
            .WithTotal(projects.Total);
    }

    private async Task<PageableListOfItems<OdpProject>> GetProjectsAsync(
        int? resourceGroupId,
        int userId,
        string? departmentId,
        ODataQueryOptions<OdpProject> queryOptions,
        CancellationToken cancellationToken)
    {
        var query = _timeTrackDbContext.Projects
           .AsNoTracking()
           .Select(Project.MapOdpProject)
           .AsQueryable();

        if (resourceGroupId is not null)
        {
            var projectIds = _timeTrackDbContext
                .Projects
                .AsNoTracking()
                .Where(project => project.ProjectUsers.Any(projectUser => projectUser.User!.ResourceGroupId == resourceGroupId)
                    || project.IsPublic
                    || project.CreatedById == userId)
                .Select(x => x.Id);

            query = query.Where(x => projectIds.Contains(x.Id));
        }

        if (departmentId is not null)
        {
            var projectIds = _timeTrackDbContext
                .Projects
                .AsNoTracking()
                .Where(project => project.ProjectUsers.Any(projectUser => projectUser.User!.DepartmentId == departmentId)
                    || project.IsPublic
                    || project.CreatedById == userId)
                .Select(x => x.Id);

            query = query.Where(x => projectIds.Contains(x.Id));
        }

        var totalCount = await queryOptions
            .RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpProject>()
            .CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return EmptyPageableListOfItems<OdpProject>.Value;
        }

        var projects = await queryOptions
            .ApplyTo(query)
            .Cast<OdpProject>()
            .ToListAsync(cancellationToken);

        return projects.ToPageable(queryOptions, totalCount);
    }

    internal sealed record Query(ODataQueryOptions<OdpProject> QueryOptions)
        : IQuery<PageableListOfItems<ProjectWithUserInfoAndTimeDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }

}
