using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.TimesheetController;

[Collection("TimeTrackerCollection")]
public class GetUserSubmitStatusIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetUserSubmitStatusIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetSubmitStatusWhenAuthAsCeoCtoShouldResponseWithOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();
        var userId = _factory.EmployeeUserId!.Value;

        await client.UpdateTimesheetStatusAsync(userId: userId, month: 3, year: 2024, status: TimesheetSubmitStatusType.UnderReview);

        await client.AuthAsCeo();
        await client.UpdateTimesheetStatusAsync(userId: userId, month: 3, year: 2024, status: TimesheetSubmitStatusType.Rejected);

        // Act
        var getResponse = await client.GetTimesheetStatusAsync(userId: userId, month: 3, year: 2024);

        // Assert
        getResponse.Should().NotBeNull();
        getResponse.UserId.Should().Be(userId);
        getResponse.Status.Should().Be(TimesheetSubmitStatusType.Rejected);
    }

    [Fact]
    public async Task GetSubmitStatusWhenRequestIsNotValidShouldResponseWithForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;
        const int month = 123;
        const TimesheetSubmitStatusType status = TimesheetSubmitStatusType.Approved;

        await client.UpdateTimesheetStatusAsync(userId: userId, month: 1, year: 2024, status: status);

        // Act
        var getResponse = await client.GetTimesheetStatusAsHttpMessageAsync(userId: userId, month: month, year: 2024);

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetSubmitStatusWhenAuthAsEmployeeOrSysAdminTryGetAnotherUserStatusShouldResponseWithForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var date = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));

        var user = await client.CreateEmployee(departmentId: "Java");
        var userId = user!.Id;

        const TimesheetSubmitStatusType status = TimesheetSubmitStatusType.UnderReview;

        await client.AuthAs(user.Email);

        await client.UpdateTimesheetStatusAsync(userId: userId, month: date.Month,
            year: date.Year,
            status: status);

        await client.AuthAsEmployee();

        // Act
        var getResponse = await client.GetTimesheetStatusAsHttpMessageAsync(userId: userId,
            month: date.Month,
            year: date.Year);

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSubmitStatusWhenAuthAsEmployeeOrSysAdminShouldResponseWithOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsAdmin();

        var userId = _factory.AdminUserId!.Value;

        // Act
        var getResponse = await client.GetTimesheetStatusAsHttpMessageAsync(userId: userId, month: 2, year: 2024);

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSubmitStatusWhenAuthAsHeadGetStatusOfUserFromHisDepartmentShouldResponseWithOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var date = DateOnly.FromDateTime(DateTime.Now.AddMonths(3));

        var user = await client.CreateEmployee();
        var userId = user!.Id;

        await client.AuthAs(user.Email);

        const TimesheetSubmitStatusType status = TimesheetSubmitStatusType.UnderReview;

        await client.UpdateTimesheetStatusAsync(userId: userId, month: date.Month, year: date.Year,
            status: status);

        await client.AuthAsHeadOfDepartment();

        // Act
        var getResponse = await client.GetTimesheetStatusAsHttpMessageAsync(userId: userId,
            month: date.Month,
            year: date.Year);

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSubmitStatusWhenAuthAsHeadGetStatusOfUserNotFromHisDepartmentShouldResponseForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var date = DateOnly.FromDateTime(DateTime.Now.AddMonths(2));

        var user = await client.CreateEmployee(departmentId: "Java");
        var userId = user!.Id;

        const TimesheetSubmitStatusType status = TimesheetSubmitStatusType.UnderReview;

        await client.AuthAs(user.Email);

        await client.UpdateTimesheetStatusAsync(userId: userId, month: date.Month, year: date.Year,
            status: status);

        await client.AuthAsHeadOfDepartment();

        // Act
        var getResponse = await client.GetTimesheetStatusAsHttpMessageAsync(userId: userId,
            month: date.Month,
            year: date.Year);

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
