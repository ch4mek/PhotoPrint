using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Inquiries.Templates;

[Collection("InquiriesCollection")]
public class ValidationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ValidationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task CreateWithNoPoliciesMustBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        // Act
        var response = await client.PostAsJsonAsync(
            "api/inquiry-templates",
            new InquiryTemplateInput
            {
                Name = "Name",
            },
            _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateActiveWhenNameNonUniqueMustBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        await client.PostAsJsonAsync(
            "api/inquiry-templates",
            new InquiryTemplateInput
            {
                Name = "Name",
                Type = InquiryType.Vacation
            },
            _cancellationToken);

        // Act
        var response = await client.PostAsJsonAsync(
            "api/inquiry-templates",
            new InquiryTemplateInput
            {
                Name = "Name",
                Type = InquiryType.Vacation
            },
            _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateWhenInvalidInputContractMustBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        // Act
        var a = await client.PostAsJsonAsync(
            "api/inquiry-templates",
            new InquiryTemplateInput
            {
                Name = "Name",
            },
            _cancellationToken);

        var b = await client.PostAsJsonAsync(
            "api/inquiry-templates",
            new InquiryTemplateInput
            {
                Name = "Name",
            },
            _cancellationToken);

        // Assert
        a.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        b.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
