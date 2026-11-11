using System.Linq.Expressions;
using Bogus;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.StimulsoftReport.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users.UserNames;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft.Models;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;
using ReportRequest = Modsen.Portal.StimulsoftReport.Contract.ReportRequest;

namespace Modsen.Portal.TimeTrack.UnitTests.Reports;

public class GetTrackedTimeByUserIdToExportRequestTests
{
    private readonly Mock<IReportExporter> _exporterMock = new();
    private readonly Mock<IDateService> _dateService = new();
    private readonly Mock<IPeriodService> _periodService = new();
    private readonly Mock<IGetUserNamesResponseByUserIdDbQuery> _getUserNamesResponseByUserIdMock = new();
    private readonly Mock<IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery> _getTimesheetDbQueryMock = new();
    private readonly Mock<ITimeTrackPortalSettingsAccessor> _settingsAccessorMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetTrackedTimeByUserIdToExportRequest _handlerBase;

    private readonly Faker<Timesheet> _faker = new();

    public GetTrackedTimeByUserIdToExportRequestTests()
    {
        _handlerBase = new GetTrackedTimeByUserIdToExportRequest(
            _exporterMock.Object,
            _dateService.Object,
            _periodService.Object,
            _getUserNamesResponseByUserIdMock.Object,
            _getTimesheetDbQueryMock.Object,
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
            ProjectId = 1,
            User = new TimeTrackUser
            {
                FirstName = "FirstName",
                Email = "test",
                LastName = "LastName"
            },
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
    public async Task HandleWhenTryToGetUserTimesheetsWithDifferentFiltersShouldReturnStream(
        int? year,
        int[] monthOfYear,
        int? weekOfYear,
        DocumentType format)
    {
        // Arrange
        var query = new GetTrackedTimeByUserIdToExportRequest.Query(
            year,
            monthOfYear,
            weekOfYear,
            DepartmentId: null,
            ProjectId: null,
            ClientId: null,
            UserId: 1,
            ResourceGroupId: null,
            format,
            ReportName: "summary report",
            TrackedTimeType.Fact);

        var timesheetsData = _faker.Generate(count: 5);

        var user = new TimeTrackUser
        {
            UserId = 1,
            DepartmentId = "Net",
            Email = "test@modsen-software.com",
            FirstName = "Test",
            LastName = "Test"
        };

        var reportContent = Array.Empty<byte>();

        Expression<Func<GetTimesheetsWithClientInfoByPeriodWithFilterDbRequest, bool>> dbRequestMatch = x =>
            x.Year == query.Year
            && x.MonthOfYear == query.MonthOfYear
            && x.WeekOfYear == query.WeekOfYear;

        _getTimesheetDbQueryMock.Setup(x => x.ExecuteAsync(
                It.Is(dbRequestMatch), _cancellationToken))
            .ReturnsAsync(timesheetsData);

        _dateService.Setup(x => x.GetDateSpan(query))
            .Returns(new DateSpanDto());

        _getUserNamesResponseByUserIdMock.Setup(x => x.GetUserNamesDtoByUserIdAsync(
                query.UserId, _cancellationToken))
            .ReturnsAsync(new UserNamesResponse(user.FirstName, user.LastName));

        Expression<Func<ReportRequest, bool>> reportRequestMatch = request =>
            request.ReportName == query.ReportName
            && request.TemplateType == TemplateType.PerEmployee
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
    public async Task HandleWhenTryToGetUserTimesheetsWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var query = new GetTrackedTimeByUserIdToExportRequest.Query(
            Year: null,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            ProjectId: null,
            ClientId: null,
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
