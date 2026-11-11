using System.Linq.Expressions;
using Bogus;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Projects;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Core.Queries;
using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Excel.Models.Projects;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByExcel;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;
using TimeTrackUser = Modsen.Portal.TimeTrack.Domain.TimeTrackUser;

namespace Modsen.Portal.TimeTrack.UnitTests.Reports;

public class GetTrackedTimeByProjectInExcelAccountRequestTests
{
    private readonly Mock<ITeamBoardContract> _teamboardContractMock = new();
    private readonly Mock<IIdentityContract> _identityContractMock = new();
    private readonly Mock<IProjectToExcelExporter> _exporterMock = new();
    private readonly Mock<IGetProjectNameByProjectIdDbQuery> _getProjectNameByProjectIdMock = new();
    private readonly Mock<IDateService> _dateServiceMock = new();
    private readonly Mock<IGetTimesheetsWithClientInfoByPeriodWithFilterDbQuery> _getTimesheetDbQueryMock = new();
    private readonly Mock<ITimeTrackPortalSettingsAccessor> _settingsAccessorMock = new();
    private readonly Mock<ICanUserReportBeExportedBusinessRule> _canUserReportBeExportedBusinessRuleMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetTrackedTimeByProjectInExcelRequest _handlerBase;

    private readonly Faker<Timesheet> _faker = new();

