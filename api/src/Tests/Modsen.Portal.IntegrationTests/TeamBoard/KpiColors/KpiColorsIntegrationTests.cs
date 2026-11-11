using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.KpiColors;

[Collection("GeneralCollection")]
public class KpiColorsIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    public KpiColorsIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Theory]
    [InlineData(PortalRoleNames.Cto)]
    [InlineData(PortalRoleNames.Ceo)]
    public async Task GetKpiColorsWhenAuthAsChiefShouldReturnKpiColors(string roleName)
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsRole(roleName);

        //Act
        var colors = await client.GetKpiColorsAsync();

        //Assert
        colors.Should().NotBeNullOrEmpty();
        colors.Count.Should().Be(3);
    }

    [Theory]
    [InlineData(PortalRoleNames.Employee)]
    [InlineData(PortalRoleNames.GroupManager)]
    [InlineData(PortalRoleNames.HeadOfDepartment)]
    [InlineData(PortalRoleNames.SystemAdministrator)]
    public async Task GetKpiColorsWhenAuthAsRoleWhichHasNoAccessShouldReturnKpiColors(string roleName)
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsRole(roleName);

        //Act
        var response = await client.GetKpiColorsAsHttpMessageAsync();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateKpiColorsWhenAuthAsCeoAndRequestShouldReturnUpdatedKpiColors()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var colors = await client.GetKpiColorsAsync();

        var colorsToUpdate = colors.Select(
            color => new UpdateKpiColorDto
            {
                Id = color.Id,
                RightIntervalBorder = colors.Last().Id == color.Id
                    ? color.RightIntervalBorder
                    : color.RightIntervalBorder + 1
            }).ToArray();

        //Act
        var result = await client.UpdateKpiColorsAsync(colorsToUpdate);

        //Assert
        result.Should().NotBeNullOrEmpty();
        result.Count.Should().Be(3);
        result[0].RightIntervalBorder.Should().Be(colors[0].RightIntervalBorder + 1);
        result[1].RightIntervalBorder.Should().Be(colors[1].RightIntervalBorder + 1);
        result[2].RightIntervalBorder.Should().Be(colors[2].RightIntervalBorder);
    }

    [Fact]
    public async Task UpdateKpiColorsWhenUpdateLastColorIntervalShouldReturnBadRequest()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var colors = await client.GetKpiColorsAsync();

        var colorsToUpdate = colors.Select(
            color => new UpdateKpiColorDto
            {
                Id = color.Id,
                RightIntervalBorder = color.RightIntervalBorder + 1
            }).ToArray();

        //Act
        var response = await client.UpdateKpiColorsAsHttpMessageAsync(colorsToUpdate);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateKpiColorsWhenNotAllColorsUpdatedShouldReturnBadRequest()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var colors = await client.GetKpiColorsAsync();

        var colorsToUpdate = colors.Select(
            color => new UpdateKpiColorDto
            {
                Id = color.Id,
                RightIntervalBorder = color.RightIntervalBorder + 1
            })
            .Skip(1)
            .ToArray();

        //Act
        var response = await client.UpdateKpiColorsAsHttpMessageAsync(colorsToUpdate);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateKpiColorsWhenIntervalBordersAreNegativeShouldReturnBadRequest()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var colors = await client.GetKpiColorsAsync();

        var colorsToUpdate = colors.Select(
                color => new UpdateKpiColorDto { Id = color.Id, RightIntervalBorder = -1 })
            .ToArray();

        //Act
        var response = await client.UpdateKpiColorsAsHttpMessageAsync(colorsToUpdate);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateKpiColorsWhenIntervalBordersAreGreaterThanValidShouldReturnBadRequest()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var colors = await client.GetKpiColorsAsync();

        var colorsToUpdate = colors.Select(
            color => new UpdateKpiColorDto
            {
                Id = color.Id,
                RightIntervalBorder = 10000
            })
            .ToArray();

        //Act
        var response = await client.UpdateKpiColorsAsHttpMessageAsync(colorsToUpdate);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateKpiColorsWhenIntervalBordersAreNotValidShouldReturnBadRequest()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var colors = await client.GetKpiColorsAsync();

        var colorsToUpdate = colors.Select(
                color => new UpdateKpiColorDto
                {
                    Id = color.Id,
                    RightIntervalBorder = colors.Last().Id == color.Id
                        ? color.RightIntervalBorder
                        : color.RightIntervalBorder + 1
                })
            .ToArray();

        colorsToUpdate[1].RightIntervalBorder = colorsToUpdate[0].RightIntervalBorder - 10;

        //Act
        var response = await client.UpdateKpiColorsAsHttpMessageAsync(colorsToUpdate);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        // var errorMessage = await response.Content.ReadAsStringAsync(_factory.CancellationToken);
        // errorMessage.Should().Contain("The set orange value must be greater than the set red value");
    }

    [Fact]
    public async Task UpdateKpiColorsWhenLastColorIntervalBorderIsNotValid2ShouldReturnBadRequest()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var colorsToUpdate = new List<UpdateKpiColorDto>
        {
            new() { Id = 1, RightIntervalBorder = 30 },
            new() { Id = 2, RightIntervalBorder = 35 },
            new() { Id = 3, RightIntervalBorder = 9998 }
        }.ToArray();

        colorsToUpdate[1].RightIntervalBorder = colorsToUpdate[0].RightIntervalBorder - 10;

        //Act
        var response = await client.UpdateKpiColorsAsHttpMessageAsync(colorsToUpdate);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        // var errorMessage = await response.Content.ReadAsStringAsync(_factory.CancellationToken);
        // errorMessage.Should().Contain("Last color interval border must be 9999");
    }

}
