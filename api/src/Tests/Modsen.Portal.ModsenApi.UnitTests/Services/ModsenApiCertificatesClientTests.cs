using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.ModsenApi.Adapter;
using Modsen.Portal.ModsenApi.Adapter.ApiModels;
using Modsen.Portal.ModsenApi.Contract;
using Moq;
using Moq.Protected;

namespace Modsen.Portal.ModsenApi.UnitTests.Services;

public sealed class ModsenApiCertificatesClientTests : IDisposable
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly IServiceScope _scope;

    public ModsenApiCertificatesClientTests()
    {
        _scope = CreateServiceScope();
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    [Fact]
    public async Task GetUserCertificatesWhenResponseStatusCodeIsNotSuccessLogErrorAndThrowAppException()
    {
        // arrange
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            ReasonPhrase = "Some error occured"
        };
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        // act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var action = () => modsenApiCertClient.GetUserCertificates("1", _cancellationToken);

        // assert
        await action.Should().ThrowAsync<AppException>($"HTTP Error during getting information from Modsen API (Get User Certificates). Reason: {response.ReasonPhrase}");
    }

    [Fact]
    public async Task GetUserCertificatesWhenResponseIsEmptyLogWarningAndThrowAppException()
    {
        // arrange
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        // act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var action = () => modsenApiCertClient.GetUserCertificates("1", _cancellationToken);

        // assert
        await action.Should().ThrowAsync<AppException>("Received empty response from Modsen API (Get User Certificates) or unexpected message format");
    }

    [Fact]
    public async Task GetUserCertificatesWhenResultIsNotSuccessLogWarningAndThrowAppException()
    {
        // arrange
        var responseModel = new ListOfCertificatesApiModel
        {
            IsSuccess = false,
            Certificates = []
        };

        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responseModel), Encoding.UTF8, "application/json")
        };
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        // act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var action = () => modsenApiCertClient.GetUserCertificates("1", _cancellationToken);

        // assert
        await action.Should().ThrowAsync<AppException>("Received failed response from Modsen API (Get User Certificates)");
    }

    [Fact]
    public async Task GetUserCertificatesWhenValidRequestReturnListOfCertificates()
    {
        // arrange
        var responseModel = new ListOfCertificatesApiModel
        {
            IsSuccess = true,
            Certificates =
            [
                new CertificateApiModel
                {
                    CertificateId = "1",
                    ReceiptDate = new DateTime(2023, 05, 05, 0, 0, 0, DateTimeKind.Utc),
                    Recipient = "test data",
                    RecipientId = "1",
                    JsStackName = "test data",
                    TechnologyName = "test data",
                    CreatedAtUtc = new DateTime(2023, 05, 05, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAtUtc = new DateTime(2023, 05, 05, 0, 0, 0, DateTimeKind.Utc)
                }
            ]
        };

        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responseModel), Encoding.UTF8, "application/json")
        };
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        // act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var result = await modsenApiCertClient.GetUserCertificates("1", _cancellationToken);

        // assert
        result.Should().NotBeNull();
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateUserCertificateWhenResponseHasConflictStatusCodeThrowCertificateAlreadyExistsException()
    {
        // arrange
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.Conflict
        };
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHealthCheckHttpClientName))
            .Returns(httpClient);

        // act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var action = () => modsenApiCertClient.CreateUserCertificate(new CreateCertificateModel(), "", _cancellationToken);

        // assert
        await action.Should().ThrowAsync<CertificateAlreadyExistsException>();
    }

    [Fact]
    public async Task CreateUserCertificateWhenResponseStatusCodeIsNotSuccessLogErrorAndThrowAppException()
    {
        // arrange
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            ReasonPhrase = "Some error occured"
        };
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHealthCheckHttpClientName))
            .Returns(httpClient);

        // act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var action = () => modsenApiCertClient.CreateUserCertificate(new CreateCertificateModel(), "", _cancellationToken);

        // assert
        await action.Should().ThrowAsync<AppException>($"HTTP Error during creating certificate using Modsen API (Create User Certificate). Reason: {response.ReasonPhrase}");
    }

    [Fact]
    public async Task CreateUserCertificateWhenResponseIsEmptyLogWarningAndThrowAppException()
    {
        // arrange
        using var response = new HttpResponseMessage();
        using var httpClient = CreateMockHttpClient(response);

        response.StatusCode = HttpStatusCode.OK;
        response.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHealthCheckHttpClientName))
            .Returns(httpClient);

        // act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var action = () => modsenApiCertClient.CreateUserCertificate(new CreateCertificateModel(), "", _cancellationToken);

        // assert
        await action.Should().ThrowAsync<AppException>("Received empty response from Modsen API (Create User Certificate) or unexpected message format");
    }

    [Fact]
    public async Task CreateUserCertificateWhenCertificateIsNullLogWarningAndThrowAppException()
    {
        // arrange
        var responseModel = new ResponseCertificateApiModel
        {
            IsSuccess = true,
            Certificate = null
        };

        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responseModel), Encoding.UTF8, "application/json")
        };
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHealthCheckHttpClientName))
            .Returns(httpClient);

        // act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var action = () => modsenApiCertClient.CreateUserCertificate(new CreateCertificateModel(), "", _cancellationToken);

        // assert
        await action.Should().ThrowAsync<AppException>("Received empty response from Modsen API (Create User Certificate) or unexpected message format");
    }

    [Fact]
    public async Task CreateUserCertificateWhenResultIsNotSuccessLogWarningAndThrowAppException()
    {
        // arrange
        var responseModel = new ResponseCertificateApiModel
        {
            IsSuccess = false,
            Certificate = new CertificateApiModel()
        };

        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responseModel), Encoding.UTF8, "application/json")
        };
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHealthCheckHttpClientName))
            .Returns(httpClient);

        // act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var action = () => modsenApiCertClient.CreateUserCertificate(new CreateCertificateModel(), "", _cancellationToken);

        // assert
        await action.Should().ThrowAsync<AppException>("Received failed response from Modsen API (Create User Certificate)");
    }

    [Fact]
    public async Task CreateUserCertificateWhenValidRequestReturnCertificateModel()
    {
        // arrange
        var responseModel = new ResponseCertificateApiModel
        {
            IsSuccess = true,
            Certificate = new CertificateApiModel
            {
                CertificateId = "1",
                ReceiptDate = new DateTime(2023, 05, 05, 0, 0, 0, DateTimeKind.Utc),
                Recipient = "test data",
                RecipientId = "1",
                JsStackName = "test data",
                TechnologyName = "test data",
                CreatedAtUtc = new DateTime(2023, 05, 05, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2023, 05, 05, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responseModel), Encoding.UTF8, "application/json")
        };
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHealthCheckHttpClientName))
            .Returns(httpClient);

        // act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var result = await modsenApiCertClient.CreateUserCertificate(new CreateCertificateModel(), "", _cancellationToken);

        // assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteUserCertificateWhenResponseStatusCodeIsNotSuccessLogErrorAndThrowAppException()
    {
        // arrange
        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError,
            ReasonPhrase = "Some error occured"
        };
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        // act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var action = () => modsenApiCertClient.DeleteUserCertificate("1", _cancellationToken);

        // assert
        await action.Should().ThrowAsync<AppException>($"HTTP Error during deleting certificate using Modsen API (Create User Certificate). Reason: {response.ReasonPhrase}");
    }

    [Fact]
    public async Task DeleteUserCertificateWhenResponseIsEmptyLogWarningAndThrowAppException()
    {
        // Arrange
        using var response = new HttpResponseMessage();
        using var httpClient = CreateMockHttpClient(response);

        response.StatusCode = HttpStatusCode.OK;
        response.Content = new StringContent("{}", Encoding.UTF8, "application/json");

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        // Act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var action = () => modsenApiCertClient.DeleteUserCertificate("1", _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<AppException>("Received empty response from Modsen API (Delete User Certificate) or unexpected message format");
    }

    [Fact]
    public async Task DeleteUserCertificateWhenCertificateIsNullLogWarningAndThrowAppException()
    {
        // Arrange
        var responseModel = new ResponseCertificateApiModel
        {
            IsSuccess = true,
            Certificate = null
        };

        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responseModel), Encoding.UTF8, "application/json")
        };
        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        // Act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var action = () => modsenApiCertClient.DeleteUserCertificate("1", _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<AppException>("Received empty response from Modsen API (Delete User Certificate) or unexpected message format");
    }

    [Fact]
    public async Task DeleteUserCertificateWhenResultIsNotSuccessLogWarningAndThrowAppException()
    {
        // Arrange
        var responseModel = new ResponseCertificateApiModel
        {
            IsSuccess = false,
            Certificate = new CertificateApiModel()
        };

        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responseModel), Encoding.UTF8, "application/json")
        };

        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        // Act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var action = () => modsenApiCertClient.DeleteUserCertificate("1", _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<AppException>("Received failed response from Modsen API (Delete User Certificate)");
    }

    [Fact]
    public async Task DeleteUserCertificateWhenValidRequestReturnCertificateModel()
    {
        // Arrange
        var certificateId = Guid.CreateVersion7().ToAlphabetic();

        var responseModel = new ResponseCertificateApiModel
        {
            IsSuccess = true,
            Certificate = new CertificateApiModel
            {
                CertificateId = certificateId,
                ReceiptDate = new DateTime(2023, 05, 05, 0, 0, 0, DateTimeKind.Utc),
                Recipient = "test data",
                RecipientId = "1",
                JsStackName = "test data",
                TechnologyName = "test data",
                CreatedAtUtc = new DateTime(2023, 05, 05, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAtUtc = new DateTime(2023, 05, 05, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        using var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responseModel), Encoding.UTF8, "application/json")
        };

        using var httpClient = CreateMockHttpClient(response);

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(ModsenApiConstants.CertificatesHttpClientName))
            .Returns(httpClient);

        // Act
        var modsenApiCertClient = _scope.ServiceProvider.GetRequiredService<IModsenApiCertificatesClient>();
        var result = await modsenApiCertClient.DeleteUserCertificate(certificateId, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
    }

    private IServiceScope CreateServiceScope()
    {
        var configurationData = new Dictionary<string, string>
        {
            {"AppSettings:CertificateSettings:ApiProdCertificateUrl", "https://www.test-prod.com/certificates"},
            {"AppSettings:CertificateSettings:ApiCertificateUrl", "https://www.test.com/certificates"},
            {"AppSettings:CertificateSettings:ApiKeyCertificate", "0000000000"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData!)
            .Build();

        var services = new ServiceCollection();
        var rootModule = new RegistrationModuleAggregate(configuration);

        rootModule.AddModule<ModsenApiAdapterModule>();
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

        return new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://www.test.com/certificates", UriKind.Absolute)
        };
    }
}
