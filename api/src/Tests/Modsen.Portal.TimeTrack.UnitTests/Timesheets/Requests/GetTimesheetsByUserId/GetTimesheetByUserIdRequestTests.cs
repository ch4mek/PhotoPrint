using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.OData;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Calculators;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.TimeTrack.UnitTests.Timesheets.Requests.GetTimesheetsByUserId;
public class GetTimesheetByUserIdRequestTests
{
    private readonly Mock<ITimesheetBlockedDaysCalculator> _blockedDaysCalculatorMock = new();
    private readonly Mock<ICanGetUserTimesheetBusinessRule> _canGetTimesheetByUserIdBusinessRuleMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext>
        { CallBase = true }.As<IAuthenticatedUserContext>();
    private readonly Mock<ITimeTrackDbContext> _dbContextMock = new();

    private readonly GetTimesheetByUserIdRequest _handler;

    public GetTimesheetByUserIdRequestTests()
    {
        _handler = new GetTimesheetByUserIdRequest(
            _blockedDaysCalculatorMock.Object,
            _canGetTimesheetByUserIdBusinessRuleMock.Object,
            _authenticatedUserContextMock.Object,
            _dbContextMock.Object
        );
    }

    [Fact]
    public Task GetTimesheetByUserIdWhenNotAllowToGetUserTimesheetsThrowsInvalidDataAppException()
    {
        // Arrange
        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };

        bool? onlyManagedProjects = null;

        var queryOptions = ODataQueryOptionsMock.BuildMock<TimesheetOdpDto>(queryParameters);
        var query = new GetTimesheetByUserIdRequest.Query(queryOptions, 1, onlyManagedProjects, false);

        _canGetTimesheetByUserIdBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(query.UserId, onlyManagedProjects, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        return result.Should().ThrowAsync<ActionNotAllowedException>();
    }
}
