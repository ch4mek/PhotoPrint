using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Jira;
using Modsen.Portal.Jira.Client;
using Modsen.Portal.Jira.Contract.Client;
using Modsen.Portal.Jira.Contract.Exceptions;
using Modsen.Portal.Jira.Providers;

namespace Modsen.Portal.TimeTrack.UnitTests.Services;

public sealed class JiraServiceTests : IDisposable
{
    private const string JiraUrl = "https://jira.example.com";
    private const string ProjectKey = "MMS";
    private const string Pat = "jira-pat";
    private const string Email = "employee@company.com";

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly Mock<IJiraApiClientFactory> _jiraApiClientFactoryMock = new();
    private readonly List<HttpClient> _httpClients = [];

    public void Dispose()
    {
        foreach (var httpClient in _httpClients)
        {
            httpClient.Dispose();
        }
    }

    [Fact]
    public async Task CheckCredentialsWhenV2ReturnsOkShouldCallV2Only()
    {
        // Arrange
        using var handler = new JiraHttpMessageHandler(CreateJsonResponse(HttpStatusCode.OK));
        var service = CreateService(handler);

        // Act
        await service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        handler.Requests.Should().ContainSingle();
        handler.Requests.Single().RequestUri!.AbsoluteUri.Should()
            .Be("https://jira.example.com/rest/api/2/search?jql=project=MMS");
        handler.Requests.Single().Headers.Authorization!.Scheme.Should().Be("Bearer");
        handler.Requests.Single().Headers.Authorization!.Parameter.Should().Be(Pat);
    }

    [Fact]
    public async Task CheckCredentialsWhenJiraUrlDoesNotContainSchemeShouldUseHttps()
    {
        // Arrange
        using var handler = new JiraHttpMessageHandler(CreateJsonResponse(HttpStatusCode.OK));
        var service = CreateService(handler);

        // Act
        await service.CheckAccessAsync("jira.example.com", ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        handler.Requests.Should().ContainSingle();
        handler.Requests.Single().RequestUri!.AbsoluteUri.Should()
            .Be("https://jira.example.com/rest/api/2/search?jql=project=MMS");
    }

    [Fact]
    public async Task CheckCredentialsWhenJiraUrlSchemeIsNotSupportedShouldThrowEntityNotFoundException()
    {
        // Arrange
        using var handler = new JiraHttpMessageHandler(CreateJsonResponse(HttpStatusCode.OK));
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync("ftp://jira.example.com", ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>()
            .WithMessage("Jira url not found");
        handler.Requests.Should().BeEmpty();
    }

    [Fact]
    public async Task CheckCredentialsWhenResponseReturnsOkWithHtmlContentShouldThrowInvalidDataAppException()
    {
        // Arrange
        using var handler = new JiraHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("<html></html>", Encoding.UTF8, "text/html")
        });
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<InvalidDataAppException>()
            .WithMessage("Invalid Jira url");
    }

