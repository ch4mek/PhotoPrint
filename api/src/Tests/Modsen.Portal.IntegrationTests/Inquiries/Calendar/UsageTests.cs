using System.Net;
using System.Text.Json;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Inquiries;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Inquiries.Calendar;

[Collection("InquiriesCollection")]
public class UsageTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public UsageTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CreateValidExclusionShouldCreate()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var exclusion = new CalendarExclusionInput { Name = "Name", Date = new DateOnly(1, 2, 3), CountryId = 1, };

        // Act
        var response = await client.CreateCalendarExclusionAsync(exclusion);

        // Assert
        response.Should().BeEquivalentTo(exclusion);
    }

    [Fact]
    public async Task CreateDoubleExclusionMustBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var exclusion = new CalendarExclusionInput { Name = "Name", Date = new DateOnly(1, 2, 3), CountryId = 1, };

        // Act
        await client.CreateCalendarExclusionAsHttpResponseAsync(exclusion);
        var second = await client.CreateCalendarExclusionAsHttpResponseAsync(exclusion);

        // Assert
        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task GetAllExclusionsShouldReturnAllExclusions()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var exclusions = new List<CalendarExclusionInput> {
            new() { Name = "Name1", Date = new DateOnly(1, 2, 3), CountryId = 2, },
            new() { Name = "Name2", Date = new DateOnly(1, 2, 4), CountryId = 2, },
            new() { Name = "Name3", Date = new DateOnly(1, 2, 5), CountryId = 2, },
        };

        await client.CreateCalendarExclusionAsync(exclusions[0]);
        await client.CreateCalendarExclusionAsync(exclusions[1]);
        await client.CreateCalendarExclusionAsync(exclusions[2]);

        // Act
        var response = await client.GetAllCalendarExclusionAsync(2, 100, 0);

        // Assert
        response.Total.Should().Be(exclusions.Count);
        response.Items.Count.Should().Be(exclusions.Count);
    }
}
