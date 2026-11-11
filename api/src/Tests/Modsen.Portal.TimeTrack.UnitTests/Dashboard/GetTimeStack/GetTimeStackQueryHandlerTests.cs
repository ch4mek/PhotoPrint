using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.Builders;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Dashboard.Requests;

namespace Modsen.Portal.TimeTrack.UnitTests.Dashboard.GetTimeStack;

public class GetTimeStackQueryHandlerTests
{
    private readonly Mock<IGetAccessedDepartmentIdForCurrentUserBusinessRule> _getAccessedDepartmentMock = new();
    private readonly Mock<ITimesheetQueryBuilder> _timesheetQueryBuilderMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;
    private readonly GetTimeStackRequest _handler;

    public GetTimeStackQueryHandlerTests()
    {
        _handler = new GetTimeStackRequest(
            _getAccessedDepartmentMock.Object,
            _authenticatedUserContextMock.Object,
            _timesheetQueryBuilderMock.Object);
    }

    [Fact]
    public async Task HandleWithValidRequestReturnsExpectedViewModelList()
    {
        // Arrange
        _getAccessedDepartmentMock.Setup(c => c.Execute("Net", _token))
            .ReturnsAsync("Net");

        var monthOfYear = new[] { 11, 12 };

        var data = new List<Timesheet>()
        {
            new()
            {
                ReportForDate = new DateOnly(2023, 11, 01),
                FactTrackedInMinutes = 4800,TaskName = "Test 1"
            },
            new()
            {
                ReportForDate = new DateOnly(2023, 11, 02),
                FactTrackedInMinutes = 4800,TaskName = "Test 1"
            },
            new()
            {
                ReportForDate = new DateOnly(2023, 11, 03),
                FactTrackedInMinutes = 4800,TaskName = "Test 1"
            }
        }.BuildMockDbSet();

        _timesheetQueryBuilderMock.Setup(x => x.FromAsNoTracked(It.IsAny<int?>(), It.IsAny<int[]?>()))
            .Returns(_timesheetQueryBuilderMock.Object);

        _timesheetQueryBuilderMock.Setup(x => x.WithDepartmentFilter(It.IsAny<string?>()))
            .Returns(_timesheetQueryBuilderMock.Object);

        _timesheetQueryBuilderMock.Setup(x => x.WithResourceGroupFilter(It.IsAny<int?>()))
            .Returns(_timesheetQueryBuilderMock.Object);

        _timesheetQueryBuilderMock.Setup(x => x.Build())
            .Returns(data.Object);

        var expectedResult = new List<TimeStackCoreDto>
        {
            new (new DateOnly(2023, 11, 01), 4800),
            new (new DateOnly(2023, 11, 02), 4800),
            new (new DateOnly(2023, 11, 03), 4800)
        };


        // Act
        var result = await _handler.Handle(new GetTimeStackRequest.Query(2023, monthOfYear, "Net"), _token);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
