using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.Conversations;

[Collection("GeneralCollection")]
public class ConversationIntegrationTests
{
    private readonly IntegrationTestFactory _testFactory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ConversationIntegrationTests(IntegrationTestFactory testFactory)
    {
        _testFactory = testFactory;
        _jsonOptions = testFactory.JsonOptions;
    }

    [Theory]
    [InlineData("top=20&skip=0")]
    [InlineData("top=20&skip=0&orderBy=date desc")]
    [InlineData("top=20&skip=0&filter=contains(tolower(SupervisorFullName), tolower('CEO Test'))")]
    [InlineData("top=1&skip=0&filter=id eq 1")]
    public async Task GetPageableConversationsWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        var client = _testFactory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetConversations(query);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpConversation>>(
            _jsonOptions, _cancellationToken);
        body.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPageabeConversationsWhenAuthedAsEmployeeResponseWithForbidden()
    {
        // Arrange
        var client = _testFactory.CreateClient();
        await client.AuthAsEmployee();

        // Act
        var response = await client.GetConversations("top=20&skip=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateConversationWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        var client = _testFactory.CreateClient();
        await client.AuthAsCeo();

        var conversationToCreate = new CreateConversationInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Bad,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = null,
            Passed = null,
            UserPerformanceReviewId = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = [],
            DevelopmentPlans =
            [
                new DevelopmentPlanFullInput
                {
                    Id = 1,
                    StartDate = new DateOnly(2025, 10, 1),
                    DeadlineDate = new DateOnly(2025, 11, 1),
                    EndDate = new DateOnly(2025, 12, 1),
                    Summary = "Summary of the development plan",
                    Notes = "Additional notes",
                    UserId = _testFactory.EmployeeUserId!.Value,
                }
            ],
            Salaries =
            [
                new UserSalaryFullCoreDto
                {
                    SalaryUsd = 100,
                    SalaryFrom = new DateOnly(2025,01,01),
                    UserId = _testFactory.EmployeeUserId!.Value,
                }
            ]
        };

        // Act
        var response = await client.CreateConversation(conversationToCreate);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadFromJsonAsync<ConversationOutput>(_cancellationToken);
        body!.SupervisorId.Should().Be(_testFactory.CeoUserId!.Value);
        body.ConversationType.Should().Be(ConversationType.Conversation.GetDescription());
        body.Date.Should().Be(conversationToCreate.Date);
        body.Theses.Should().Be(conversationToCreate.Theses);
        body.EmployeeTheses.Should().Be(conversationToCreate.EmployeeTheses);
        body.Agreements.Should().Be(conversationToCreate.Agreements);
        body.UserId.Should().Be(_testFactory.EmployeeUserId!.Value);
    }

    [Fact]
    public async Task CreatePerformanceReviewWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        var client = _testFactory.CreateClient();
        await client.AuthAsCeo();

