using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Merch;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.Merch.Storage;

[Collection("MerchCollection")]
public class StorageControllerIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public StorageControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetItemsStorageAsyncWhenUserHasNoClaimReturnForbidden()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        //Act
        var result = await client.GetMerchStorageAsHttpMessage(top: 1, skip: 0, new StorageQueryOptionsDto());

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);

    }

    [Fact]
    public async Task GetItemsStorageAsyncWhenUserHasClaimReturnOk()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        //Act
        var result = await client.GetMerchStorageAsHttpMessage(top: 1, skip: 0, new StorageQueryOptionsDto());

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);

    }

    [Fact]
    public async Task GetItemsStorageAsyncWhenTopIsLessThan1ReturnBadRequest()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        //Act
        var result = await client.GetMerchStorageAsHttpMessage(top: 0, skip: 0, new StorageQueryOptionsDto());

        //Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    }
}
