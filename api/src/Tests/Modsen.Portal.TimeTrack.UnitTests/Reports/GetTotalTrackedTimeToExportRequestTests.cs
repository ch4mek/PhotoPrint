using System.Linq.Expressions;
using Modsen.Portal.Core;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.StimulsoftReport.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;
using ReportRequest = Modsen.Portal.StimulsoftReport.Contract.ReportRequest;

namespace Modsen.Portal.TimeTrack.UnitTests.Reports;

public class GetTotalTrackedTimeToExportRequestTests
{
    private readonly Mock<IReportExporter> _exporterMock = new();
    private readonly Mock<IDateService> _dateService = new();
    private readonly Mock<IPeriodService> _periodService = new();
    private readonly Mock<IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery> _getTimesheetDbQuery = new();
    private readonly Mock<ITimeTrackPortalSettingsAccessor> _settingsAccessorMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetTotalTrackedTimeToExportRequest _useCase;

    public GetTotalTrackedTimeToExportRequestTests()
    {
        _useCase = new GetTotalTrackedTimeToExportRequest(
            _exporterMock.Object,
            _dateService.Object,
            _periodService.Object,
            _getTimesheetDbQuery.Object,
            _settingsAccessorMock.Object
        );

        _settingsAccessorMock
            .Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TimeTrackPortalSettingsDto
            {
                DefaultTimesheetReportName = "TimeReportSummary",
                ReportsAuthorName = "Modsen Company"
            });
    }

    [Theory]
    [InlineData(2023, new int[] { }, null, DocumentType.Pdf)]
    [InlineData(2023, new[] { 1 }, null, DocumentType.Word)]
    [InlineData(2023, new[] { 1, 2 }, null, DocumentType.Pdf)]
    [InlineData(2023, new[] { 5 }, 1, DocumentType.Word)]
    public async Task HandleWhenTryToGetTimesheetsWithDifferentFiltersShouldReturnExpectedString(
        int? year,
        int[] monthOfYear,
        int? weekOfYear,
        DocumentType format)
    {
        // Arrange
        var query = new GetTotalTrackedTimeToExportRequest.Query(
            year,
            monthOfYear,
            weekOfYear,
            DepartmentId: null,
            UserId: null,
            ResourceGroupId: null,
            format,
            ReportName: It.IsAny<string>(),
            TrackedTimeType: TrackedTimeType.Fact);

        var timesheetsData = new List<Timesheet>
        {
            new()
            {
                TaskName = "task1",
                FactTrackedInMinutes = 180,
                BillableTrackedInMinutes = 0,
                ReportForDate = default,
                ReportYear = 2023,
                ReportMonthOfYear = 1,
                ReportWeekOfYear = 1,
                ReportDayOfYear = 1,
                UserId = 1,
                User = new TimeTrackUser
                {
                    FirstName = "TEST",Email = "test",
                    LastName = "TEST"
                },
                ProjectId = 1,
                Project = new Project
                {
                    Id = 1,
                    Name = "TEST",
                    ClientId = 1,
                    Color = "#101010",
                    Client = new Client
                    {
                        Id = 1,
                        Name = "TEST",
                    },
                }
            },
            new()
            {
                TaskName = "task2",
                FactTrackedInMinutes = 180,
                BillableTrackedInMinutes = 0,
                ReportForDate = default,
                ReportYear = 2023,
                ReportMonthOfYear = 1,
                ReportWeekOfYear = 1,
                ReportDayOfYear = 2,
                UserId = 1,
                ProjectId = 1,
                User = new TimeTrackUser
                {
                    FirstName = "TEST",Email = "test",
                    LastName = "TEST"
                },
                Project = new Project
                {
                    Id = 1,
                    Name = "TEST",
                    ClientId = 1,
                    Color = "#101010",
                    Client = new Client
                    {
                        Id = 1,
                        Name = "TEST",
                    },
                }
            }
        };

        var reportContent = Array.Empty<byte>();

        _getTimesheetDbQuery.Setup(x => x.ExecuteAsync(
                It.IsAny<GetTimesheetsWithClientInfoByPeriodWithFilterDbRequest>(),
                _cancellationToken))
            .ReturnsAsync(timesheetsData);

        _dateService.Setup(x => x.GetDateSpan(query))
            .Returns(new DateSpanDto());

        Expression<Func<ReportRequest, bool>> reportRequestMatch = request =>
            request.ReportName == query.ReportName
            && request.TemplateType == TemplateType.Total
            && request.RootDataObject != null
            && request.TitleObject != null;

        _exporterMock.Setup(x => x.ExportPdfAsync(It.Is(reportRequestMatch), _cancellationToken))
            .ReturnsAsync(reportContent);

        _exporterMock.Setup(x => x.ExportWordAsync(It.Is(reportRequestMatch), _cancellationToken))
            .ReturnsAsync(reportContent);

        // Act
        var result = await _useCase.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeSameAs(reportContent);
    }
}