        var conversationToCreate = new CreateConversationInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType = ConversationType.PerformanceReview,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = DateTime.UtcNow.AddDays(2),
            Passed = true,
            UserPerformanceReviewId = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = []
        };

        // Act
        var response = await client.CreateConversation(conversationToCreate);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadFromJsonAsync<ConversationOutput>(_cancellationToken);
        body!.SupervisorId.Should().Be(_testFactory.CeoUserId!.Value);
        body.ConversationType.Should().Be(ConversationType.PerformanceReview.GetDescription());
        body.Date.Should().Be(conversationToCreate.Date);
        body.Theses.Should().Be(conversationToCreate.Theses);
        body.EmployeeTheses.Should().Be(conversationToCreate.EmployeeTheses);
        body.Agreements.Should().Be(conversationToCreate.Agreements);
        body.UserId.Should().Be(_testFactory.EmployeeUserId!.Value);
    }

    [Fact]
    public async Task CreateHrConversationWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        var client = _testFactory.CreateClient();
        await client.AuthAsCeo();

        var conversationToCreate = new CreateConversationInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType = ConversationType.HRConversation,
            ConversationResult = ConversationResult.Great,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = null,
            Passed = null,
            UserPerformanceReviewId = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = [],
            DevelopmentPlans = [],
            Salaries = []
        };

        // Act
        var response = await client.CreateConversation(conversationToCreate);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadFromJsonAsync<ConversationOutput>(_cancellationToken);
        body!.SupervisorId.Should().Be(_testFactory.CeoUserId!.Value);
        body.ConversationType.Should().Be(ConversationType.HRConversation.GetDescription());
        body.Date.Should().Be(conversationToCreate.Date);
        body.Theses.Should().Be(conversationToCreate.Theses);
        body.EmployeeTheses.Should().Be(conversationToCreate.EmployeeTheses);
        body.Agreements.Should().Be(conversationToCreate.Agreements);
        body.UserId.Should().Be(_testFactory.EmployeeUserId!.Value);
    }

    [Fact]
    public async Task GetPageableConversationsWithFilterByIdShouldReturnCreatedConversation()
    {
        // Arrange
        var client = _testFactory.CreateClient();
        await client.AuthAsCeo();

        var conversationToCreate = new CreateConversationInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Great,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = null,
            Passed = null,
            UserPerformanceReviewId = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = []
        };

        var createdConversation = await client.CreateConversation(conversationToCreate)
            .WithResponseType<ConversationOutput>();

        // Act
        var response = await client.GetConversations($"top=1&skip=0&filter=id eq {createdConversation.Id}");

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpConversation>>(
            _jsonOptions, _cancellationToken);
        body!.Total.Should().Be(1);
        body.Page.Should().Be(1);
        body.PageSize.Should().Be(1);

        var item = body.Items.First();

        item.SupervisorId.Should().Be(_testFactory.CeoUserId!.Value);
        item.Date.Should().Be(conversationToCreate.Date);
        item.Theses.Should().Be(conversationToCreate.Theses);
        item.EmployeeTheses.Should().Be(conversationToCreate.EmployeeTheses);
        item.Agreements.Should().Be(conversationToCreate.Agreements);
        item.UserId.Should().Be(_testFactory.EmployeeUserId!.Value);
    }

    [Fact]
    public async Task UpdateConversationWhenConversationDoesNotExistResponseWithNotFound()
    {
        // Arrange
        var client = _testFactory.CreateClient();
        await client.AuthAsCeo();

        var conversationToUpdate = new UpdateConversationInput
        {
            Id = int.MaxValue,
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Normal,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = null,
            Passed = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = []
        };

        // Act
        var response = await client.UpdateConversation(conversationToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdatePerformanceReviewWhenPerformanceReviewDoesNotExistResponseWithNotFound()
    {
        // Arrange
        var client = _testFactory.CreateClient();
        await client.AuthAsCeo();

        var conversationToUpdate = new UpdateConversationInput
        {
            Id = int.MaxValue,
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Normal,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = null,
            Passed = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = []
        };

        // Act
        var response = await client.UpdateConversation(conversationToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateConversationWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        var client = _testFactory.CreateClient();
        await client.AuthAsCeo();

        var conversationToCreate = new CreateConversationInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Great,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = null,
            Passed = null,
            UserPerformanceReviewId = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = []
        };

        var conversation = await client.CreateConversation(conversationToCreate)
            .WithResponseType<ConversationOutput>();

        var conversationToUpdate = new UpdateConversationInput
        {
            Id = conversation.Id,
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Normal,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = null,
            Passed = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = []
        };

        // Act
        var response = await client.UpdateConversation(conversationToUpdate);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadFromJsonAsync<ConversationOutput>(_cancellationToken);
        body!.SupervisorId.Should().Be(_testFactory.CeoUserId!.Value);
        body.Theses.Should().Be(conversationToUpdate.Theses);
        body.EmployeeTheses.Should().Be(conversationToUpdate.EmployeeTheses);
        body.Agreements.Should().Be(conversationToUpdate.Agreements);
        body.UserId.Should().Be(_testFactory.EmployeeUserId!.Value);
    }

    [Fact]
    public async Task UpdatePerformanceReviewWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        var client = _testFactory.CreateClient();
        await client.AuthAsCeo();

        var conversationToCreate = new CreateConversationInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Normal,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = null,
            Passed = null,
            UserPerformanceReviewId = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = []
        };

        var conversation = await client.CreateConversation(conversationToCreate)
            .WithResponseType<ConversationOutput>();

        var conversationToUpdate = new UpdateConversationInput
        {
            Id = conversation.Id,
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Great,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = null,
            Passed = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = []
        };

        // Act
        var response = await client.UpdateConversation(conversationToUpdate);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadFromJsonAsync<ConversationOutput>(_cancellationToken);
        body!.SupervisorId.Should().Be(_testFactory.CeoUserId!.Value);
        body.ConversationType.Should().Be(ConversationType.Conversation.GetDescription());
        body.Theses.Should().Be(conversationToUpdate.Theses);
        body.EmployeeTheses.Should().Be(conversationToUpdate.EmployeeTheses);
        body.Agreements.Should().Be(conversationToUpdate.Agreements);
        body.UserId.Should().Be(_testFactory.EmployeeUserId!.Value);
    }

    [Fact]
    public async Task UpdatePerformanceReviewWhenRequestIsInvalidResponseWithConflict()
    {
        // Arrange
        // Arrange
        var client = _testFactory.CreateClient();
        await client.AuthAsCeo();

        var conversationToCreate = new CreateConversationInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType = ConversationType.PerformanceReview,
            ConversationResult = null,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = DateTime.UtcNow.AddDays(3),
            Passed = true,
            UserPerformanceReviewId = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = []
        };

        // Act
        var conversation = await client.CreateConversation(conversationToCreate)
            .WithResponseType<ConversationOutput>();

        var conversationToUpdate = new UpdateConversationInput
        {
            Id = conversation.Id,
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Great,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = null,
            Passed = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = []
        };

        // Act
        var response = await client.UpdateConversation(conversationToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetSupervisorsByUserIdWhenRequestValidResponseWithOk()
    {
        // Arrange
        // Arrange
        var client = _testFactory.CreateClient();
        await client.AuthAsCeo();

        var conversationToCreate = new CreateConversationInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType = ConversationType.PerformanceReview,
            Theses = Guid.CreateVersion7().ToAlphabetic(),
            EmployeeTheses = Guid.CreateVersion7().ToAlphabetic(),
            Agreements = Guid.CreateVersion7().ToAlphabetic(),
            PerformanceReviewDate = DateTime.UtcNow.AddDays(3),
            Passed = true,
            UserPerformanceReviewId = null,
            UserId = _testFactory.EmployeeUserId!.Value,
            Files = [],
            DocumentLinks = []
        };

        // Act
        var conversation = await client.CreateConversation(conversationToCreate)
            .WithResponseType<ConversationOutput>();

        // Act
        var response = await client.GetSupervisorsByUserId(_testFactory.EmployeeUserId!.Value);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
