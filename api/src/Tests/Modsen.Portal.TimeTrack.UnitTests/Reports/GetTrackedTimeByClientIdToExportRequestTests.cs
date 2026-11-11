using System.Linq.Expressions;
using Bogus;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.StimulsoftReport.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Clients;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft.Models;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;
using ReportRequest = Modsen.Portal.StimulsoftReport.Contract.ReportRequest;

namespace Modsen.Portal.TimeTrack.UnitTests.Reports;

public class GetTrackedTimeByClientIdToExportRequestTests
{
    private readonly Mock<IReportExporter> _exporterMock = new();
    private readonly Mock<IDateService> _dateService = new();
    private readonly Mock<IPeriodService> _periodService = new();
    private readonly Mock<IGetClientNameByClientIdDbQuery> _getClientNameByClientIdMock = new();
    private readonly Mock<IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery> _getTimesheetDbQuery = new();
    private readonly Mock<ITimeTrackPortalSettingsAccessor> _settingsAccessorMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly Faker<Timesheet> _faker = new();

    private readonly GetTrackedTimeByClientIdToExportRequest _handlerBase;

    public GetTrackedTimeByClientIdToExportRequestTests()
    {
        _handlerBase = new GetTrackedTimeByClientIdToExportRequest(
            _exporterMock.Object,
            _dateService.Object,
            _periodService.Object,
            _getClientNameByClientIdMock.Object,
            _getTimesheetDbQuery.Object,
            _settingsAccessorMock.Object
        );

        _faker.CustomInstantiator(f => new Timesheet
        {
            TaskName = f.Lorem.Sentence(),
            ReportForDate = new DateOnly(2023, 06, 04),
            ReportYear = 2023,
            ReportMonthOfYear = 6,
            ReportWeekOfYear = 25,
            ReportDayOfYear = 183,
            FactTrackedInMinutes = 180,
            BillableTrackedInMinutes = 180,
            UserId = 1,
            User = new TimeTrackUser
            {
                FirstName = "FirstName",
                Email = "test",
                LastName = "LastName"
            },
            ProjectId = 1,
            Project = new Project
            {
                Id = 1,
                Name = "Project Name",
                ClientId = 1,
                Color = f.Internet.Color(),
                Client = new Client
                {
                    Id = 1,
                    Name = f.Company.CompanyName(),
                },
            }
        });

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
    public async Task HandleWhenTryToGetClientTimesheetsWithDifferentFiltersShouldReturnStream(
        int? year,
        int[] monthOfYear,
        int? weekOfYear,
        DocumentType format)
    {
        // Arrange
        var query = new GetTrackedTimeByClientIdToExportRequest.Query(
            year, monthOfYear, weekOfYear,
            DepartmentId: null,
            ClientId: 1,
            UserId: null,
            ResourceGroupId: null,
            format,
            ReportName: "summary report",
            TrackedTimeType.Fact);

        var timesheetsData = _faker.Generate(count: 3);

        var reportContent = Array.Empty<byte>();

        var clientName = "Client1";

        _getClientNameByClientIdMock.Setup(x => x.GetClientNameByClientIdAsync(
            query.ClientId, _cancellationToken)).ReturnsAsync(clientName);

        Expression<Func<GetTimesheetsWithClientInfoByPeriodWithFilterDbRequest, bool>>
            dbRequestMatch = x =>
                x.Year == year
                && x.MonthOfYear == monthOfYear
                && x.WeekOfYear == weekOfYear;

        _getTimesheetDbQuery.Setup(x => x.ExecuteAsync(
                It.Is(dbRequestMatch),
                _cancellationToken))
            .ReturnsAsync(timesheetsData);

        _dateService.Setup(x => x.GetDateSpan(query))
            .Returns(new DateSpanDto());

        Expression<Func<ReportRequest, bool>> reportRequestMatch = request =>
            request.ReportName == query.ReportName
            && request.TemplateType == TemplateType.PerClient
            && request.RootDataObject is List<ProjectToExport>
            && request.TitleObject is ReportTitle;

        _exporterMock.Setup(x => x.ExportPdfAsync(It.Is(reportRequestMatch), _cancellationToken))
            .ReturnsAsync(reportContent);

        _exporterMock.Setup(x => x.ExportWordAsync(It.Is(reportRequestMatch), _cancellationToken))
            .ReturnsAsync(reportContent);

        // Act
        var result = await _handlerBase.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeSameAs(reportContent);
    }

    [Fact]
    public async Task HandleWhenTryToGetClientTimesheetsWhenClientDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var query = new GetTrackedTimeByClientIdToExportRequest.Query(
            Year: null,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            ClientId: 1,
            UserId: 1,
            ResourceGroupId: null,
            Format: DocumentType.Pdf,
            ReportName: It.IsAny<string>(),
            TrackedTimeType.Fact);

        // Act
        var result = () => _handlerBase.Handle(query, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }
}
