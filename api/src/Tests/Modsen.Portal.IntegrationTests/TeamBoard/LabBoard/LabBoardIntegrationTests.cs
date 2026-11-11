using System.Net;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TeamBoard.LabBoard;

[Collection("GeneralCollection")]
public class LabBoardIntegrationTests
{
    private readonly IntegrationTestFactory _factory;

    public LabBoardIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("")]
    [InlineData("?")]
    [InlineData("?top=50")]
    [InlineData("?skip=0")]
    [InlineData("?top=0&skip=50")]
    [InlineData("?top=50&skip=-1")]
    [InlineData("?top=50&skip=15")]
    [InlineData("?top=26&skip=50")]
    [InlineData("?top=50&skip=50&filter=Id eq a")]
    public async Task GetPageableInternsWhenRequestIsInvalidResponseWithBadRequest(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetPageableInterns(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("?top=20&skip=0")]
    [InlineData("?top=10&skip=50")]
    [InlineData("?top=50&skip=50&orderby=Id")]
    [InlineData("?top=20&skip=0&filter=id eq 7")]
    public async Task GetPageableInternsWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetPageableInterns(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetLabBoardInternLookupWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetLabBoardInternLookup("?includeInactive=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetInternshipWhenInternExistsResponseWithNoContent()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var intern = await client.CreateIntern();

        // Act
        var response = await client.GetInternship(intern!.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task EditInternshipWhenInternExistsResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var intern = await client.CreateIntern();

        var request = new EditInternshipInput
        {
            Id = null,
            Specialization = "Internship",
            Type = InternshipType.Primary,
            EngineerStage = 1,
            CompletedPoints = null,
            ApprovedPoints = null,
            FinalResult = null,
            StartDate = DateOnly.MinValue,
            PlannedEndDate = DateOnly.MaxValue,
            InternshipSpecializations = []
        };

        // Act
        var response = await client.EditInternship(intern!.Id, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
