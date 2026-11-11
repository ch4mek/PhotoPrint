using Modsen.Portal.Core;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.StimulsoftReport.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft;

internal sealed class GetTotalTrackedTimeToExportRequest : GetTrackedTimeToExportRequestBase, IQueryHandler<GetTotalTrackedTimeToExportRequest.Query, byte[]>
{
    private readonly IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery _getTimesheetsDbQuery;
    private readonly ITimeTrackPortalSettingsAccessor _timeTrackPortalSettingsAccessor;

    public GetTotalTrackedTimeToExportRequest(
        IReportExporter exporter,
        IDateService dateService,
        IPeriodService periodService,
        IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery getTimesheetsDbQuery,
        ITimeTrackPortalSettingsAccessor timeTrackPortalSettingsAccessor
        )
        : base(dateService, exporter, periodService)
    {
        _getTimesheetsDbQuery = getTimesheetsDbQuery;
        _timeTrackPortalSettingsAccessor = timeTrackPortalSettingsAccessor;
    }

    public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var reportTitle = CreateReportTitle(subjectName: null, request);

        var filter = new TimesheetFilter
        {
            DepartmentId = request.DepartmentId,
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
            TemplateType = TemplateType.Total,
            ReportsAuthorName = settings.ReportsAuthorName,
            TitleObject = reportTitle,
            RootDataObject = projectsToExport
        };

        return await GetReportAsync(reportRequest, request.Format, cancellationToken);
    }

    internal sealed record Query(
            int? Year,
            int[]? MonthOfYear,
            int? WeekOfYear,
            string? DepartmentId,
            int? UserId,
            int? ResourceGroupId,
            DocumentType Format,
            string ReportName,
            TrackedTimeType TrackedTimeType)
        : IQuery<byte[]>, IPeriodRequest, ITimeTrackRequest;
}
