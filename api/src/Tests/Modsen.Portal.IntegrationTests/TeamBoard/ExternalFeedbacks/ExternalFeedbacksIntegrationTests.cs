using System.Net;
using System.Net.Http.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TeamBoard.ExternalFeedbacks;

[Collection("TeamBoardCollection")]
public class ExternalFeedbacksIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ExternalFeedbacksIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetExternalFeedbackByIdWhenDoesNotExistResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = int.MaxValue;

        // Act
        var response = await client.GetExternalFeedback(userId);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetExternalFeedbackByIdWhenUserIsInternResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();

        var project = await client.CreateProjectAsHttpResponse(
            new CreateProjectInput
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                ClientId = _factory.ModsenClientId!.Value,
                IsBillable = true,
                IsPublic = false,
                Color = "#ffffff"
            }).WithResponseType<ProjectOutput>();

        var externalFeedbackToCreate = new CreateExternalFeedbackInput(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: "feedback", ActionsTodo: "actionsTodo",
            Reviewer: "Reviewer Reviewer", ConversationId: null, project.Id, employee!.Id);

        await client.CreateProjectAssignment(employee!.Id, project.Id,
            new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), true, 0.3m, "TraineeL2"));

        await client.CreateExternalFeedback(externalFeedbackToCreate);

        var userToUpdate = new UpdateUserInput
        {
            Id = employee!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = employee.FirstNameRu,
            LastNameRu = employee.LastNameRu,
            PatronymicRu = employee.PatronymicRu,
            Color = "#fdfdfd",
            Email = null,
            Position = PortalRoleNames.Intern,
            Location = LocationType.Minsk,
            DepartmentId = "LD",
            CountryId = 1,
            RegisteredLocationId = employee.RegisteredLocationId
        };

        await client.UpdateUserAsync(userToUpdate);

        // Act
        var response = await client.GetExternalFeedback(employee!.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?")]
    [InlineData("?top=50")]
    [InlineData("?skip=0")]
    [InlineData("?top=0&skip=50")]
    [InlineData("?top=50&skip=-1")]
    [InlineData("?top=50&skip=15")]
    [InlineData("?top=26&skip=50")]
    [InlineData("?top=50&skip=50&filter=UserId eq rr")]
    [InlineData("?top=50&skip=50&filter=UserId eq 5&orderBy=Fake desc")]
    public async Task GetPageableExternalFeedbacksWhenRequestIsInvalidResponseWithBadRequest(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetExternalFeedback(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("?top=20&skip=0")]
    [InlineData("?top=10&skip=50")]
    [InlineData("?top=50&skip=50&orderby=Id")]
    [InlineData("?top=20&skip=0&filter=UserId eq 12")]
    [InlineData("?top=20&skip=0&orderby=Id&filter=date(ExternalFeedbackDate) ge 2020-05-10 and date(ExternalFeedbackDate) le 2023-05-10")]
    [InlineData("?top=10&skip=0&orderyby=ExternalFeedbackDate desc, Id desc&filter=UserId eq 12 and date(ExternalFeedbackDate) ge 2020-05-10 and date(ExternalFeedbackDate) le 2023-05-10")]
    public async Task GetPageableExternalFeedbacksWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetExternalFeedback(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<PageableListOfItems<OdpExternalFeedback>>(_cancellationToken);
        body.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateExternalFeedbackWhenConversationIsNullResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProjectAsHttpResponse(
            new CreateProjectInput
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                ClientId = _factory.ModsenClientId!.Value,
                IsBillable = true,
                IsPublic = false,
                Color = "#ffffff"
            }).WithResponseType<ProjectOutput>();

        var externalFeedbackToCreate = new CreateExternalFeedbackInput(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: "feedback", ActionsTodo: "actionsTodo",
            Reviewer: "Reviewer Reviewer", ConversationId: null, project.Id, userId);

        await client.CreateProjectAssignment(userId, project.Id,
            new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), true, 0.3m, "TraineeL2"));

        // Act
        var externalFeedback = await client.CreateExternalFeedback(
            externalFeedbackToCreate)
            .WithResponseType<ExternalFeedbackOutput>();

        // Assert
        externalFeedback.Should().NotBeNull();
        externalFeedback.ExternalFeedbackDate.Should().Be(externalFeedbackToCreate.ExternalFeedbackDate);
        externalFeedback.Feedback.Should().BeEquivalentTo(externalFeedbackToCreate.Feedback);
        externalFeedback.ExternalFeedbackType.Should().Be(externalFeedbackToCreate.ExternalFeedbackType);
        externalFeedback.ActionsTodo.Should().BeEquivalentTo(externalFeedbackToCreate.ActionsTodo);
        externalFeedback.Reviewer.Should().BeEquivalentTo(externalFeedbackToCreate.Reviewer);
        externalFeedback.ConversationId.Should().Be(externalFeedbackToCreate.ConversationId);
        externalFeedback.ProjectId.Should().Be(externalFeedbackToCreate.ProjectId);
        externalFeedback.UserId.Should().Be(externalFeedbackToCreate.UserId);
    }

    [Fact]
    public async Task CreateExternalFeedbackWhenUserIsInternResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.InternUserId!.Value;

        var project = await client.CreateProjectAsHttpResponse(
            new CreateProjectInput
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                ClientId = _factory.ModsenClientId!.Value,
                IsBillable = true,
                IsPublic = false,
                Color = "#ffffff"
            }).WithResponseType<ProjectOutput>();

        var externalFeedbackToCreate = new CreateExternalFeedbackInput(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: "feedback", ActionsTodo: "actionsTodo",
            Reviewer: "Reviewer Reviewer", ConversationId: null, project.Id, userId);

        await client.CreateProjectAssignment(userId, project.Id,
            new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), true, 0.3m, "TraineeL2"));

        await client.CreateExternalFeedback(externalFeedbackToCreate);

        // Act
        var response = await client.CreateExternalFeedback(externalFeedbackToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateExternalFeedbackWhenConversationIsNotNullResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProjectAsHttpResponse(
            new CreateProjectInput
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                ClientId = _factory.ModsenClientId!.Value,
                IsBillable = true,
                IsPublic = false,
                Color = "#ffffff"
            }).WithResponseType<ProjectOutput>();

        var createConversationInput = new CreateConversationInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Great,
            Theses = "theses",
            EmployeeTheses = "emplTheses",
            Agreements = "agreements",
            PerformanceReviewDate = null,
            Passed = null,
            UserPerformanceReviewId = null,
            UserId = userId,
            Files = [],
            DocumentLinks = []
        };

        var conversation = await client.CreateConversation(createConversationInput)
            .WithResponseType<ConversationOutput>();

        await client.CreateProjectAssignment(userId, project.Id,
            new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), true, 0.3m, "TraineeL2"));

        var externalFeedbackToCreate = new CreateExternalFeedbackInput(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: "feedback", ActionsTodo: "actionsTodo",
            Reviewer: "Reviewer Reviewer", conversation.Id, project.Id, userId);

        // Act
        var externalFeedback = await client.CreateExternalFeedback(externalFeedbackToCreate)
            .WithResponseType<ExternalFeedbackOutput>();

        // Assert
        externalFeedback.Should().NotBeNull();
        externalFeedback.ExternalFeedbackDate.Should().Be(externalFeedbackToCreate.ExternalFeedbackDate);
        externalFeedback.Feedback.Should().BeEquivalentTo(externalFeedbackToCreate.Feedback);
        externalFeedback.ExternalFeedbackType.Should().Be(externalFeedbackToCreate.ExternalFeedbackType);
        externalFeedback.ActionsTodo.Should().BeEquivalentTo(externalFeedbackToCreate.ActionsTodo);
        externalFeedback.Reviewer.Should().BeEquivalentTo(externalFeedbackToCreate.Reviewer);
        externalFeedback.ConversationId.Should().Be(externalFeedbackToCreate.ConversationId);
        externalFeedback.ProjectId.Should().Be(externalFeedbackToCreate.ProjectId);
        externalFeedback.UserId.Should().Be(externalFeedbackToCreate.UserId);
    }

    [Fact]
    public async Task UpdateExternalFeedbackWhenUserIsInternResponseWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();

        var userId = employee!.Id;

        var project = await client.CreateProjectAsHttpResponse(
            new CreateProjectInput
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                ClientId = _factory.ModsenClientId!.Value,
                IsBillable = true,
                IsPublic = false,
                Color = "#ffffff"
            }).WithResponseType<ProjectOutput>();

        var createConversationInput = new CreateConversationInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Great,
            Theses = "theses",
            EmployeeTheses = "employeeTheses",
            Agreements = "agreements",
            PerformanceReviewDate = null,
            Passed = null,
            UserPerformanceReviewId = null,
            UserId = userId,
            Files = [],
            DocumentLinks = []
        };

        var conversation = await client.CreateConversation(createConversationInput)
            .WithResponseType<ConversationOutput>();

        await client.CreateProjectAssignment(userId, project.Id,
            new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), true, 0.3m, "TraineeL2"));

        var externalFeedbackToCreate = new CreateExternalFeedbackInput(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: "feedback", ActionsTodo: "actionsTodo",
            Reviewer: "Reviewer Reviewer", conversation.Id, project.Id, userId);

        var externalFeedback = await client.CreateExternalFeedback(externalFeedbackToCreate)
            .WithResponseType<ExternalFeedbackOutput>();

        var userToUpdate = new UpdateUserInput
        {
            Id = employee!.Id,
            FirstName = null,
            LastName = null,
            FirstNameRu = employee.FirstNameRu,
            LastNameRu = employee.LastNameRu,
            PatronymicRu = employee.PatronymicRu,
            Color = "#fdfdfd",
            Email = null,
            Position = PortalRoleNames.Intern,
            Location = LocationType.Minsk,
            DepartmentId = "LD",
            CountryId = 1,
            RegisteredLocationId = employee.RegisteredLocationId
        };

        await client.UpdateUserAsync(userToUpdate);

        var externalFeedbackToUpdate = new UpdateExternalFeedbackInput(externalFeedback.Id,
            ExternalFeedbackType.Great, ActionsTodo: null, ConversationId: null, Reviewer: null);

        // Act
        var response = await client.UpdateExternalFeedback(externalFeedbackToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateExternalFeedbackWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProjectAsHttpResponse(
            new CreateProjectInput
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                ClientId = _factory.ModsenClientId!.Value,
                IsBillable = true,
                IsPublic = false,
                Color = "#ffffff"
            }).WithResponseType<ProjectOutput>();

        var createConversationInput = new CreateConversationInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Great,
            Theses = "theses",
            EmployeeTheses = "employeeTheses",
            Agreements = "agreements",
            PerformanceReviewDate = null,
            Passed = null,
            UserPerformanceReviewId = null,
            UserId = userId,
            Files = [],
            DocumentLinks = []
        };

        var conversation = await client.CreateConversation(createConversationInput)
            .WithResponseType<ConversationOutput>();

        await client.CreateProjectAssignment(userId, project.Id,
            new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), true, 0.3m, "TraineeL2"));

        var externalFeedbackToCreate = new CreateExternalFeedbackInput(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: "feedback", ActionsTodo: "actionsTodo",
            Reviewer: "Reviewer Reviewer", conversation.Id, project.Id, userId);

        var externalFeedback = await client.CreateExternalFeedback(externalFeedbackToCreate)
            .WithResponseType<ExternalFeedbackOutput>();

        var externalFeedbackToUpdate = new UpdateExternalFeedbackInput(externalFeedback.Id,
            ExternalFeedbackType.Great, ActionsTodo: null, ConversationId: null, Reviewer: null);

        // Act
        var updatedExternalFeedback = await client.UpdateExternalFeedback(externalFeedbackToUpdate)
            .WithResponseType<ExternalFeedbackOutput>();

        // Assert
        updatedExternalFeedback.Id.Should().Be(externalFeedback.Id);
        updatedExternalFeedback.ExternalFeedbackDate.Should().Be(externalFeedback.ExternalFeedbackDate);
        updatedExternalFeedback.Feedback.Should().BeEquivalentTo(externalFeedback.Feedback);
        updatedExternalFeedback.ProjectId.Should().Be(externalFeedback.ProjectId);
        updatedExternalFeedback.UserId.Should().Be(externalFeedback.UserId);
        updatedExternalFeedback.ActionsTodo.Should().BeEquivalentTo(externalFeedbackToUpdate.ActionsTodo);
        updatedExternalFeedback.ConversationId.Should().Be(externalFeedbackToUpdate.ConversationId);
        updatedExternalFeedback.Reviewer.Should().BeEquivalentTo(externalFeedbackToUpdate.Reviewer);
        updatedExternalFeedback.ExternalFeedbackType.Should().Be(externalFeedbackToUpdate.ExternalFeedbackType);
    }

    [Fact]
    public async Task GetExternalFeedbackByIdWhenConversationIsNotNullConversationFieldsShouldBeEqualToCreated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProjectAsHttpResponse(
            new CreateProjectInput
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                ClientId = _factory.ModsenClientId!.Value,
                IsBillable = true,
                IsPublic = false,
                Color = "#ffffff"
            }).WithResponseType<ProjectOutput>();

        var createConversationInput = new CreateConversationInput
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            ConversationType = ConversationType.Conversation,
            ConversationResult = ConversationResult.Great,
            Theses = "theses",
            EmployeeTheses = "employeeTheses",
            Agreements = "agreements",
            PerformanceReviewDate = null,
            Passed = null,
            UserPerformanceReviewId = null,
            UserId = userId,
            Files = [],
            DocumentLinks = []
        };

        var conversation = await client.CreateConversation(createConversationInput)
            .WithResponseType<ConversationOutput>();

        await client.CreateProjectAssignment(userId, project.Id,
                new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow),
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), true, 0.3m, "TraineeL2"))
            .WithResponseType<UserProjectAssignmentOutput>();

        var externalFeedbackToCreate = new CreateExternalFeedbackInput(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: "feedback", ActionsTodo: "actionsTodo",
            Reviewer: "Reviewer Reviewer", conversation.Id, project.Id, userId);

        var externalFeedback = await client.CreateExternalFeedback(externalFeedbackToCreate)
            .WithResponseType<ExternalFeedbackOutput>();

        var currentUser = await client.GetCurrentUser();

        // Act
        var result = await client.GetExternalFeedback(externalFeedback.Id)
            .WithResponseType<ExternalFeedbackWithProjectAndConversationOutput>();

        // Assert
        result.ConversationId.Should().Be(conversation.Id);
        result.ConversationDate.Should().Be(conversation.Date);
        result.ConversationSupervisorFullName.Should().BeEquivalentTo($"{currentUser.FirstName} {currentUser.LastName}");
    }

    [Fact]
    public async Task GetExternalFeedbackByIdWhenConversationIsNullConversationFieldsShouldBeNull()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId!.Value;

        var project = await client.CreateProjectAsHttpResponse(
            new CreateProjectInput
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                ClientId = _factory.ModsenClientId!.Value,
                IsBillable = true,
                IsPublic = false,
                Color = "#ffffff"
            }).WithResponseType<ProjectOutput>();

        await client.CreateProjectAssignment(userId, project.Id,
            new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), true, 0.3m, "TraineeL2"));

        var externalFeedbackToCreate = new CreateExternalFeedbackInput(DateOnly.FromDateTime(DateTime.UtcNow),
            ExternalFeedbackType.Normal, Feedback: "feedback", ActionsTodo: "actionsTodo",
            Reviewer: "Reviewer Reviewer", null, project.Id, userId);

        var externalFeedback = await client.CreateExternalFeedback(externalFeedbackToCreate)
            .WithResponseType<ExternalFeedbackOutput>();

        // Act
        var result = await client.GetExternalFeedback(externalFeedback.Id)
            .WithResponseType<ExternalFeedbackWithProjectAndConversationOutput>();

        // Assert
        result.ConversationId.Should().BeNull();
        result.ConversationDate.Should().BeNull();
        result.ConversationSupervisorFullName.Should().BeNull();
    }

    [Fact]
    public async Task GetUniqueExternalFeedbackClientsWhenMultipleShouldReturnExpectedClients()
    {
        // Arrange
        using var httpClient = _factory.CreateClient();
        await httpClient.AuthAsCeo();

        var clientNames = new[] { "Client one", "Client two", "Client three" };

        var createdClients = new List<CreateClientOutput>();

        foreach (var clientName in clientNames)
        {
            var client = await httpClient.CreateClient(
                new CreateClientInput { Name = clientName, RegisteredLocation = 1 })
                .WithResponseType<CreateClientOutput>();

            createdClients.Add(client);
        }

        var projectIds = new List<int>();

        var sameClientProject = await httpClient.CreateProjectAsHttpResponse(
            new CreateProjectInput
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                ClientId = createdClients[1].Id,
                IsBillable = true,
                IsPublic = false,
                Color = "#ffffff",
            }).WithResponseType<ProjectOutput>();

        projectIds.Add(sameClientProject.Id);

        foreach (var client in createdClients)
        {
            var project = await httpClient.CreateProjectAsHttpResponse(
            new CreateProjectInput
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                ClientId = client.Id,
                IsBillable = true,
                IsPublic = false,
                Color = "#ffffff"
            }).WithResponseType<ProjectOutput>();

            projectIds.Add(project.Id);
        }

        var userId = _factory.EmployeeUserId!.Value;

        for (var i = 0; i < projectIds.Count - 1; i++)
        {
            await httpClient.CreateProjectAssignment(userId, projectIds[i],
            new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), true, 0.3m, "TraineeL2"));
        }

        // Act
        var clients = await httpClient.GetExternalFeedbackClients(userId)
            .WithResponseType<List<string>>();

        // Assert
        clients.Where(client => client == clientNames[0]).Should().HaveCount(1);
        clients.Where(client => client == clientNames[1]).Should().HaveCount(1);
        clients.Where(client => client == clientNames[2]).Should().HaveCount(0);
    }

    [Fact]
    public async Task GetUniqueExternalFeedbackReviewersWhenMultipleAndNullableShouldReturnExpectedReviewers()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var project = await client.CreateProjectAsHttpResponse(
            new CreateProjectInput
            {
                Name = Guid.CreateVersion7().ToAlphabetic(),
                ClientId = _factory.ModsenClientId!.Value,
                IsBillable = true,
                IsPublic = false,
                Color = "#ffffff"
            })
            .WithResponseType<ProjectOutput>();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var userId = _factory.EmployeeUserId!.Value;

        await client.CreateProjectAssignment(userId, project.Id,
            new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)), true, 0.3m, "TraineeL2"));

        foreach (var reviewer in new[] { "Reviewer One", "Reviewer Two", "Reviewer Two", null })
        {
            await client.CreateExternalFeedback(new CreateExternalFeedbackInput(date,
                ExternalFeedbackType.Great, Feedback: "feedback", ActionsTodo: "actionsTodo", reviewer,
                ConversationId: null, project.Id, userId));
        }

        var expectedReviewers = new[] { "Reviewer One", "Reviewer Two" };

        // Act
        var reviewers = await client.GetExternalFeedbackReviewers(userId)
            .WithResponseType<List<string>>();

        reviewers.Where(r => r == expectedReviewers[0]).Should().HaveCount(1);
        reviewers.Where(r => r == expectedReviewers[1]).Should().HaveCount(1);
        reviewers.Should().NotContain(r => r == null);
    }
}
