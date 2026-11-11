using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TeamBoard.WorkingHoursPerMonths;

[Collection("GeneralCollection")]
public class WorkingHoursPerMonthIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public WorkingHoursPerMonthIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CreateWorkingHoursPerMonthWhenRequestIsValidShouldResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var configuration = await client.GetConfigurationAsync();
        var countryId = configuration!.ConfigurableValues.Countries.First().Id;

        var workingHoursToCreate = new CreateWorkingHoursPerMonthInput
        {
            Year = 2023,
            MonthOfYear = 1,
            CountryId = countryId,
            Hours = 12
        };

        var expectedOutput = new WorkingHoursPerMonthOutput
        {
            Id = 1,
            Year = 2023,
            MonthOfYear = 1,
            CountryId = countryId,
            Hours = 12
        };

        // Act
        var workingHoursPerMonth = await client.CreateWorkingHoursPerMonth(workingHoursToCreate)
            .WithResponseType<WorkingHoursPerMonthOutput>();

        // Assert
        workingHoursPerMonth.Year.Should().Be(expectedOutput.Year);
        workingHoursPerMonth.MonthOfYear.Should().Be(expectedOutput.MonthOfYear);
        workingHoursPerMonth.CountryId.Should().Be(expectedOutput.CountryId);
        workingHoursPerMonth.Hours.Should().Be(expectedOutput.Hours);

        var workingHoursSheet = await client.GetWorkingHours(expectedOutput.Year)
            .WithResponseType<WorkingHoursPerMonthsSheetOutput>();

        workingHoursSheet.HoursPerMonth.First(x => x.Id == workingHoursPerMonth.Id)
            .Should().BeEquivalentTo(workingHoursPerMonth);
    }

    [Fact]
    public async Task CreateWorkingHoursPerMonthWhenTheSameEntryExistsShouldResponseWithConflict()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var year = 2025;
        var countryId = 1;
        var monthOfYear = 1;

        var workingHoursToCreate = new CreateWorkingHoursPerMonthInput
        {
            Year = year,
            MonthOfYear = monthOfYear,
            CountryId = countryId,
            Hours = 12
        };

        await client.CreateWorkingHoursPerMonth(workingHoursToCreate);

        // Act
        var response = await client.CreateWorkingHoursPerMonth(
                workingHoursToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateWorkingHoursPerMonthWhenCountryDoesNotExistShouldResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var workingHoursToCreate = new CreateWorkingHoursPerMonthInput
        {
            Year = 2023,
            MonthOfYear = 2,
            CountryId = int.MaxValue,
            Hours = 12
        };

        // Act
        var response = await client.CreateWorkingHoursPerMonth(workingHoursToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateWorkingHoursPerMonthWhenEntryDoesNotExistsShouldResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var workingHoursToUpdate = new UpdateWorkingHoursPerMonthInput
        {
            Id = int.MaxValue,
            Hours = 12
        };

        // Act
        var response = await client.UpdateWorkingHoursPerMonth(workingHoursToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateWorkingHoursPerMonthWhenRequestIsValidShouldResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var configuration = await client.GetConfigurationAsync();
        var countryId = configuration!.ConfigurableValues.Countries.First().Id;

        var workingHoursToCreate = new CreateWorkingHoursPerMonthInput
        {
            Year = 2022,
            MonthOfYear = 5,
            CountryId = countryId,
            Hours = 12
        };

        var workingHoursPerMonth = await client
            .CreateWorkingHoursPerMonth(workingHoursToCreate)
            .WithResponseType<WorkingHoursPerMonthOutput>();

        var expectedOutput = new WorkingHoursPerMonthOutput
        {
            Id = workingHoursPerMonth.Id,
            Year = workingHoursPerMonth.Year,
            MonthOfYear = workingHoursPerMonth.MonthOfYear,
            CountryId = countryId,
            Hours = 24
        };

        // Act
        var updatedWorkingHoursPerMonth = await client
            .UpdateWorkingHoursPerMonth(
                new UpdateWorkingHoursPerMonthInput
                {
                    Id = workingHoursPerMonth.Id,
                    Hours = 24
                }).WithResponseType<WorkingHoursPerMonthOutput>();

        // Assert
        updatedWorkingHoursPerMonth.Should().BeEquivalentTo(expectedOutput);

        var workingHoursSheet = await client.GetWorkingHours(expectedOutput.Year)
            .WithResponseType<WorkingHoursPerMonthsSheetOutput>();

        workingHoursSheet.HoursPerMonth.First(x => x.Id == updatedWorkingHoursPerMonth.Id)
            .Should().BeEquivalentTo(updatedWorkingHoursPerMonth);
    }

    [Theory]
    [InlineData(PortalRoleNames.Ceo)]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.GroupManager)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    public async Task GetWorkingHoursPerMonthWhenRequestIsValidShouldResponseWithOk(string role)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsRole(role);

        // Act
        var response = await client.GetWorkingHours(2024);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