    public GetTrackedTimeByProjectInExcelAccountRequestTests()
    {
        _handlerBase = new GetTrackedTimeByProjectInExcelRequest(
            _exporterMock.Object,
            _getProjectNameByProjectIdMock.Object,
            _dateServiceMock.Object,
            _getTimesheetDbQueryMock.Object,
            _settingsAccessorMock.Object,
            _canUserReportBeExportedBusinessRuleMock.Object,
            _teamboardContractMock.Object,
            _identityContractMock.Object);

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
            User = new TimeTrack.DataAccess.Npgsql.DatabaseEntities.TimeTrackUser
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

        _settingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TimeTrackPortalSettingsDto
            {
                ReportsAuthorName = "TEST",
                ExcelSettings = new TimeTrackPortalSettingsDto.Excel
                {
                    SummaryReportWorksheet = "Summary Report",
                    SummaryWorkingHoursReportWorksheet = "Working hours",
                    DefaultFontName = "DejaVu Sans",
                    BaseFontSize = 10,
                    NumberFormat = "0.00"
                }
            });
    }

    [Theory]
    [InlineData(2023, new int[] { }, null, "2023-01-01", "2023-12-31")]
    [InlineData(2023, new[] { 1 }, null, "2023-01-01", "2023-01-31")]
    [InlineData(2023, new[] { 1, 2 }, null, "2023-01-01", "2023-02-28")]
    [InlineData(2023, new[] { 5 }, 1, "2023-01-02", "2023-01-08")]
    public async Task HandleWhenTryToGetClientWithDifferentFiltersShouldReturnStreamWithExcel(
        int? year,
        int[] monthOfYear,
        int? weekOfYear,
        DateTime startDate,
        DateTime endDate)
    {
        // Arrange
        var query = new GetTrackedTimeByProjectInExcelRequest.Query(
            year,
            monthOfYear,
            weekOfYear,
            DepartmentId: null,
            ClientId: null,
            UserId: null,
            ResourceGroupId: null,
            ProjectId: 1,
            TrackedTimeType.Fact);

        var project = new Project
        {
            Id = 1,
            Name = "TEST",
            Color = "#ffffff",
            IsPublic = false,
            IsBillable = false,
            IsActive = true,
            ClientId = 1,
            CreatedById = 1,
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        _getProjectNameByProjectIdMock.Setup(x => x.GetProjectNameByProjectIdAsync(
                project.Id,
                _cancellationToken))
            .ReturnsAsync(project.Name);

        var timesheetsData = _faker.Generate(count: 5);

        var reportContent = Array.Empty<byte>();

        Expression<Func<GetTimesheetsWithClientInfoByPeriodWithFilterDbRequest, bool>> dbRequestMatch = x =>
            x.Year == query.Year
            && x.MonthOfYear == query.MonthOfYear
            && x.WeekOfYear == query.WeekOfYear;

        _getTimesheetDbQueryMock.Setup(x => x.ExecuteAsync(
                It.Is(dbRequestMatch),
                _cancellationToken))
            .ReturnsAsync(timesheetsData);

        Expression<Func<IPeriodRequest, bool>> requestMatch = x =>
            x.Year == year
            && x.MonthOfYear == monthOfYear
            && x.WeekOfYear == weekOfYear;

        var dateSpan = new DateSpanDto
        {
            StartDate = startDate,
            EndDate = endDate
        };

        _dateServiceMock.Setup(x => x.GetDateSpan(It.Is(requestMatch)))
            .Returns(dateSpan);

        var settings = await _settingsAccessorMock.Object.GetSettings(_cancellationToken);

        var exportModel = new ProjectWithDatesToExportModel
        {
            Project = It.IsAny<ProjectToExportModel>(),
            StartTime = startDate,
            EndTime = endDate,
            ExcelConfig = settings.ExcelSettings,
            ReportsAuthorName = "test",
            ClientName = "test"
        };

        _exporterMock.Setup(x => x.Export(exportModel, _cancellationToken))
            .ReturnsAsync(reportContent);

        // Act
        var result = await _handlerBase.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenTryToGetProjectTimesheetsShouldReturnStreamWithExcel()
    {
        // Arrange
        var currentDate = new DateTime(2023, 01, 01, 0, 0, 0, DateTimeKind.Utc);

        var query = new GetTrackedTimeByProjectInExcelRequest.Query(
            Year: null,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            ClientId: null,
            UserId: 1,
            ResourceGroupId: null,
            ProjectId: 1,
            TrackedTimeType.Fact);

        var project = new Project
        {
            Id = 1,
            Name = "TEST",
            Color = "#ffffff",
            IsPublic = false,
            IsBillable = false,
            IsActive = true,
            ClientId = 1,
            CreatedById = 1,
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        var timesheetsData = _faker.Generate(count: 5);

        var reportContent = Array.Empty<byte>();

        var timeTrackUser = new TimeTrackUser
        {
            UserId = query.UserId!.Value,
            FirstName = "Test",
            LastName = "Test",
            Email = "test@modsen-software.com",
            DepartmentId = "Js"
        };

        _getProjectNameByProjectIdMock.Setup(x => x.GetProjectNameByProjectIdAsync(
                project.Id,
                _cancellationToken))
            .ReturnsAsync(project.Name);

        _canUserReportBeExportedBusinessRuleMock.Setup(x =>
                x.IsCorrespondsToBusiness(timeTrackUser.UserId, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Success);

        Expression<Func<GetTimesheetsWithClientInfoByPeriodWithFilterDbRequest, bool>> dbRequestMatch = x =>
            x.Year == query.Year
            && x.MonthOfYear == query.MonthOfYear
            && x.WeekOfYear == query.WeekOfYear;

        _getTimesheetDbQueryMock.Setup(x => x.ExecuteAsync(
                It.Is(dbRequestMatch),
                _cancellationToken))
            .ReturnsAsync(timesheetsData);

        var settings = await _settingsAccessorMock.Object.GetSettings(_cancellationToken);

        var exportModel = new ProjectWithDatesToExportModel
        {
            Project = It.IsAny<ProjectToExportModel>(),
            StartTime = It.IsAny<DateTime>(),
            EndTime = currentDate,
            ExcelConfig = settings.ExcelSettings,
            ReportsAuthorName = "test",
            ClientName = "test"
        };

        _exporterMock.Setup(x => x.Export(exportModel, _cancellationToken))
            .ReturnsAsync(reportContent);

        Expression<Func<IPeriodRequest, bool>> requestMatch = x =>
            x.Year == query.Year
            && x.MonthOfYear == query.MonthOfYear
            && x.WeekOfYear == query.WeekOfYear;

        var dateSpan = new DateSpanDto
        {
            StartDate = DateTime.MinValue,
            EndDate = currentDate
        };

        _dateServiceMock.Setup(x => x.GetDateSpan(It.Is(requestMatch)))
            .Returns(dateSpan);

        // Act
        var result = await _handlerBase.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenTryToGetUserTimesheetsWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var query = new GetTrackedTimeByProjectInExcelRequest.Query(
            Year: null,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            ClientId: null,
            UserId: 1,
            ResourceGroupId: null,
            ProjectId: 1,
            TrackedTimeType.Fact);

        var project = new Project
        {
            Id = 1,
            Name = "TEST",
            Color = "#ffffff",
            IsPublic = false,
            IsBillable = false,
            IsActive = true,
            ClientId = 1,
            CreatedById = 1,
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        _getProjectNameByProjectIdMock.Setup(x => x.GetProjectNameByProjectIdAsync(
                project.Id,
                _cancellationToken))
            .ReturnsAsync(project.Name);

        _canUserReportBeExportedBusinessRuleMock.Setup(x =>
                x.IsCorrespondsToBusiness(1, _cancellationToken))
            .ThrowsAsync(new EntityNotFoundException());

        // Act
        var result = () => _handlerBase.Handle(query, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenTryToGetUserTimesheetsWhenUserFromDifferentDepartmentShouldThrowActionNotAllowedException()
    {
        // Arrange
        var query = new GetTrackedTimeByProjectInExcelRequest.Query(
            Year: null,
            MonthOfYear: [],
            WeekOfYear: null,
            DepartmentId: null,
            ClientId: null,
            UserId: 1,
            ResourceGroupId: null,
            ProjectId: 1,
            TrackedTimeType.Fact);

        var timeTrackUser = new TimeTrackUser
        {
            UserId = query.UserId!.Value,
            FirstName = "Test",
            LastName = "Test",
            Email = "test@modsen-software.com",
            DepartmentId = "Js"
        };

        var project = new Project
        {
            Id = 1,
            Name = "TEST",
            Color = "#ffffff",
            IsPublic = false,
            IsBillable = false,
            IsActive = true,
            ClientId = 1,
            CreatedById = 1,
            ProjectUsers = [],
            TechStack = null,
            Industry = null,
            CloudServices = null,
            Architecture = null
        };

        _getProjectNameByProjectIdMock.Setup(x => x.GetProjectNameByProjectIdAsync(
                project.Id,
                _cancellationToken))
            .ReturnsAsync(project.Name);

        _canUserReportBeExportedBusinessRuleMock.Setup(x =>
                x.IsCorrespondsToBusiness(timeTrackUser.UserId, _cancellationToken))
            .ReturnsAsync(BusinessRuleResult.Fail("Fail"));

        // Act
        var result = () => _handlerBase.Handle(query, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }
}
