using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Attendance.DataAccess.Npgsql.OData;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Attendance;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.Attendance.Attendances;

[Collection("AttendanceCollection")]
public class AttendancesControllerIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public AttendancesControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetDetailedAttendancesWhenUserIsCeoResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetDetailedAttendancesAsHttpMessage(new Dictionary<string, string?>
        {
            ["$top"] = "10", ["$skip"] = "0"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDetailedAttendancesWhenUserIsCtoResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.GetDetailedAttendancesAsHttpMessage(new Dictionary<string, string?>
        {
            ["$top"] = "10", ["$skip"] = "0"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDetailedAttendancesWhenUserIsHeadOfDepartmentResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetDetailedAttendancesAsHttpMessage(new Dictionary<string, string?>
        {
            ["$top"] = "10", ["$skip"] = "0"
        });
        var items =
            await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpAttendance>>(_jsonOptions) ??
            throw new InvalidDataException();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        items.Items.Should().Contain(x => x.UserDepartmentId == "Net");
        items.Items.Should().NotContain(x => x.UserDepartmentId == "Java");
    }

    [Fact]
    public async Task GetDetailedAttendancesWhenUserIsGroupManagerResponseWithOkResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsGroupManager();

        // Act
        var response = await client.GetDetailedAttendancesAsHttpMessage(new Dictionary<string, string?>
        {
            ["$top"] = "10", ["$skip"] = "0"
        });
        var items =
            await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpAttendance>>(_jsonOptions) ??
            throw new InvalidDataException();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        items.Items.Should().Contain(x => x.UserResourceGroupName == ".NET Minsk - Group 1");
        items.Items.Should().NotContain(x => x.UserResourceGroupName == "Java Minsk - Group 1");
        items.Items.Should().NotContain(x => x.UserResourceGroupName == "Java Minsk - Group 2");
    }

    [Fact]
    public async Task GetDetailedAttendancesWhenUserIsEmployeeResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetDetailedAttendancesAsHttpMessage(new Dictionary<string, string?>
        {
            ["$top"] = "10", ["$skip"] = "0"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData(EmailConstants.CeoEmail)]
    [InlineData(EmailConstants.CtoEmail)]
    public async Task GetAttendancesTotalWhenAuthedAsChiefResponseWithAllUsers(string email)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAs(email);

        // Act
        var response = await client.GetOdpAttendancesTotal(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        });

        // Assert
        response!.Items.Select(x => x.UserId).Distinct().Should().HaveCount(5);
    }

    [Fact]
    public async Task GetAttendancesTotalWhenAuthedAsHoDResponseWithSameDepartmentUsers()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        // Act
        var response = await client.GetOdpAttendancesTotal(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        });

        // Assert
        response!.Items.Should().OnlyContain(x => x.UserDepartmentId == "Net");
    }

    [Fact]
    public async Task GetAttendancesTotalWhenAuthedAsGmResponseWithSameGroupUsers()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsGroupManager();

        // Act
        var response = await client.GetOdpAttendancesTotal(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        });

        // Assert
        response!.Items.Should().OnlyContain(x => x.UserResourceGroupName == ".NET Minsk - Group 1");
    }

    [Fact]
    public async Task GetAttendancesTotalWhenAuthedAsEmployeeResponseForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetOdpAttendancesTotalAsHttpMessage(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "10",
            [nameof(ODataRawQueryOptions.Skip)] = "0",
        });

        // Assert
        response!.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("$top=20&$skip=0")]
    [InlineData("$top=20&$skip=0&$filter=userId eq 1000002")]
    [InlineData("$top=20&$skip=0&$filter=contains(tolower(userFullName), tolower('Test5'))")]
    [InlineData("$top=20&$skip=0&$filter=userDepartmentId in ('Net')")]
    [InlineData("$top=20&$skip=0&$filter=contains(tolower(userResourceGroupName), tolower('.NET Minsk - Group 1'))")]
    [InlineData("$top=20&$skip=0&$filter=(date(date) ge 2026-01-01) and (date(date) le 2027-01-01)")]
    public async Task GetAttendancesTotalWhenRequestIsValidResponseOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.GetOdpAttendancesTotalAsHttpMessage(query);

        // Assert
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
