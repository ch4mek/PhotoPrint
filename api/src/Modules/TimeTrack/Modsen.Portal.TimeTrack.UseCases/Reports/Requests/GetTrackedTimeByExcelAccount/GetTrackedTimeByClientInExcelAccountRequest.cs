using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByExcel;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByExcelAccount;

internal sealed class GetTrackedTimeByClientInExcelAccountRequest
    : GetTrackedTimeInFileRequestBase, IQueryHandler<GetTrackedTimeByClientInExcelAccountRequest.Query, byte[]>
{
    private readonly IClientsToExcelAccountExporter _clientsToExcelExporter;
    private readonly IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery _getTimesheetsWithClientInfoByPeriodWithFilterDbQuery;
    private readonly IDateService _dateService;
    private readonly ITimeTrackPortalSettingsAccessor _timeTrackPortalSettingsAccessor;

    public GetTrackedTimeByClientInExcelAccountRequest(
        IClientsToExcelAccountExporter clientsToExcelExporter,
        IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery getTimesheetsWithClientInfoByPeriodWithFilterDbQuery,
        IDateService dateService,
        ITimeTrackPortalSettingsAccessor timeTrackPortalSettingsAccessor,
        ICanUserReportBeExportedBusinessRule canUserReportBeExportedBusinessRule,
        ITeamBoardContract teamBoardContract,
        IIdentityContract identityContract)
        : base(canUserReportBeExportedBusinessRule, teamBoardContract, identityContract)
    {
        _clientsToExcelExporter = clientsToExcelExporter;
        _getTimesheetsWithClientInfoByPeriodWithFilterDbQuery = getTimesheetsWithClientInfoByPeriodWithFilterDbQuery;
        _dateService = dateService;
        _timeTrackPortalSettingsAccessor = timeTrackPortalSettingsAccessor;
    }

    public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
    {
        await CheckReportCanBeExported(request.UserId, cancellationToken);

        var filter = new TimesheetFilter
        {
            DepartmentId = request.DepartmentId,
            ClientId = request.ClientId,
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

        var clients = GetClientsToExport(timesheets, request.TrackedTimeType);

        foreach (var client in clients)
        {
            foreach (var project in client.Projects)
            {
                await FillUsersInfo(project.Users.ToList(), cancellationToken);
            }
        }

        var settings = await _timeTrackPortalSettingsAccessor.GetSettings(cancellationToken);

        ArgumentNullException.ThrowIfNull(settings.ExcelSettings);
        ArgumentNullException.ThrowIfNull(settings.ReportsAuthorName);

        var dateSpan = _dateService.GetDateSpan(request);
        var exportModel = new ClientsWithDatesToExportModel
        {
            Clients = clients,
            StartTime = dateSpan.StartDate,
            EndTime = dateSpan.EndDate,
            ExcelConfig = settings.ExcelSettings,
            ReportsAuthorName = settings.ReportsAuthorName
        };

        return await _clientsToExcelExporter.Export(exportModel, cancellationToken);
    }

    internal sealed record Query(
            int? Year,
            int[]? MonthOfYear,
            int? WeekOfYear,
            string? DepartmentId,
            int? ClientId,
            int? UserId,
            int? ResourceGroupId,
            TrackedTimeType TrackedTimeType)
        : IQuery<byte[]>, IPeriodRequest, ITimeTrackRequest;
}
