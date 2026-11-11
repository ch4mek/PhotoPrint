using MediatR;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.Contract.Input;
using Modsen.Portal.TimeTrack.Contract.Contract.Output;
using Modsen.Portal.TimeTrack.Contract.ContractDto;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Modsen.Portal.TimeTrack.UseCases.Queries;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UseCases;


internal class TimeTrackContract : ITimeTrackContract
{
    private readonly ISender _sender;
    private readonly IProjectsProvider _projectsProvider;
    private readonly IGetClientNamesFromProjectsUserIsAssignedToDbQuery _getClientNamesFromProjectsUserIsAssignedToDbQuery;
    private readonly IGetProjectManagerIdsByUserIdDbQuery _getProjectManagerIdsByUserIdDbQuery;

    public TimeTrackContract(
        ISender sender,
        IProjectsProvider projectsProvider,
        IGetClientNamesFromProjectsUserIsAssignedToDbQuery getClientNamesFromProjectsUserIsAssignedToDbQuery,
        IGetProjectManagerIdsByUserIdDbQuery getProjectManagerIdsByUserIdDbQuery)
    {
        _sender = sender;
        _projectsProvider = projectsProvider;
        _getClientNamesFromProjectsUserIsAssignedToDbQuery = getClientNamesFromProjectsUserIsAssignedToDbQuery;
        _getProjectManagerIdsByUserIdDbQuery = getProjectManagerIdsByUserIdDbQuery;
    }

    public async Task<List<ProjectOutput>> GetProjectsByUserId(int userId, CancellationToken cancellationToken)
    {
        var projects = await _sender.Send(new GetProjectsByUserIdRequest.Query(userId), cancellationToken);

        return projects.Select(project => new ProjectOutput
        {
            ClientId = project.ClientId,
            Color = project.Color,
            IsActive = project.IsActive,
            IsBillable = project.IsBillable,
            IsPublic = project.IsPublic,
            Id = project.Id,
            Name = project.Name,
            ProjectUsers = project.ProjectUsers.ToList().ConvertAll(x => new ProjectUserOutput
            {
                UserId = x.Id
            })
        }).ToList();

    }

    public async Task<List<TimesheetByFiltersOutput>> GetTimesheetByFilters(TimesheetByFiltersQuery request, CancellationToken cancellationToken)
    {
        var timesheets = await _sender.Send(new GetTimesheetsByFiltersRequest.Query(request.StartYear, request.EndYear, request.DepartmentId,
            request.Projects, request.Clients, request.IsBillable), cancellationToken);

        return timesheets.Select(t => new TimesheetByFiltersOutput
        {
            Duration = t.Duration,
            Month = t.Month,
            Year = t.Year,
            Users = t.Users.Select(user => new TimesheetUserOutput
            {
                Id = user.Id,
                Count = user.Count,
                Duration = user.Duration,
                FirstName = user.FirstName,
                LastName = user.LastName
            }).ToList()
        }).ToList();
    }

    public async Task<List<DepartmentTimesheetsByFiltersOutput>> GetDepartmentTimesheetByFilters(TimesheetByFiltersQuery request, CancellationToken cancellationToken)
    {
        var timesheets = await _sender.Send(new GetDepartmentTimesheetsByFiltersRequest.Query(request.StartYear, request.EndYear,
            request.DepartmentId, request.Projects, request.Clients, request.IsBillable), cancellationToken);

        return timesheets.Select(t => new DepartmentTimesheetsByFiltersOutput
        {
            Month = t.Month,
            Year = t.Year,
            Duration = t.Duration,
            Departments = t.Departments.Select(department => new TimesheetDepartmentOutput
            {
                Duration = department.Duration,
                Count = department.Count,
                DepartmentId = department.DepartmentId
            })
            .ToArray()
        }).ToList();
    }

    public async Task<List<ProjectOutput>> GetListOfProjectsByIds(IReadOnlyCollection<int> projectIds, CancellationToken cancellationToken)
    {
        var projects = await _sender.Send(new GetProjectsListByIdsRequest.Query(projectIds), cancellationToken);

        return projects.Select(project => new ProjectOutput
        {
            ClientId = project.ClientId,
            Color = project.Color,
            IsActive = project.IsActive,
            IsBillable = project.IsBillable,
            IsPublic = project.IsPublic,
            Id = project.Id,
            Name = project.Name,
            ProjectUsers = project.ProjectUsers.ToList().ConvertAll(x => new ProjectUserOutput { UserId = x.Id })
        }).ToList();
    }

