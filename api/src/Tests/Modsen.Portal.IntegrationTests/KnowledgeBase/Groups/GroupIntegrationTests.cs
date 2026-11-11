using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.KnowledgeBase;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.KnowledgeBase.Controllers.Contracts.Input;
using Modsen.Portal.KnowledgeBase.Requests.ArticleGroups.Core;
namespace Modsen.Portal.IntegrationTests.KnowledgeBase.Groups;

[Collection("KnowledgeBaseCollection")]
public class GroupIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GroupIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task SetGroupOrderResponseWithNoContentResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        using var scope = _factory.Services.CreateScope();

        var firstGroup = await client.CreateGroup();
        await client.CreateGroup(parentGroupId: firstGroup.Id);

        await client.CreateArticle(1);
        await client.CreateArticle(1);

        var input = new GroupOrderInput
        {
            GroupOrders = new List<GroupOrderDto>()
            {
                new()
                {
                    ParentGroupId = null,
                    Id = 1,
                    ArticleOrders =
                    [
                        new() { Id = 1, OrderWeight = 100, }
                    ],
                    ChildGroups = [],
                    OrderWeight = 100,
                }
            }
        };

        // Act
        var response = await client.SetGroupOrderAsHttpMessage(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task SetGroupOrderResponseWithBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        using var scope = _factory.Services.CreateScope();

        var firstGroup = await client.CreateGroup();
        await client.CreateGroup(parentGroupId: firstGroup.Id);

        await client.CreateArticle(1);
        await client.CreateArticle(1);

        var input = new GroupOrderInput
        {
            GroupOrders = new List<GroupOrderDto>()
            {
                new()
                {
                    ParentGroupId = null,
                    Id = 1,
                    ArticleOrders =
                    [
                        new() { Id = 100, OrderWeight = 100, }
                    ],
                    ChildGroups = [],
                    OrderWeight = 100,
                }
            }
        };

        // Act
        var response = await client.SetGroupOrderAsHttpMessage(input);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
