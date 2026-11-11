using System.Globalization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Dtos;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;
using Project = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.Project;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetUserProjectsWithDisabledDaysRequest
    : IQueryHandler<GetUserProjectsWithDisabledDaysRequest.Query, List<ProjectWithDisabledDaysDto>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IEnvironmentFeatureContext _environmentFeatureContext;

    public GetUserProjectsWithDisabledDaysRequest(
        ITimeTrackDbContext timeTrackDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IEnvironmentFeatureContext environmentFeatureContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _environmentFeatureContext = environmentFeatureContext;
    }

    public async Task<List<ProjectWithDisabledDaysDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var userInfo = await _timeTrackDbContext.ProjectUsers
            .Where(projectUser => projectUser.UserId == request.UserId)
            .Select(projectUser => new
            {
                projectUser.UserId,
                DepartmentId = projectUser.User!.DepartmentId!,
                projectUser.User.ResourceGroupId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userInfo is null)
        {
            return [];
        }

        var currentUserId = _authenticatedUserContext.GetRequiredUserId();

        var isRequestedManagedProjects = (_authenticatedUserContext.GetRoles().IsEmployee() ||
                _authenticatedUserContext.GetRoles().IsHeadOfDepartment()) &&
                request.OnlyManagedProjects.HasValue &&
                request.OnlyManagedProjects.Value;

        var isHeadOfDepartmentRequestUserFromOtherDepartment = _authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            userInfo.DepartmentId != _authenticatedUserContext.DepartmentId;

        var isGroupManagerRequestUserFromOtherDepartment = _authenticatedUserContext.GetRoles().IsGroupManager() &&
            userInfo.ResourceGroupId != _authenticatedUserContext.ResourceGroupId;

        if (isHeadOfDepartmentRequestUserFromOtherDepartment || isGroupManagerRequestUserFromOtherDepartment || isRequestedManagedProjects)
        {
            await Validate(userInfo.UserId, currentUserId, isHeadOfDepartmentRequestUserFromOtherDepartment,
                isGroupManagerRequestUserFromOtherDepartment, cancellationToken);
            return await GetProjectsAsync(request, currentUserId, cancellationToken);
        }

        var projectsWithDisabledDays = await GetProjectsAsync(request, currentUserId, cancellationToken);

        var projectsDto = _authenticatedUserContext.IsManager
            ? projectsWithDisabledDays
                .Select(project => ProjectWithDisabledDaysDto.Of(project, isCurrentUserAbleToSeeBillableTrackedTime: true))
            : projectsWithDisabledDays
                .Select(project => ProjectWithDisabledDaysDto.Of(
                    project,
                    isCurrentUserAbleToSeeBillableTrackedTime: _authenticatedUserContext
                        .HasClaimWithValue(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject, project.Project!.Id.ToString(CultureInfo.InvariantCulture))))
                .ToList();

        if (_authenticatedUserContext.IsManager)
        {
            foreach (var project in projectsWithDisabledDays)
            {
                project.Project!.IsCurrentUserAbleToSeeBillableTrackedTime =
                     _authenticatedUserContext.HasClaimWithValue(PortalClaimTypeNames.TrackBillableWorkingHoursOnProject, project.Project.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        return projectsWithDisabledDays
            .Join(
                projectsDto,
                responseProject => responseProject.Project!.Id,
                projectDto => projectDto.Project!.Id,
                (responseProject, projectDto) => new ProjectWithDisabledDaysDto
                {
                    Project = projectDto.Project,
                    DisabledDates = responseProject.DisabledDates,
                    BillabilityDays = responseProject.BillabilityDays.Select(x => new BillableAssignementDate
                    {
                        Date = x.Date,
                        IsBillable = x.IsBillable,
                    }).ToArray(),
                })
            .ToList();
    }

    private async Task Validate(int userId, int currentUserId, bool isHeadOfDepartmentRequestUserFromOtherDepartment,
        bool isGroupManagerRequestUserFromOtherDepartment, CancellationToken token)
    {
        if (!await IsProjectsManagedByCurrentUserExistAsync(userId, currentUserId, token) &&
            !await IsUserManagePublicProjectsAsync(currentUserId, token))
        {
            if (isHeadOfDepartmentRequestUserFromOtherDepartment)
            {
                throw new ActionNotAllowedException(PortalMessages.HeadOfDepartmentDoesNotHaveAccessToProjectsOfUserFromAnotherDepartment);
            }

            if (isGroupManagerRequestUserFromOtherDepartment)
            {
                throw new ActionNotAllowedException(PortalMessages.GroupManagerDoesNotHaveAccessToProjectsOfUserFromAnotherResourceGroup);
            }
        }
    }

    private async Task<List<ProjectWithDisabledDaysDto>> GetProjectsAsync(
        Query request,
        int currentUserId,
        CancellationToken cancellationToken)
    {
        var query = _timeTrackDbContext.Projects
            .Include(x => x.ProjectUsers)
            .AsNoTracking();

        if (request.OnlyManagedProjects.HasValue && request.OnlyManagedProjects.Value)
        {
            query = query.Where(x => x.ManagerUserId == currentUserId);
        }

        var checkAssignDates = await _environmentFeatureContext.IsCheckAssignDatesFeatureEnabledAsync();

        if (checkAssignDates)
        {
            var projects = await query
                   .Where(x => x.ProjectUsers.Any(y => y.UserId == request.UserId &&
                       (y.UserOnProjectTo == null || y.UserOnProjectTo >= request.TrackIntervalFrom) && y.UserOnProjectFrom <= request.TrackIntervalTo)
                           && !x.IsPublic && x.IsActive)
                   .Select(x => new Project
                   {
                       Id = x.Id,
                       Name = x.Name,
                       Client = x.Client,
                       IsBillable = x.IsBillable,
                       Color = x.Color,
                       ProjectUsers = x.ProjectUsers
                       .Where(y => y.UserId == request.UserId &&
                           (y.UserOnProjectTo == null || y.UserOnProjectTo >= request.TrackIntervalFrom) && y.UserOnProjectFrom <= request.TrackIntervalTo)
                       .ToList()
                   })
                   .OrderBy(x => x.Id)
                   .ToListAsync(cancellationToken);


            return GetProjectsWithDisabledDays(projects, request.TrackIntervalFrom, request.TrackIntervalTo);
        }

        return await query
                    .Where(x => x.ProjectUsers.Any(y => y.UserId == request.UserId) && !x.IsPublic && x.IsActive)
                    .Select(x => new ProjectWithDisabledDaysDto
                    {
                        Project = new ProjectLookupDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            ClientName = x.Client!.Name,
                            IsBillable = x.IsBillable,
                            Color = x.Color,
                            IsCurrentUserAbleToSeeBillableTrackedTime = true,
                        },
                        DisabledDates = Array.Empty<DateOnly>(),
                        BillabilityDays = Array.Empty<BillableAssignementDate>()
                    })
                    .OrderBy(x => x.Project!.Id)
                    .ToListAsync(cancellationToken);

    }

    private static List<ProjectWithDisabledDaysDto> GetProjectsWithDisabledDays(
        List<Project> projects,
        DateOnly trackIntervalFrom,
        DateOnly trackIntervalTo)
    {
        var result = new List<ProjectWithDisabledDaysDto>();

        var allDays = Enumerable.Range(0, trackIntervalTo.DayNumber - trackIntervalFrom.DayNumber + 1)
            .Select(trackIntervalFrom.AddDays)
            .ToList();

        foreach (var project in projects)
        {
            var excludedDays = new HashSet<DateOnly>();
            var billableDays = new List<BillableAssignementDate>();

            foreach (var projectUser in project.ProjectUsers)
            {
                var projectStart = projectUser.UserOnProjectFrom > trackIntervalFrom ? projectUser.UserOnProjectFrom : trackIntervalFrom;
                var projectEnd = projectUser.UserOnProjectTo < trackIntervalTo ? projectUser.UserOnProjectTo : trackIntervalTo;

                var assignedDays = Enumerable.Range(0, projectEnd.Value.DayNumber - projectStart.DayNumber + 1)
                    .Select(d => projectStart.AddDays(d));

                excludedDays.UnionWith(Enumerable.Range(0, projectEnd.Value.DayNumber - projectStart.DayNumber + 1)
                    .Select(d => projectStart.AddDays(d)));

                billableDays.AddRange(assignedDays.Select(day => new BillableAssignementDate
                {
                    Date = day,
                    IsBillable = projectUser.IsTimeOnProjectBillable
                }));
            }

            var missingDays = allDays.Where(day => !excludedDays.Contains(day)).ToArray();

            result.Add(new ProjectWithDisabledDaysDto
            {
                Project = new ProjectLookupDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    ClientName = project.Client!.Name,
                    IsBillable = project.IsBillable,
                    Color = project.Color,
                    IsCurrentUserAbleToSeeBillableTrackedTime = true,
                },
                DisabledDates = missingDays,
                BillabilityDays = billableDays.ToArray()
            });
        }

        return result;
    }

    private Task<bool> IsProjectsManagedByCurrentUserExistAsync(int userId, int currentUserId, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Projects
            .Where(project => project.ManagerUserId == currentUserId)
            .Where(project => project.ProjectUsers.Any(user => user.UserId == userId) ||
                project.Timesheets.Any(timesheet => timesheet.UserId == userId))
            .Select(project => project.Id)
            .AnyAsync(cancellationToken);
    }

    private Task<bool> IsUserManagePublicProjectsAsync(int currentUserId, CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Projects
            .Where(project => project.IsPublic)
            .Where(project => project.ManagerUserId == currentUserId)
            .AnyAsync(cancellationToken);
    }

    private void ValidateRequest(Query request)
    {
        var isEmployeeRequestingNotManagedProjects = _authenticatedUserContext.GetRoles().IsEmployee() &&
            (!request.OnlyManagedProjects.HasValue || request.OnlyManagedProjects == false);

        if ((isEmployeeRequestingNotManagedProjects || _authenticatedUserContext.GetRoles().IsSystemAdministrator()) &&
            _authenticatedUserContext.UserId != request.UserId)
        {
            throw new ActionNotAllowedException(PortalMessages.CannotAccessOtherUserData);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
            request.OnlyManagedProjects.HasValue && request.OnlyManagedProjects.Value)
        {
            throw new ActionNotAllowedException(PortalMessages.CurrentUserCantBeProjectManager);
        }
    }

    internal sealed record Query(int UserId, bool? OnlyManagedProjects, DateOnly TrackIntervalFrom, DateOnly TrackIntervalTo)
        : IQuery<List<ProjectWithDisabledDaysDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.TrackIntervalFrom).NotNull();
            RuleFor(x => x.TrackIntervalTo).NotNull();
            RuleFor(x => x.TrackIntervalFrom).LessThanOrEqualTo(x => x.TrackIntervalTo);
        }
    }
}
