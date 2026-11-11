using System.Globalization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Requests;

internal sealed class GetProjectsLookupByUserIdRequest : IQueryHandler<GetProjectsLookupByUserIdRequest.Query, List<ProjectLookupDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimeTrackDbContext _timeTrackDbContext;

    public GetProjectsLookupByUserIdRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ITimeTrackDbContext timeTrackDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _timeTrackDbContext = timeTrackDbContext;
    }

    public async Task<List<ProjectLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var userInfo = await _timeTrackDbContext.ProjectUsers
            .Include(projectUser => projectUser.User)
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
        var currentUserRoles = _authenticatedUserContext.GetRoles();

        var isRequestedManagedProjects = (currentUserRoles.IsEmployee() ||
                                          currentUserRoles.IsHeadOfDepartment()) &&
                                          request.OnlyManagedProjects.HasValue &&
                                          request.OnlyManagedProjects.Value;

        var isHeadOfDepartmentRequestUserFromOtherDepartment =
            currentUserRoles.IsHeadOfDepartment() &&
            userInfo.DepartmentId != _authenticatedUserContext.DepartmentId;

        var isGroupManagerRequestUserFromOtherDepartment =
            currentUserRoles.IsGroupManager() &&
            userInfo.ResourceGroupId != _authenticatedUserContext.ResourceGroupId;

        if (isHeadOfDepartmentRequestUserFromOtherDepartment ||
            isGroupManagerRequestUserFromOtherDepartment ||
            isRequestedManagedProjects)
        {
            await ValidateManagedProjectsAccessAsync(
                userInfo.UserId, currentUserId,
                isHeadOfDepartmentRequestUserFromOtherDepartment,
                isGroupManagerRequestUserFromOtherDepartment,
                cancellationToken);

            return await GetProjectsByUserIdAsync(
                request,
                userInfo.UserId,
                isForManagedProjects: true,
                currentUserId,
                isUserCanSeeBillableHours: true,
                cancellationToken);
        }

        var projects = await GetProjectsByUserIdAsync(
                request,
                userInfo.UserId,
                isForManagedProjects: false,
                currentUserId,
                isUserCanSeeBillableHours: true,
                cancellationToken);

        if (_authenticatedUserContext.IsManager)
        {
            return projects;
        }

        foreach (var project in projects)
        {
            project.IsCurrentUserAbleToSeeBillableTrackedTime = _authenticatedUserContext.HasClaimWithValue(
                PortalClaimTypeNames.TrackBillableWorkingHoursOnProject,
                project.Id.ToString(CultureInfo.InvariantCulture)
            );
        }

        return projects;
    }

    private async Task ValidateManagedProjectsAccessAsync(
        int userId,
        int currentUserId,
        bool isHeadOfDepartmentRequestUserFromOtherDepartment,
        bool isGroupManagerRequestUserFromOtherDepartment,
        CancellationToken cancellationToken)
    {
        var isProjectsManagedByCurrentUserExist = await IsProjectsManagedByCurrentUserExistAsync(
            userId, currentUserId, cancellationToken);
        var isAnyPublicProjectManaged = await IsUserManagePublicProjectsAsync(currentUserId, cancellationToken);

        if (isProjectsManagedByCurrentUserExist || isAnyPublicProjectManaged)
        {
            return;
        }

        if (isHeadOfDepartmentRequestUserFromOtherDepartment)
        {
            throw new ActionNotAllowedException(PortalMessages.HeadOfDepartmentDoesNotHaveAccessToProjectsOfUserFromAnotherDepartment);
        }

        if (isGroupManagerRequestUserFromOtherDepartment)
        {
            throw new ActionNotAllowedException(PortalMessages.GroupManagerDoesNotHaveAccessToProjectsOfUserFromAnotherResourceGroup);
        }
    }

    private Task<List<ProjectLookupDto>> GetProjectsByUserIdAsync(
        Query request,
        int userId,
        bool isForManagedProjects,
        int currentUserId,
        bool isUserCanSeeBillableHours,
        CancellationToken cancellationToken)
    {
        var query = _timeTrackDbContext.Projects.AsNoTracking();

        if (isForManagedProjects)
        {
            query = query.Where(project => project.ManagerUserId == currentUserId);
        }

        if (request.IsPublic.HasValue && request.IsPublic.Value)
        {
            query = query
                .Where(project => project.IsPublic)
                .Where(project => project.Timesheets.Any(y => y.UserId == userId));
        }
        else
        {
            query = query
                .Where(project => !project.IsPublic)
                .Where(project => project.ProjectUsers.Any(y => y.UserId == userId));

            if (request.TrackIntervalFrom.HasValue && request.TrackIntervalTo.HasValue)
            {
                query = query.Where(x =>
                    x.ProjectUsers.Any(y =>
                        y.UserId == userId &&
                        !y.UserOnProjectTo.HasValue
                            ? request.TrackIntervalTo >= y.UserOnProjectFrom
                            : !(
                                request.TrackIntervalTo < y.UserOnProjectFrom ||
                                request.TrackIntervalFrom > y.UserOnProjectTo
                            )
                    )
                );
            }
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(e => e.IsActive == request.IsActive);
        }

        return query
            .OrderBy(x => x.Id)
            .Select(project => new ProjectLookupDto
            {
                Id = project.Id,
                Name = project.Name,
                ClientName = project.Client!.Name,
                IsBillable = project.IsBillable,
                Color = project.Color,
                IsCurrentUserAbleToSeeBillableTrackedTime = isUserCanSeeBillableHours
            })
            .ToListAsync(cancellationToken);
    }

    private Task<bool> IsProjectsManagedByCurrentUserExistAsync(
        int userId,
        int currentUserId,
        CancellationToken cancellationToken)
    {
        return _timeTrackDbContext.Projects
            .Where(project => project.ManagerUserId == currentUserId)
            .Where(project =>
                project.ProjectUsers.Any(user => user.UserId == userId) ||
                project.Timesheets.Any(timesheet => timesheet.UserId == userId)
            )
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
        var userRoles = _authenticatedUserContext.GetRoles();

        var isEmployeeRequestingNotManagedProjects =
            userRoles.IsEmployee() &&
            request.OnlyManagedProjects is null or false;

        if ((isEmployeeRequestingNotManagedProjects || userRoles.IsSystemAdministrator()) &&
            _authenticatedUserContext.UserId != request.UserId)
        {
            throw new ActionNotAllowedException(PortalMessages.CannotAccessOtherUserData);
        }

        if (userRoles.IsGroupManager() && request.OnlyManagedProjects.HasValue && request.OnlyManagedProjects.Value)
        {
            throw new ActionNotAllowedException(PortalMessages.CurrentUserCantBeProjectManager);
        }
    }

    internal sealed record Query(
        int UserId,
        bool? IsPublic,
        bool? OnlyManagedProjects,
        bool? IsActive,
        DateOnly? TrackIntervalFrom,
        DateOnly? TrackIntervalTo
    ) : IQuery<List<ProjectLookupDto>>, ITimeTrackRequest;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            When(x => x.TrackIntervalTo is not null, () => RuleFor(x => x.TrackIntervalFrom).NotNull());
            When(x => x.TrackIntervalFrom is not null, () => RuleFor(x => x.TrackIntervalTo).NotNull());

            When(x => x.TrackIntervalFrom is not null && x.TrackIntervalTo is not null, () =>
                RuleFor(x => x.TrackIntervalFrom).LessThanOrEqualTo(x => x.TrackIntervalTo));
        }
    }
}
