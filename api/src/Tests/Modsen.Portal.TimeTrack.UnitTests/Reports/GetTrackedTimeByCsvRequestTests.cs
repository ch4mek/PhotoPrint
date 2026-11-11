using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.TimeTrackPortalSettings;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv;
using Modsen.Portal.TimeTrack.UseCases.Queries.Reports.Exporters.Csv.Models;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;
using Timesheet = Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities.Timesheet;
using TimeTrackUser = Modsen.Portal.TimeTrack.Domain.TimeTrackUser;

namespace Modsen.Portal.TimeTrack.UnitTests.Reports;

public class GetTrackedTimeByCsvRequestTests
{
    private readonly Mock<IReportToCsvExporter> _exporterMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ITimeTrackUserProvider> _timeTrackUserProviderMock = new();
    private readonly Mock<ITimeTrackPortalSettingsAccessor> _settingsAccessorMock = new();
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly GetTrackedTimeByCsvRequest _handlerBase;

    public GetTrackedTimeByCsvRequestTests()
    {
        _handlerBase = new GetTrackedTimeByCsvRequest(
            _exporterMock.Object,
            _authenticatedUserContextMock.Object,
            _timeTrackUserProviderMock.Object,
            _settingsAccessorMock.Object,
            _dbContextMock.Object
        );

        _settingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TimeTrackPortalSettingsDto
            {
                ExcelSettings = new TimeTrackPortalSettingsDto.Excel
                {
                    SummaryReportWorksheet = "Summary Report",
                    SummaryWorkingHoursReportWorksheet = "Working hours",
                    DefaultFontName = "DejaVu Sans",
                    BaseFontSize = 10,
                    NumberFormat = "0.00",
                    DefaultDateFormat = "dd/MM/yyyy"
                }
            });
    }

    [Fact]
    public async Task HandleWhenTryToGetProjectTimesheetsShouldReturnStreamWithExcel()
    {
        // Arrange
        var query = new GetTrackedTimeByCsvRequest.Query(
            null, [], null, null, null, null, null, null);

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
                ProjectId = 1
            }
        };

        var tasks = new List<TaskToCsvExportModel>
        {
            new()
            {
                Date = default,
                Description = "task1",
                TimeInMinutes = 180
            }
        };

        var streamWithCsv = Array.Empty<byte>();

        _dbContextMock
            .Setup(context => context.Timesheets)
            .Returns(timesheetsData.BuildMockDbSet().Object);

        var settings = await _settingsAccessorMock.Object.GetSettings(_cancellationToken);

        _exporterMock.Setup(x => x.Export(It.Is<TasksToCsvExportModel>(model =>
            model.Tasks.Count() == tasks.Count &&
            !model.Tasks.Where((t, i) =>
                t.Date != tasks[i].Date ||
                t.Description != tasks[i].Description ||
                t.TimeInMinutes != tasks[i].TimeInMinutes).Any() &&
            model.DateFormat == settings.ExcelSettings.DefaultDateFormat), _cancellationToken))
            .ReturnsAsync(streamWithCsv);

        // Act
        var result = await _handlerBase.Handle(query, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(streamWithCsv);
    }

    [Fact]
    public async Task HandleWhenTryToGetUserTimesheetsWhenUserDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var query = new GetTrackedTimeByCsvRequest.Query(
            null, [], null, null, null, null, null, 1);

        _timeTrackUserProviderMock.Setup(x => x.GetByIdAsync(query.UserId!.Value, _cancellationToken))
            .ReturnsAsync((TimeTrackUser?)null);

        // Act
        var result = () => _handlerBase.Handle(query, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenTryToGetUserTimesheetsWhenUserFromDifferentDepartmentShouldThrowActionNotAllowedException()
    {
        // Arrange
        var query = new GetTrackedTimeByCsvRequest.Query(
            null, [], null, null, null, null, null, 1);

        var timeTrackUser = new TimeTrackUser
        {
            UserId = query.UserId!.Value,
            FirstName = "Test",
            LastName = "Test",
            Email = "test@modsen-software.com",
            DepartmentId = "Js"
        };

        _timeTrackUserProviderMock.Setup(x => x.GetByIdAsync(query.UserId!.Value, _cancellationToken))
            .ReturnsAsync(timeTrackUser);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns("Net");

        // Act
        var result = () => _handlerBase.Handle(query, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenTryToGetUserTimesheetsWhenUserFromDifferentGroupShouldThrowActionNotAllowedException()
    {
        // Arrange
        var query = new GetTrackedTimeByCsvRequest.Query(
            null, [], null, null, null, null, null, 1);

        var timeTrackUser = new TimeTrackUser
        {
            UserId = query.UserId!.Value,
            FirstName = "Test",
            LastName = "Test",
            Email = "test@modsen-software.com",
            DepartmentId = "Java",
            ResourceGroupId = 2
        };

        _timeTrackUserProviderMock.Setup(x => x.GetByIdAsync(query.UserId!.Value, _cancellationToken))
            .ReturnsAsync(timeTrackUser);

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId)
            .Returns("Net");
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId)
            .Returns(1);

        // Act
        var result = () => _handlerBase.Handle(query, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }
}
