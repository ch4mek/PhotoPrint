using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.StimulsoftReport.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft;

internal sealed class GetTrackedTimeByProjectIdToExportRequest : GetTrackedTimeToExportRequestBase, IQueryHandler<GetTrackedTimeByProjectIdToExportRequest.Query, byte[]>
{
    private readonly IGetProjectNameByProjectIdDbQuery _getProjectNameByIdDbQuery;
    private readonly IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery _getTimesheetsDbQuery;
    private readonly ITimeTrackPortalSettingsAccessor _timeTrackPortalSettingsAccessor;

    public GetTrackedTimeByProjectIdToExportRequest(
        IReportExporter exporter,
        IDateService dateService,
        IPeriodService periodService,
        IGetProjectNameByProjectIdDbQuery getProjectNameByIdDbQuery,
        IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery getTimesheetsDbQuery,
        ITimeTrackPortalSettingsAccessor timeTrackPortalSettingsAccessor
        )
        : base(dateService, exporter, periodService)
    {
        _getProjectNameByIdDbQuery = getProjectNameByIdDbQuery;
        _getTimesheetsDbQuery = getTimesheetsDbQuery;
        _timeTrackPortalSettingsAccessor = timeTrackPortalSettingsAccessor;
    }

    public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var projectName = await _getProjectNameByIdDbQuery
            .GetProjectNameByProjectIdAsync(request.ProjectId, cancellationToken)
                ?? throw new EntityNotFoundException(PortalMessages.ProjectNotFound);

        var reportTitle = CreateReportTitle(projectName, request);

        var filter = new TimesheetFilter
        {
            DepartmentId = request.DepartmentId,
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

        var timesheets = await _getTimesheetsDbQuery.ExecuteAsync(dbRequest, cancellationToken);

        var projectsToExport = GetProjectsToExport(timesheets, request.TrackedTimeType);

        var settings = await _timeTrackPortalSettingsAccessor.GetSettings(cancellationToken);

        ArgumentNullException.ThrowIfNull(settings.ReportsAuthorName);

        var reportRequest = new ReportRequest
        {
            ReportName = request.ReportName,
            TemplateType = TemplateType.PerProject,
            ReportsAuthorName = settings.ReportsAuthorName,
            RootDataObject = projectsToExport,
            TitleObject = reportTitle
        };

        return await GetReportAsync(reportRequest, request.Format, cancellationToken);
    }

    internal sealed record Query(
            int? Year,
            int[]? MonthOfYear,
            int? WeekOfYear,
            string? DepartmentId,
            int ProjectId,
            int? UserId,
            int? ResourceGroupId,
            DocumentType Format,
            string ReportName,
            TrackedTimeType TrackedTimeType)
        : IQuery<byte[]>, IPeriodRequest, ITimeTrackRequest;
}