    public Task<bool> IsUserAssignedToProjectAsPmAsync(int userId, CancellationToken cancellationToken)
    {
        return _projectsProvider.IsUserAssignedToProjectAsPmAsync(userId, cancellationToken);
    }

    public Task<PayloadContractDto> GetTrackedTimePayloadForUsers(
        IReadOnlyCollection<int> userIds,
        int year,
        string? projectName,
        IReadOnlyCollection<string>? departmentIds,
        CancellationToken token)
    {
        return _sender.Send(new GetTrackedTimePayloadForUsersUseCase.Query(userIds, year, projectName, departmentIds), token);
    }

    public async Task<List<UserEmailOutput>> GetUsersWithInactiveTimeTracking(int daysRangeBeforeCurrentDate,
        CancellationToken cancellationToken)
    {
        var response = await _sender.Send(new GetUsersWithInactiveTimeTrackingUseCase.Query(daysRangeBeforeCurrentDate),
            cancellationToken);

        return response.Users.ConvertAll(user => new UserEmailOutput(user.Email, user.Name));
    }

    public async Task<List<string>> GetClientNamesFromProjectsUserIsAssignedTo(int userId, CancellationToken token)
    {
        return await _getClientNamesFromProjectsUserIsAssignedToDbQuery.ExecuteAsync(userId, token);
    }

    public async Task<List<ProjectsTrakedTimeOutput>> GetSumTrackedTimeByProjectsIds(IReadOnlyCollection<int> projectIds, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetTimesheetsByProjectIdsRequest.Query(projectIds), cancellationToken);
        return result.ConvertAll(x => new ProjectsTrakedTimeOutput(x.ProjectId, x.FactTrackedSum, x.BillableTrackedSum));
    }

    public Task<bool> IsAssignHasTrackedTime(int userId, int projectId, DateOnly userProjectFrom, DateOnly? userProjectTo, CancellationToken cancellationToken)
    {
        return _sender.Send(new IsAssignHasTrackedTimeRequest.Query(userId, projectId, userProjectFrom, userProjectTo), cancellationToken);
    }

    public Task<List<int>> GetProjectManagerIdsByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return _getProjectManagerIdsByUserIdDbQuery.ExecuteAsync(userId, cancellationToken);
    }

    public Task<List<int>> GetUsersByProjectId(int projectId, CancellationToken cancellationToken)
    {
        return _sender.Send(new GetProjectUsersByProjectIdRequest.Query(projectId), cancellationToken);
    }

    public Task<List<int>> GetUsersBySalesManagerId(int salesManagerId, CancellationToken cancellationToken)
    {
        return _sender.Send(new GetProjectUsersBySalesManagerRequest.Query(salesManagerId), cancellationToken);
    }

    public Task<List<int>> GetUserIdsByProjectManagerAsync(int managerId, CancellationToken cancellationToken)
    {
        return _sender.Send(new GetUserIdsByProjectManagerRequest.Query(managerId), cancellationToken);
    }

    public async Task<IReadOnlyCollection<TimesheetByUserOutput>> GetTimesheetByUsers(IReadOnlyCollection<TimesheetByUserInput> request, CancellationToken cancellationToken)
    {
        if (request.Count == 0)
        {
            return [];
        }

        var usersTimesheetsDto = request
            .Select(x => new GetUserTimesheetByMonthDto
            {
                UserId = x.UserId,
                ProjectId = x.ProjectId,
                Year = x.Year,
                Month = x.Month
            })
            .ToList();

        var response = await _sender.Send(new GetUsersTimesheetsByMonthRequest.Query(usersTimesheetsDto), cancellationToken);

        return response
            .Select(x => new TimesheetByUserOutput
            {
                UserId = x.UserId,
                ProjectId = x.ProjectId,
                BillableTrackedSum = x.BillableTrackedSum
            })
            .ToList();
    }
}
