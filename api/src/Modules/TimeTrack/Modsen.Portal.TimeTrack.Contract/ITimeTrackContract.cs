using Modsen.Portal.TimeTrack.Contract.Contract.Input;
using Modsen.Portal.TimeTrack.Contract.Contract.Output;
using Modsen.Portal.TimeTrack.Contract.ContractDto;

namespace Modsen.Portal.TimeTrack.Contract;

public interface ITimeTrackContract
{
    // Team Work
    Task<List<TimesheetByFiltersOutput>> GetTimesheetByFilters(TimesheetByFiltersQuery request, CancellationToken cancellationToken);
    Task<List<DepartmentTimesheetsByFiltersOutput>> GetDepartmentTimesheetByFilters(TimesheetByFiltersQuery request, CancellationToken cancellationToken);
    Task<List<UserEmailOutput>> GetUsersWithInactiveTimeTracking(int daysRangeBeforeCurrentDate, CancellationToken cancellationToken);
    Task<List<ProjectsTrakedTimeOutput>> GetSumTrackedTimeByProjectsIds(IReadOnlyCollection<int> projectIds, CancellationToken cancellationToken);

    // Identity
    Task<List<ProjectOutput>> GetProjectsByUserId(int userId, CancellationToken cancellationToken);
    Task<List<ProjectOutput>> GetListOfProjectsByIds(IReadOnlyCollection<int> projectIds, CancellationToken cancellationToken);

    // Identity
    Task<bool> IsUserAssignedToProjectAsPmAsync(int userId, CancellationToken cancellationToken);

    // Team Board
    Task<PayloadContractDto> GetTrackedTimePayloadForUsers(IReadOnlyCollection<int> userIds, int year, string? projectName, IReadOnlyCollection<string>? departmentIds, CancellationToken token);
    Task<List<string>> GetClientNamesFromProjectsUserIsAssignedTo(int userId, CancellationToken token);
    Task<bool> IsAssignHasTrackedTime(int userId, int projectId, DateOnly userProjectFrom, DateOnly? userProjectTo, CancellationToken cancellationToken);

    // Finance
    Task<IReadOnlyCollection<TimesheetByUserOutput>> GetTimesheetByUsers(IReadOnlyCollection<TimesheetByUserInput> request, CancellationToken cancellationToken);

    // Inquiry
    Task<List<int>> GetProjectManagerIdsByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<List<int>> GetUsersByProjectId(int projectId, CancellationToken cancellationToken);
    Task<List<int>> GetUsersBySalesManagerId(int salesManagerId, CancellationToken cancellationToken);

    Task<List<int>> GetUserIdsByProjectManagerAsync(int managerId, CancellationToken cancellationToken);
}
