using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByExcel;

internal sealed class GetTrackedTimeByProjectInExcelRequest : GetTrackedTimeInFileRequestBase,
    IQueryHandler<GetTrackedTimeByProjectInExcelRequest.Query, byte[]>
{
    private readonly IProjectToExcelExporter _projectToExcelExporter;
    private readonly IGetProjectNameByProjectIdDbQuery _getProjectNameByIdDbQuery;
    private readonly IDateService _dateService;
    private readonly IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery _getTimesheetsWithClientInfoByPeriodWithFilterDbQuery;
    private readonly ITimeTrackPortalSettingsAccessor _timeTrackPortalSettingsAccessor;

    public GetTrackedTimeByProjectInExcelRequest(
        IProjectToExcelExporter projectToExcelExporter,
        IGetProjectNameByProjectIdDbQuery getProjectNameByIdDbQuery,
        IDateService dateService,
        IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery getTimesheetsWithClientInfoByPeriodWithFilterDbQuery,
        ITimeTrackPortalSettingsAccessor timeTrackPortalSettingsAccessor,
        ICanUserReportBeExportedBusinessRule canUserReportBeExportedBusinessRule,
        ITeamBoardContract teamBoardContract,
        IIdentityContract identityContract)
        : base(canUserReportBeExportedBusinessRule, teamBoardContract, identityContract)
    {
        _projectToExcelExporter = projectToExcelExporter;
        _getProjectNameByIdDbQuery = getProjectNameByIdDbQuery;
        _dateService = dateService;
        _getTimesheetsWithClientInfoByPeriodWithFilterDbQuery = getTimesheetsWithClientInfoByPeriodWithFilterDbQuery;
        _timeTrackPortalSettingsAccessor = timeTrackPortalSettingsAccessor;
    }

    public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var projectName = await _getProjectNameByIdDbQuery.GetProjectNameByProjectIdAsync(
                                request.ProjectId,
                                cancellationToken);

        if (projectName is null)
        {
            throw new EntityNotFoundException($"Project with id {request.ProjectId} does not exist");
        }

        await CheckReportCanBeExported(request.UserId, cancellationToken);

        var filter = new TimesheetFilter
        {
            DepartmentId = request.DepartmentId,
            ClientId = request.ClientId,
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            TrackedTimeType = request.TrackedTimeType,
            ResourceGroupId = request.ResourceGroupId
        };

        var dbRequest = new GetTimesheetsWithClientInfoByPeriodWithFilterDbRequest(
            request.Year,
            request.MonthOfYear,
            request.WeekOfYear,
            filter);

        var timesheets = await _getTimesheetsWithClientInfoByPeriodWithFilterDbQuery
            .ExecuteAsync(dbRequest, cancellationToken);

        var projectToExport = timesheets.Count == 0
            ? new ProjectToExportModel { ProjectName = projectName }
            : GetProjectToExport(timesheets, request.TrackedTimeType);

        var dateSpan = _dateService.GetDateSpan(request);

        var settings = await _timeTrackPortalSettingsAccessor.GetSettings(cancellationToken);

        ArgumentNullException.ThrowIfNull(settings.ExcelSettings);
        ArgumentNullException.ThrowIfNull(settings.ReportsAuthorName);

        var exportModel = new ProjectWithDatesToExportModel
        {
            ClientName = timesheets.Select(timesheet => timesheet.Project!.Client?.Name).FirstOrDefault() ?? string.Empty,
            Project = projectToExport,
            StartTime = dateSpan.StartDate,
            EndTime = dateSpan.EndDate,
            ExcelConfig = settings.ExcelSettings,
            ReportsAuthorName = settings.ReportsAuthorName
        };

        return await _projectToExcelExporter.Export(exportModel, cancellationToken);
    }

    internal sealed record Query(
            int? Year,
            int[]? MonthOfYear,
            int? WeekOfYear,
            string? DepartmentId,
            int? ClientId,
            int? UserId,
            int? ResourceGroupId,
            int ProjectId,
            TrackedTimeType TrackedTimeType)
        : IQuery<byte[]>, IPeriodRequest, ITimeTrackRequest;
}
