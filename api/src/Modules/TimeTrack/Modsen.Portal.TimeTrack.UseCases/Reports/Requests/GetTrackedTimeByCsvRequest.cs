using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv.Models;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

internal sealed class GetTrackedTimeByCsvRequest : IQueryHandler<GetTrackedTimeByCsvRequest.Query, byte[]>
{
    private readonly IReportToCsvExporter _exporter;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimeTrackUserProvider _timeTrackUserProvider;
    private readonly ITimeTrackPortalSettingsAccessor _timeTrackPortalSettingsAccessor;
    private readonly ITimeTrackDbContext _dbContext;

    public GetTrackedTimeByCsvRequest(
        IReportToCsvExporter exporter,
        IAuthenticatedUserContext authenticatedUserContext,
        ITimeTrackUserProvider timeTrackUserProvider,
        ITimeTrackPortalSettingsAccessor timeTrackPortalSettingsAccessor,
        ITimeTrackDbContext dbContext)
    {
        _exporter = exporter;
        _authenticatedUserContext = authenticatedUserContext;
        _timeTrackUserProvider = timeTrackUserProvider;
        _timeTrackPortalSettingsAccessor = timeTrackPortalSettingsAccessor;
        _dbContext = dbContext;
    }

    public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
    {
        if (request.UserId is not null)
        {
            var user = await _timeTrackUserProvider.GetByIdAsync(request.UserId.Value, cancellationToken);

            if (user is null)
            {
                throw new EntityNotFoundException($"User with id = {request.UserId.Value} does not exist");
            }

            if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment()
                && user.DepartmentId != _authenticatedUserContext.DepartmentId)
            {
                throw new ActionNotAllowedException(PortalMessages.HeadCannotExportCsvOfUserFromOtherDepartment);
            }

            if (_authenticatedUserContext.GetRoles().IsGroupManager()
                && user.ResourceGroupId != _authenticatedUserContext.ResourceGroupId)
            {
                throw new ActionNotAllowedException(PortalMessages.GroupManagerCannotExportCsvOfUserFromOtherGroup);
            }
        }

        var filter = new TimesheetFilter
        {
            DepartmentId = request.DepartmentId,
            ClientId = request.ClientId,
            UserId = request.UserId,
            ProjectId = request.ProjectId,
            ResourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager()
                ? _authenticatedUserContext.ResourceGroupId
                : request.ResourceGroupId
        };

        var timesheets = await GetFilteredTimesheetsAsync(request, filter, cancellationToken);

        var tasksToExport = timesheets.ConvertAll(TaskToCsvExportModel.Of);
        var orderedTasks = tasksToExport.OrderByDescending(task => task.Date).ToList();

        var settings = await _timeTrackPortalSettingsAccessor.GetSettings(cancellationToken);

        ArgumentNullException.ThrowIfNull(settings.ExcelSettings);
        ArgumentNullException.ThrowIfNull(settings.ExcelSettings.DefaultDateFormat);

        var tasksToCsvExportModel = new TasksToCsvExportModel
        {
            Tasks = orderedTasks,
            DateFormat = settings.ExcelSettings.DefaultDateFormat
        };

        return await _exporter.Export(tasksToCsvExportModel, cancellationToken);
    }

    private Task<List<Timesheet>> GetFilteredTimesheetsAsync(Query request,
        TimesheetFilter filter,
        CancellationToken cancellationToken)
    {
        return _dbContext.Timesheets
            .AsNoTracking()
            .FilterByPeriod(request.Year, request.MonthOfYear, request.WeekOfYear)
            .ApplyTimesheetFilter(filter)
            .OrderBy(timesheet => timesheet.Id)
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(
    int? Year,
    int[]? MonthOfYear,
    int? WeekOfYear,
    string? DepartmentId,
    int? ResourceGroupId,
    int? ClientId,
    int? ProjectId,
    int? UserId) : IQuery<byte[]>, IPeriodRequest, ITimeTrackRequest;
}
