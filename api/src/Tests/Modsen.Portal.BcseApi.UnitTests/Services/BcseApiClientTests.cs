using System.Net;
using System.Text;
using AwesomeAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.BcseApi.Adapter;
using Modsen.Portal.BcseApi.Contract;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Utils.Modules;
using Moq;
using Moq.Protected;
using Xunit;

namespace Modsen.Portal.BcseApi.UnitTests.Services;

public sealed class BcseApiClientTests
{
    private readonly Uri BaseAddress = new Uri("https://test-bcse.by", UriKind.Absolute);

    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
    private readonly IServiceScope _scope;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public BcseApiClientTests()
    {
        _scope = CreateServiceScope();
    }

    [Fact]
    public async Task GetСurrentExchangeRatesWhenResponseStatusCodeIsNotSuccessThrowAppException()
    {
        // Arrange
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            ReasonPhrase = "Some error occured"
        };

        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(BcseApiConstants.BcseApiHttpClientName))
            .Returns(httpClient);

        // Act
        var bcseApiClient = _scope.ServiceProvider.GetRequiredService<IBcseApiClient>();
        var action = () => bcseApiClient.GetCurrentExchangeRates(_cancellationToken);

        // Assert
        await action.Should().ThrowAsync<AppException>($"HTTP Error during getting exchange rates from Bcse API. Reason: {response.ReasonPhrase}");
    }

    [Fact]
    public async Task GetСurrentExchangeRatesWhenResponseIsEmptyReturnEmptyList()
    {
        // Arrange
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };

        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(BcseApiConstants.BcseApiHttpClientName))
            .Returns(httpClient);

        // Act
        var bcseApiClient = _scope.ServiceProvider.GetRequiredService<IBcseApiClient>();
        var result = await bcseApiClient.GetCurrentExchangeRates(_cancellationToken);

        // Assert
        result.ExchangeRates.Should().BeEmpty();
    }

    [Fact]
    public async Task GetСurrentExchangeRatesWhenResponseHasUnexpectedMessageFormatThrowAppException()
    {
        // Arrange
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                """
                {
                  "rates": [
                    {
                      "toggle": "Южная Африка",
                      "include": "zar_f",
                      "rateCoef": "10 ZAR",
                      "rateNB": 1.7019
                    }
                  ]
                }
                """,
                Encoding.UTF8,
                "application/json")
        };

        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(BcseApiConstants.BcseApiHttpClientName))
            .Returns(httpClient);

        // Act
        var bcseApiClient = _scope.ServiceProvider.GetRequiredService<IBcseApiClient>();
        var action = () => bcseApiClient.GetCurrentExchangeRates(_cancellationToken);

        // Assert
        await action.Should().ThrowAsync<AppException>();
    }

    [Fact]
    public async Task GetСurrentExchangeRatesWhenResponseHasUnexpectedRateCoefFormatThrowAppException()
    {
        // Arrange
        using var response = GetHttpResponseMessage(rateCoef: "10ZAR");
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(BcseApiConstants.BcseApiHttpClientName))
            .Returns(httpClient);

        // Act
        var bcseApiClient = _scope.ServiceProvider.GetRequiredService<IBcseApiClient>();
        var action = () => bcseApiClient.GetCurrentExchangeRates(_cancellationToken);

        // Assert
        await action.Should().ThrowAsync<AppException>($"Error during parsing rateCoef from Bcse API response. RateCoef from Bcse API response: 10ZAR");
    }

    [Fact]
    public async Task GetExchangeRatesByDatesWhenDatesIsNullReturnEmptyList()
    {        
        // Act
        var bcseApiClient = _scope.ServiceProvider.GetRequiredService<IBcseApiClient>();        
        var result = await bcseApiClient.GetExchangeRatesByDates(null!, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _httpClientFactoryMock.Verify(x => x.CreateClient(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetExchangeRatesByDatesWhenSeveralDatesReturnExchangeRatesForAllDates()
    {
        // Arrange
        var firstDate = new DateOnly(2026, 7, 28);
        var secondDate = new DateOnly(2026, 7, 29);

        using var response1 = GetHttpResponseMessage();
        using var response2 = GetHttpResponseMessage();

        using var httpClient = CreateMockHttpClient(response1, response2);       

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        // Act
        var bcseApiClient = _scope.ServiceProvider.GetRequiredService<IBcseApiClient>();
        var result = await bcseApiClient.GetExchangeRatesByDates([firstDate, secondDate], CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);

        result.Should().ContainSingle(x => x.Date == firstDate);
        result.Should().ContainSingle(x => x.Date == secondDate);
    }

    [Fact]
    public async Task GetExchangeRatesByDatesWhenExchangeRateApiResponseIsNullReturnEmptyRates()
    {
        var targetDate = new DateOnly(2026, 7, 28);
        using var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("null") };
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        // Act
        var bcseApiClient = _scope.ServiceProvider.GetRequiredService<IBcseApiClient>();
        var result = await bcseApiClient.GetExchangeRatesByDates([targetDate], _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result.Single().Date.Should().Be(targetDate);
        result.Single().ExchangeRates.Should().BeEmpty();
    }

    private IServiceScope CreateServiceScope()
    {
        var configurationData = new Dictionary<string, string>
        {
            {"AppSettings:BcseApi:BcseApiBaseUrl", "https://test-bcse.by"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData!)
            .Build();

        var services = new ServiceCollection();
        var rootModule = new RegistrationModuleAggregate(configuration);

        rootModule.AddModule<BcseApiAdapterModule>();
        rootModule.LoadRoot(services);

        services.AddSingleton(_ => _httpClientFactoryMock.Object);

        return services
            .BuildServiceProvider()
            .CreateScope();
    }

    private HttpClient CreateMockHttpClient(HttpResponseMessage response)
    {
        _httpMessageHandlerMock
          .Protected()
          .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
          )
          .ReturnsAsync(response);

        return new HttpClient(_httpMessageHandlerMock.Object) { BaseAddress = BaseAddress };
    }

    private HttpClient CreateMockHttpClient(HttpResponseMessage response1, HttpResponseMessage response2)
    {
        _httpMessageHandlerMock
            .Protected()
            .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response1)
            .ReturnsAsync(response2);

        return new HttpClient(_httpMessageHandlerMock.Object) { BaseAddress = BaseAddress };
    }

    private HttpResponseMessage GetHttpResponseMessage(string rateCoef = "10 ZAR")
    {
        return new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(
                $$"""
                {
                  "rates": [
                    {
                      "toggle": "Южная Африка",
                      "include": "zar_f",
                      "currency": "рэнд",
                      "rateCoef": "{{rateCoef}}",
                      "rateNB": 1.7019
                    }
                  ]
                }
                """,
                Encoding.UTF8,
                "application/json")
        };
    }
}