    [Fact]
    public async Task CheckCredentialsWhenResponseReturnsUnauthorizedShouldThrowJiraUnauthorizedException()
    {
        // Arrange
        using var handler = new JiraHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.Unauthorized));
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<JiraUnauthorizedException>()
            .WithMessage("Invalid PAT");
    }

    [Fact]
    public async Task CheckCredentialsWhenResponseReturnsForbiddenShouldThrowActionNotAllowedException()
    {
        // Arrange
        using var handler = new JiraHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.Forbidden));
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>()
            .WithMessage($"Access to Jira project {ProjectKey} is denied");
    }

    [Fact]
    public async Task CheckCredentialsWhenSearchReturnsBadRequestAndMyPermissionsReturnsNotFoundShouldThrowInvalidDataAppException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = CreateJsonContent(
                """{"errorMessages":["Localized or changed Jira error text"]}""")
        };
        using var handler = new JiraHttpMessageHandler(response, new HttpResponseMessage(HttpStatusCode.NotFound));
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<InvalidDataAppException>()
            .WithMessage($"Jira project {ProjectKey} not found");
    }

    [Fact]
    public async Task CheckCredentialsWhenSearchReturnsBadRequestAndBrowseProjectsPermissionIsMissingShouldThrowActionNotAllowedException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = CreateJsonContent(
                """{"errorMessages":["Localized or changed Jira error text"]}""")
        };
        var permissionsResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = CreateJsonContent(
                """{"permissions":{}}""")
        };
        using var handler = new JiraHttpMessageHandler(response, permissionsResponse);
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>()
            .WithMessage($"Access to Jira project {ProjectKey} is denied");
    }

    [Fact]
    public async Task CheckCredentialsWhenSearchReturnsBadRequestAndBrowseProjectsPermissionIsFalseShouldThrowActionNotAllowedException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = CreateJsonContent(
                """{"errorMessages":["Localized or changed Jira error text"]}""")
        };
        var permissionsResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = CreateJsonContent(
                """{"permissions":{"BROWSE_PROJECTS":{"havePermission":false}}}""")
        };
        using var handler = new JiraHttpMessageHandler(response, permissionsResponse);
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>()
            .WithMessage($"Access to Jira project {ProjectKey} is denied");
        handler.Requests.Select(x => x.RequestUri!.AbsoluteUri).Should().Equal(
            "https://jira.example.com/rest/api/2/search?jql=project=MMS",
            "https://jira.example.com/rest/api/2/mypermissions?projectKey=MMS&permissions=BROWSE_PROJECTS&os_authType=basic");
    }

    [Fact]
    public async Task CheckCredentialsWhenMyPermissionsReturnsBadRequestShouldThrowInvalidDataAppException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = CreateJsonContent(
                """{"errorMessages":["Localized or changed Jira error text"]}""")
        };
        var permissionsResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = CreateJsonContent(
                """{"errorMessages":["Another localized or changed Jira error text"]}""")
        };
        using var handler = new JiraHttpMessageHandler(response, permissionsResponse);
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<InvalidDataAppException>()
            .WithMessage($"Jira project {ProjectKey} not found");
    }

    [Fact]
    public async Task CheckCredentialsWhenMyPermissionsReturnsNotFoundShouldThrowInvalidDataAppException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = CreateJsonContent(
                """{"errorMessages":["Localized or changed Jira error text"]}""")
        };
        var permissionsResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
        using var handler = new JiraHttpMessageHandler(response, permissionsResponse);
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<InvalidDataAppException>()
            .WithMessage($"Jira project {ProjectKey} not found");
    }

    [Fact]
    public async Task CheckCredentialsWhenJiraHostCannotBeResolvedShouldThrowJiraTemporarilyUnavailableException()
    {
        // Arrange
        using var handler = new JiraHttpMessageHandler(new HttpRequestException("Name or service not known"));
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<JiraTemporarilyUnavailableException>()
            .WithMessage("Jira url not found");
    }

    [Fact]
    public async Task CheckCredentialsWhenJiraRequestTimesOutShouldThrowJiraTemporarilyUnavailableException()
    {
        // Arrange
        using var handler = new JiraHttpMessageHandler(
            new TaskCanceledException("The request was canceled due to the configured HttpClient.Timeout."));
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<JiraTemporarilyUnavailableException>()
            .WithMessage("Jira url not found");
    }

    [Fact]
    public async Task CheckCredentialsWhenSearchReturnsBadRequestButProjectAccessIsAllowedShouldThrowJiraCredentialsCheckFailedException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = CreateJsonContent("""{"errorMessages":["Error in the JQL Query"]}""")
        };
        var permissionsResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = CreateJsonContent(
                """{"permissions":{"BROWSE_PROJECTS":{"havePermission":true}}}""")
        };
        using var handler = new JiraHttpMessageHandler(response, permissionsResponse);
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<JiraCredentialsCheckFailedException>();
    }

    [Fact]
    public async Task CheckCredentialsWhenMyPermissionsHavePermissionIsNotBooleanShouldThrowJiraCredentialsCheckFailedException()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = CreateJsonContent("""{"errorMessages":["Error in the JQL Query"]}""")
        };
        var permissionsResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = CreateJsonContent(
                """{"permissions":{"BROWSE_PROJECTS":{"havePermission":"true"}}}""")
        };
        using var handler = new JiraHttpMessageHandler(response, permissionsResponse);
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<JiraCredentialsCheckFailedException>();
    }

    [Fact]
    public async Task CheckCredentialsWhenResponseReturnsRetryStatusCodeShouldThrowJiraTemporarilyUnavailableException()
    {
        // Arrange
        using var handler = new JiraHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.InternalServerError));
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<JiraTemporarilyUnavailableException>();
    }

    [Fact]
    public async Task CheckCredentialsWhenResponseReturnsUnexpectedStatusCodeShouldThrowJiraCredentialsCheckFailedException()
    {
        // Arrange
        using var handler = new JiraHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed));
        var service = CreateService(handler);

        // Act
        var action = () => service.CheckAccessAsync(JiraUrl, ProjectKey, Pat, Email, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<JiraCredentialsCheckFailedException>();
    }


    private JiraService CreateService(JiraHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler, false);
        _httpClients.Add(httpClient);

        _jiraApiClientFactoryMock
            .Setup(x => x.CreateBearer(It.IsAny<string>()))
            .Returns<string>(token => new JiraApiClient(httpClient, "Bearer", token));

        _jiraApiClientFactoryMock
            .Setup(x => x.CreateBasic(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<string, string>((email, token) =>
            {
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{token}"));
                return new JiraApiClient(httpClient, "Basic", credentials);
            });

        var serverValidator = new JiraServerAccessValidator(Mock.Of<ILogger<JiraServerAccessValidator>>());
        var cloudValidator = new JiraCloudAccessValidator();
        var worklogServerProvider = new JiraServerWorklogProvider();
        var worklogCloudProvider = new JiraCloudWorklogProvider();

        return new JiraService(
            _jiraApiClientFactoryMock.Object,
            serverValidator,
            cloudValidator,
            worklogServerProvider,
            worklogCloudProvider);
    }

    private static StringContent CreateJsonContent(string json)
    {
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static HttpResponseMessage CreateJsonResponse(HttpStatusCode statusCode)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = CreateJsonContent("{}")
        };
    }

    private sealed class JiraHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<object> _responses;
        private readonly List<HttpRequestMessage> _requests = [];

        public JiraHttpMessageHandler(params object[] responses)
        {
            _responses = new Queue<object>(responses);
        }

        public IReadOnlyCollection<HttpRequestMessage> Requests => _requests;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _requests.Add(request);

            var response = _responses.Dequeue();

            return response switch
            {
                Exception exception => Task.FromException<HttpResponseMessage>(exception),
                HttpResponseMessage httpResponse => Task.FromResult(httpResponse),
                _ => throw new InvalidOperationException("Unsupported Jira test response")
            };
        }
    }
}
