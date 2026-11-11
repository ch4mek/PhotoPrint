using System.Text.Json;
using System.Text.Json.Nodes;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.Requests.Microfrontends.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.MicroFrontend;

public class GetMicrofrontendsOptionsRequestTests
{
    private readonly GetMicrofrontendsOptionsRequest request;
    private readonly Mock<IAuthenticatedUserContext> _userContext;

    public GetMicrofrontendsOptionsRequestTests()
    {
        var jsonObject = new
        {
            microfrontends = new[]
            {
            
                new
                {
                    name = "mms-cv-mfe",
                    claims = new[] { "ManageCV", "ViewCv" }
                },
                new
                {
                    name = "public-mfe",
                    claims = new[] { "ManageMfe" }
                }
            }
        };
        var json = JsonSerializer.Serialize(jsonObject);
        var cache = new Mock<IMicrofrontendsCache>();
        cache.Setup(x => x.GetMicrofrontendsConfig(It.IsAny<CancellationToken>()))
            .Returns(ValueTask.FromResult(new MicrofrontendsCacheRecord
            {
                Id = 1,
                JsonData = json
            }));
        
        _userContext = new Mock<IAuthenticatedUserContext>();
        request = new GetMicrofrontendsOptionsRequest(_userContext.Object, cache.Object);
    }

    [Fact]
    public async Task HandleShouldReturnFullResponseWhenUserWithManageMfe()
    {
        // Arrange
        _userContext.Setup(x => x.GetUserClaimsNames())
            .Returns(["ManageMfe"]);

        var expected = JsonSerializer.Serialize(new
        {
            microfrontends = new[]
            {

                new
                {
                    name = "mms-cv-mfe",
                    claims = new[] { "ManageCV", "ViewCv" }
                },
                new
                {
                    name = "public-mfe",
                    claims = new[] { "ManageMfe" }
                }
            }
        });
        var jsonOptions = new JsonSerializerOptions();

        // Act
        var actual = await request.Handle(new GetMicrofrontendsOptionsRequest.Query(), CancellationToken.None);

        // Assert
        JsonNode.Parse(actual)?.ToJsonString(jsonOptions).Should().Be(JsonNode.Parse(expected)?.ToJsonString(jsonOptions));
    }

    [Fact]
    public async Task HandleShouldReturnResponseWithoutPublicMfeWhenUserWithoutManageMfe()
    {
        // Arrange
        _userContext.Setup(x => x.GetUserClaimsNames())
            .Returns(["ViewCv"]);

        var expected = JsonSerializer.Serialize(new
        {
            microfrontends = new[]
            {
                new
                {
                    name = "mms-cv-mfe",
                    claims = new[] { "ManageCV", "ViewCv" }
                }
            }
        });
        var jsonOptions = new JsonSerializerOptions();

        // Act
        var actual = await request.Handle(new GetMicrofrontendsOptionsRequest.Query(), CancellationToken.None);

        // Assert
        JsonNode.Parse(actual)?.ToJsonString(jsonOptions).Should().Be(JsonNode.Parse(expected)?.ToJsonString(jsonOptions));
    }
}
