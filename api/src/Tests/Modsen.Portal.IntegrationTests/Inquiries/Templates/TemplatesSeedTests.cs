using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;

namespace Modsen.Portal.IntegrationTests.Inquiries.Templates;

[Collection("InquiriesCollection")]
public class TemplatesSeedTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly JsonSerializerOptions _serializerOptions;

    public TemplatesSeedTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _jsonOptions;
    }

    [Fact]
    public async Task GetTemplatesSeedMustContainAllTypes()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        // Act
        var response = await client.GetAsync(
            "api/inquiry-templates",
            _cancellationToken);

        var seedTemplates =
            await response.Content.ReadFromJsonAsync<List<InquiryTemplateOutput>>(
                _serializerOptions,
                _cancellationToken);

        var types = seedTemplates.Select(x => x.Type).Distinct();

        // Assert
        types.Should().Contain([
            InquiryType.SickLeave,
            InquiryType.SickDay,
            InquiryType.HomeOffice,
            InquiryType.OwnExpense,
            InquiryType.Vacation,
            InquiryType.WeekendTransfer,
            InquiryType.SocialVacation,
            InquiryType.MilitaryEnlistmentOffice,
        ]);
    }
}
