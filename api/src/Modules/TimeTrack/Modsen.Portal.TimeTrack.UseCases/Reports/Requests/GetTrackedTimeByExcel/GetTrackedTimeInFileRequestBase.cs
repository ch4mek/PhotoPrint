using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByExcel;

internal abstract class GetTrackedTimeInFileRequestBase
{
    private readonly ICanUserReportBeExportedBusinessRule _canUserReportBeExportedBusinessRule;
    private readonly ITeamBoardContract _teamBoardContract;
    private readonly IIdentityContract _identityContract;

    protected GetTrackedTimeInFileRequestBase(
        ICanUserReportBeExportedBusinessRule canUserReportBeExportedBusinessRule,
        ITeamBoardContract teamBoardContract,
        IIdentityContract identityContract)
    {
        _canUserReportBeExportedBusinessRule = canUserReportBeExportedBusinessRule;
        _teamBoardContract = teamBoardContract;
        _identityContract = identityContract;
    }

    protected async Task CheckReportCanBeExported(int? userId, CancellationToken cancellationToken)
    {
        if (userId is null)
        {
            return;
        }

        var canUserReportBeExported = await _canUserReportBeExportedBusinessRule.IsCorrespondsToBusiness(userId.Value, cancellationToken);

        if (!canUserReportBeExported.IsSuccess)
        {
            throw new ActionNotAllowedException(canUserReportBeExported.Reason ?? string.Empty);
        }
    }

    protected async Task FillUsersInfo(List<UserToExportModel> users, CancellationToken cancellationToken)
    {
        var userIds = users.Select(user => user.UserId).Distinct().ToArray();

        var userRuNames = await _teamBoardContract.GetUsersRuNames(userIds, cancellationToken);
        var departmentNames = await _identityContract.GetDepartmentNames(cancellationToken);

        foreach (var userToExportModel in users)
        {
            var ruName = userRuNames.FirstOrDefault(output => output.Id == userToExportModel.UserId);

            userToExportModel.LastNameRu = ruName?.LastName;
            userToExportModel.FirstNameRu = ruName?.FirstName;
            if (!userToExportModel.Department.IsNullOrEmpty())
            {
                userToExportModel.Department =
                    departmentNames
                        .FirstOrDefault(x => x.DepartmentId == userToExportModel.Department)?
                        .FullName ?? userToExportModel.Department;
            }
        }
    }

    protected static ProjectToExportModel GetProjectToExport(List<Timesheet> timesheets, TrackedTimeType trackedTimeType)
    {
        var projectToExport = new ProjectToExportModel
        {
            ProjectName = timesheets[0].Project?.Name ?? string.Empty
        };

        var usersToExport = new List<UserToExportModel>();
        var userGroups = timesheets
            .GroupBy(timesheet => timesheet.UserId)
            .ToList();

        foreach (var userGroup in userGroups)
        {
            var userTimesheets = userGroup
                .ToList();

            var tasksPerDates = userTimesheets
                .GroupBy(timesheet => timesheet.ReportForDate)
                .Select(dateGroup => new TasksPerDateToExportModel
                {
                    Date = dateGroup.Key,
                    TotalTimeInMinutes = dateGroup.Sum(GetTrackedTimeSelector(trackedTimeType)),
                    Tasks = dateGroup
                        .OrderBy(timesheet => timesheet.Id)
                        .Select(timesheet => new TaskToExportModel
                        {
                            TrackedInMinutes = GetTrackedTimeByType(timesheet, trackedTimeType),
                            TaskName = timesheet.TaskName
                        }).ToList()
                }).OrderBy(tasksPerDate => tasksPerDate.Date).ToList();

            var userToExport = new UserToExportModel
            {
                UserId = userGroup.Key,
                FirstName = userTimesheets[0].User?.FirstName ?? string.Empty,
                LastName = userTimesheets[0].User?.LastName ?? string.Empty,
                Department = userTimesheets[0].User?.DepartmentId ?? string.Empty,
                TotalTimeInMinutes = tasksPerDates.Sum(tasksPerDate => tasksPerDate.TotalTimeInMinutes),
                Dates = tasksPerDates
            };

            usersToExport.Add(userToExport);
        }

        projectToExport.Users = usersToExport.OrderBy(users => users.LastName).ToList();
        projectToExport.TotalTimeInMinutes = usersToExport.Sum(user => user.TotalTimeInMinutes);
        return projectToExport;
    }

    protected static List<ClientToExportModel> GetClientsToExport(List<Timesheet> timesheets, TrackedTimeType trackedTimeType)
    {
        var clientsToExport = new List<ClientToExportModel>();

        if (timesheets.Count == 0)
        {
            return clientsToExport;
        }

        var clientsGroup = timesheets
            .GroupBy(timesheet => timesheet.Project?.Client?.Id);

        foreach (var clientGroup in clientsGroup)
        {
            var clientTimesheets = clientGroup.ToList();
            var projectsToExport = clientTimesheets
                .GroupBy(timesheet => timesheet.Project?.Name)
                .Select(projectGroup => GetProjectToExport(
                    timesheets: projectGroup.ToList(),
                    trackedTimeType))
                .ToList();

            clientsToExport.Add(new ClientToExportModel
            {
                Name = clientTimesheets[0].Project?.Client?.Name ?? string.Empty,
                TotalTimeInMinutes = projectsToExport.Sum(projectToExport => projectToExport.TotalTimeInMinutes),
                Projects = projectsToExport
            });
        }

        return clientsToExport.OrderBy(client => client.Name).ToList();
    }

    private static Func<Timesheet, int> GetTrackedTimeSelector(TrackedTimeType trackedTimeType)
    {
        return trackedTimeType switch
        {
            TrackedTimeType.Fact => x => x.FactTrackedInMinutes,
            TrackedTimeType.Billable => x => x.BillableTrackedInMinutes,
            _ => throw new InvalidOperationException()
        };
    }

    private static int GetTrackedTimeByType(Timesheet timesheet, TrackedTimeType trackedTimeType)
    {
        return trackedTimeType switch
        {
            TrackedTimeType.Fact => timesheet.FactTrackedInMinutes,
            TrackedTimeType.Billable => timesheet.BillableTrackedInMinutes,
            _ => throw new InvalidOperationException()
        };
    }
}
