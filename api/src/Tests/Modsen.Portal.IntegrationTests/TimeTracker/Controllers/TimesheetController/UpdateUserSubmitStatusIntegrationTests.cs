using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
namespace Modsen.Portal.IntegrationTests.TimeTracker.Controllers.TimesheetController;

[Collection("TimeTrackerCollection")]
public class UpdateUserSubmitStatusIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UpdateUserSubmitStatusIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task UpdateStatusOnSubmittedWhenAuthAsEmployeeOrSysAdminShouldResponseOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        var user = await client.CreateEmployee(departmentId: "Java");
        var userId = user!.Id;

        await client.AuthAs(user.Email);

        await client.UpdateTimesheetStatusAsync(userId: userId, month: currentDate.Month, year: currentDate.Year,
            status: TimesheetSubmitStatusType.UnderReview);

        // Act
        var getResponse = await client.GetTimesheetStatusAsync(userId: userId,
            month: currentDate.Month,
            year: currentDate.Year);

        // Assert
        getResponse.UserId.Should().Be(userId);
        getResponse.Status.Should().Be(TimesheetSubmitStatusType.UnderReview);
    }

    [Fact]
    public async Task UpdateStatusOnSubmittedWhenAuthAsEmployeeOrSysAdminUpdateAnotherUserStatusShouldResponseForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var user = await client.CreateEmployee(departmentId: "Java");
        var userId = user!.Id;

        await client.AuthAsEmployee();

        // Act
        var response = await client.UpdateTimesheetStatusAsHttpMessageAsync(userId: userId, month: 1, year: 2024,
            status: TimesheetSubmitStatusType.Rejected);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateStatusOnSubmittedWhenAuthAsCeoCtoHeadShouldResponseWithForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var response = await client.UpdateTimesheetStatusAsHttpMessageAsync(userId: userId, month: 1, year: 2024,
            status: TimesheetSubmitStatusType.UnderReview);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateStatusOnApprovedOrRejectedWhenAuthAsEmployeeOrSysAdminShouldResponseWithForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var userId = _factory.EmployeeUserId!.Value;

        // Act
        var createResponse = await client.UpdateTimesheetStatusAsHttpMessageAsync(userId: userId, month: 1, year: 2024,
            status: TimesheetSubmitStatusType.Rejected);

        // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateStatusForEmployeeOnApprovedOrRejectedWhenAuthAsCeoCtoShouldResponseWithOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        var user = await client.CreateEmployee(departmentId: "Java");
        var userId = user!.Id;

        // Act
        var createResponse = await client.UpdateTimesheetStatusAsHttpMessageAsync(userId: userId,
            month: currentDate.Month,
            year: currentDate.Year,
            status: TimesheetSubmitStatusType.Approved);

        // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateStatusOnApprovedOrRejectedWhenAuthAsHeadUpdateStatusForUserFromHisDepartmentShouldResponseWithOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var currentDate = DateOnly.FromDateTime(DateTime.Now);

        var user = await client.CreateEmployee();
        var userId = user!.Id;

        await client.AuthAs(user.Email);

        await client.UpdateTimesheetStatusAsHttpMessageAsync(userId: userId,
            month: currentDate.Month,
            year: currentDate.Year,
            status: TimesheetSubmitStatusType.UnderReview);

        await client.AuthAsHeadOfDepartment();

        // Act
        var updateResponse = await client.UpdateTimesheetStatusAsHttpMessageAsync(userId: userId,
            month: currentDate.Month,
            year: currentDate.Year,
            status: TimesheetSubmitStatusType.Approved);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateStatusOnApprovedOrRejectedWhenAuthAsHeadUpdateStatusForUserNotFromHisDepartmentShouldResponseWithForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var user = await client.CreateEmployee(departmentId: "Java");
        var userId = user!.Id;

        await client.AuthAsHeadOfDepartment();

        // Act
        var createResponse = await client.UpdateTimesheetStatusAsHttpMessageAsync(userId: userId, month: 1,
            year: 2024,
            status: TimesheetSubmitStatusType.Rejected);

        // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
