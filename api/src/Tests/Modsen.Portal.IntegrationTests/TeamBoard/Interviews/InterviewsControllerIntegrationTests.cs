using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

namespace Modsen.Portal.IntegrationTests.TeamBoard.Interviews;

[Collection("TeamBoardCollection")]
public class InterviewsControllerIntegrationTests
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public InterviewsControllerIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
        _serializerOptions = _jsonOptions;
    }

    [Theory]
    [InlineData("top=20&skip=0")]
    [InlineData("top=10&skip=50")]
    [InlineData("top=50&skip=50&orderby=Id")]
    [InlineData("top=20&skip=0&filter=UserId eq 12")]
    [InlineData("top=20&skip=0&orderby=Id&filter=date(UserInterviewDate) ge 2020-05-10 and date(UserInterviewDate) le 2023-05-10")]
    [InlineData("top=10&skip=0&orderby=UserInterviewDate desc, Id desc&filter=UserId eq 12 and date(UserInterviewDate) ge 2020-05-10 and date(UserInterviewDate) le 2023-05-10")]
    public async Task GetPageableInterviewsWhenRequestIsValidResponseOk(string query)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.GetPageableInterviewsAsHttpResponseAsync(query, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("")]
    [InlineData("top=50")]
    [InlineData("skip=0")]
    [InlineData("top=0&skip=50")]
    [InlineData("top=50&skip=-1")]
    [InlineData("top=50&skip=15")]
    [InlineData("top=26&skip=50")]
    [InlineData("top=50&skip=50&filter=Id eq ll")]
    public async Task GetPageableInterviewsWhenRequestIsInvalidResponseBadRequest(string query)
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        // Act
        var response = await client.GetPageableInterviewsAsHttpResponseAsync(query, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateInterviewWhenUserIsInternResponseForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var input = new CreateOrUpdateInterviewInput(
            UserId: _factory.InternUserId!.Value,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: "test client",
            JiraTask: "MMS-1234",
            Type: Core.TeamBoard.UserInterviewType.Prescreen,
            Status: Core.TeamBoard.UserInterviewStatus.WaitingForInterview,
            InterviewDate: DateOnly.FromDateTime(DateTime.Today),
            InterviewTimeFromUtc: new TimeOnly(11, 40, 0),
            SaleManagerId: null,
            ReasonForRefusal: "reason for refusal",
            Notes: "notes");

        // Act
        var response = await client.CreateInterviewAsHttpResponseAsync(input, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateInterviewWhenRequestIsValidResponseOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var input = new CreateOrUpdateInterviewInput(
            UserId: 1,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: "test client",
            JiraTask: "MMS-1234",
            Type: Core.TeamBoard.UserInterviewType.Prescreen,
            Status: Core.TeamBoard.UserInterviewStatus.WaitingForInterview,
            InterviewDate: DateOnly.FromDateTime(DateTime.Today),
            InterviewTimeFromUtc: new TimeOnly(11, 40, 0),
            SaleManagerId: null,
            ReasonForRefusal: "reason for refusal",
            Notes: "notes");

        // Act
        var response = await client.CreateInterviewAsync(input, _cancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.UserId.Should().Be(input.UserId);
        response.Specialization.Should().Be(input.Specialization);
        response.ClientName.Should().Be(input.Client);
        response.JiraTask.Should().Be(input.JiraTask);
        response.InterviewType.Should().Be(input.Type);
        response.Status.Should().Be(input.Status);
        response.UserInterviewDate.Should().Be(input.InterviewDate);
        response.UserInterviewFromUtc.Should().Be(input.InterviewTimeFromUtc);
        response.SalesResponsibleUserId.Should().Be(input.SaleManagerId);
        response.ReasonForRefusal.Should().Be(input.ReasonForRefusal);
        response.Notes.Should().Be(input.Notes);
    }

    [Fact]
    public async Task CreateInterviewWithEmptyValuesWhenRequestIsValidResponseOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var input = new CreateOrUpdateInterviewInput(
            UserId: 1,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: null,
            JiraTask: null,
            Type: null,
            Status: null,
            InterviewDate: null,
            InterviewTimeFromUtc: null,
            SaleManagerId: null,
            ReasonForRefusal: null,
            Notes: null);

        // Act
        var response = await client.CreateInterviewAsync(input, _cancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.UserId.Should().Be(input.UserId);
        response.Specialization.Should().Be(input.Specialization);
        response.ClientName.Should().Be(input.Client);
        response.JiraTask.Should().Be(input.JiraTask);
        response.InterviewType.Should().Be(input.Type);
        response.Status.Should().Be(input.Status);
        response.UserInterviewDate.Should().Be(input.InterviewDate);
        response.UserInterviewFromUtc.Should().Be(input.InterviewTimeFromUtc);
        response.SalesResponsibleUserId.Should().Be(input.SaleManagerId);
        response.ReasonForRefusal.Should().Be(input.ReasonForRefusal);
        response.Notes.Should().Be(input.Notes);
    }

    [Fact]
    public async Task CreateInterviewInGroupWhenUserIsInternResponseNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var datetime = DateTime.Today.AddMinutes(700);

        var employee = await client.CreateEmployee();

        var interview = new CreateOrUpdateInterviewInput(
            UserId: employee!.Id,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: "test client",
            JiraTask: "MMS-1234",
            Type: Core.TeamBoard.UserInterviewType.Prescreen,
            Status: Core.TeamBoard.UserInterviewStatus.WaitingForInterview,
            InterviewDate: DateOnly.FromDateTime(datetime),
            InterviewTimeFromUtc: TimeOnly.FromDateTime(datetime),
            SaleManagerId: null,
            ReasonForRefusal: "reason for refusal",
            Notes: "notes");

        var stage = new CreateOrUpdateInterviewInput(
            UserId: employee!.Id,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: "test client",
            JiraTask: "MMS-1234",
            Type: Core.TeamBoard.UserInterviewType.Tech,
            Status: Core.TeamBoard.UserInterviewStatus.WaitingForResult,
            InterviewDate: DateOnly.FromDateTime(datetime),
            InterviewTimeFromUtc: TimeOnly.FromDateTime(datetime),
            SaleManagerId: null,
            ReasonForRefusal: "reason for refusal 2",
            Notes: "notes 2");

        var createdInterview = await client.CreateInterviewAsync(interview, _cancellationToken);

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
            Location = employee.Location,
            DepartmentId = "LD",
            CountryId = 1,
            RegisteredLocationId = employee.RegisteredLocationId
        };

        await client.UpdateUserAsync(userToUpdate);

        // Act
        var response = await client.CreateInterviewInGroupAsHttpResponseAsync(createdInterview!.GroupId, stage, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateInterviewInGroupWhenRequestIsValidResponseOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var datetime = DateTime.Today.AddMinutes(700);

        var interview = new CreateOrUpdateInterviewInput(
            UserId: 1,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: "test client",
            JiraTask: "MMS-1234",
            Type: Core.TeamBoard.UserInterviewType.Prescreen,
            Status: Core.TeamBoard.UserInterviewStatus.WaitingForInterview,
            InterviewDate: DateOnly.FromDateTime(datetime),
            InterviewTimeFromUtc: TimeOnly.FromDateTime(datetime),
            SaleManagerId: null,
            ReasonForRefusal: "reason for refusal",
            Notes: "notes");

        var stage = new CreateOrUpdateInterviewInput(
            UserId: 1,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: "test client",
            JiraTask: "MMS-1234",
            Type: Core.TeamBoard.UserInterviewType.Tech,
            Status: Core.TeamBoard.UserInterviewStatus.WaitingForResult,
            InterviewDate: DateOnly.FromDateTime(datetime),
            InterviewTimeFromUtc: TimeOnly.FromDateTime(datetime),
            SaleManagerId: null,
            ReasonForRefusal: "reason for refusal 2",
            Notes: "notes 2");

        var createdInterview = await client.CreateInterviewAsync(interview, _cancellationToken);

        // Act
        var response = await client.CreateInterviewInGroupAsync(createdInterview.GroupId, stage, _cancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.UserId.Should().Be(stage.UserId);
        response.Specialization.Should().Be(stage.Specialization);
        response.ClientName.Should().Be(stage.Client);
        response.JiraTask.Should().Be(stage.JiraTask);
        response.InterviewType.Should().Be(stage.Type);
        response.Status.Should().Be(stage.Status);
        response.UserInterviewDate.Should().Be(stage.InterviewDate);
        response.UserInterviewFromUtc.Should().Be(stage.InterviewTimeFromUtc);
        response.SalesResponsibleUserId.Should().Be(stage.SaleManagerId);
        response.ReasonForRefusal.Should().Be(stage.ReasonForRefusal);
        response.Notes.Should().Be(stage.Notes);
    }

    [Fact]
    public async Task UpdateInterviewWhenUserIsInternResponseForbidden()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var datetime = DateTime.Today.AddMinutes(700);

        var interview = new CreateOrUpdateInterviewInput(
            UserId: 1,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: "test client",
            JiraTask: "MMS-1234",
            Type: Core.TeamBoard.UserInterviewType.Prescreen,
            Status: Core.TeamBoard.UserInterviewStatus.WaitingForInterview,
            InterviewDate: DateOnly.FromDateTime(datetime),
            InterviewTimeFromUtc: TimeOnly.FromDateTime(datetime),
            SaleManagerId: null,
            ReasonForRefusal: "reason for refusal",
            Notes: "notes");

        var input = new CreateOrUpdateInterviewInput(
            UserId: _factory.InternUserId!.Value,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: "test client2",
            JiraTask: "MMS-12345",
            Type: Core.TeamBoard.UserInterviewType.Tech,
            Status: Core.TeamBoard.UserInterviewStatus.WaitingForResult,
            InterviewDate: DateOnly.FromDateTime(datetime),
            InterviewTimeFromUtc: TimeOnly.FromDateTime(datetime),
            SaleManagerId: null,
            ReasonForRefusal: "reason for refusal 2",
            Notes: "notes 2");

        var createdInterview = await client.CreateInterviewAsync(interview, _cancellationToken);

        // Act
        var response = await client.UpdateInterviewAsHttpResponseAsync(createdInterview!.Id, input, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateInterviewWhenRequestIsValidResponseOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var datetime = DateTime.Today.AddMinutes(700);

        var interview = new CreateOrUpdateInterviewInput(
            UserId: 1,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: "test client",
            JiraTask: "MMS-1234",
            Type: Core.TeamBoard.UserInterviewType.Prescreen,
            Status: Core.TeamBoard.UserInterviewStatus.WaitingForInterview,
            InterviewDate: DateOnly.FromDateTime(datetime),
            InterviewTimeFromUtc: TimeOnly.FromDateTime(datetime),
            SaleManagerId: null,
            ReasonForRefusal: "reason for refusal",
            Notes: "notes");

        var input = new CreateOrUpdateInterviewInput(
            UserId: 2,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: "test client2",
            JiraTask: "MMS-12345",
            Type: Core.TeamBoard.UserInterviewType.Tech,
            Status: Core.TeamBoard.UserInterviewStatus.WaitingForResult,
            InterviewDate: DateOnly.FromDateTime(datetime),
            InterviewTimeFromUtc: TimeOnly.FromDateTime(datetime),
            SaleManagerId: null,
            ReasonForRefusal: "reason for refusal 2",
            Notes: "notes 2");

        var createdInterview = await client.CreateInterviewAsync(interview, _cancellationToken);

        // Act
        var response = await client.UpdateInterviewAsync(createdInterview!.Id, input, _cancellationToken);

        // Assert
        response.Should().NotBeNull();
        response.UserId.Should().Be(input.UserId);
        response.Specialization.Should().Be(input.Specialization);
        response.ClientName.Should().Be(input.Client);
        response.JiraTask.Should().Be(input.JiraTask);
        response.InterviewType.Should().Be(input.Type);
        response.Status.Should().Be(input.Status);
        response.UserInterviewDate.Should().Be(input.InterviewDate);
        response.UserInterviewFromUtc.Should().Be(input.InterviewTimeFromUtc);
        response.SalesResponsibleUserId.Should().Be(input.SaleManagerId);
        response.ReasonForRefusal.Should().Be(input.ReasonForRefusal);
        response.Notes.Should().Be(input.Notes);
    }

    [Fact]
    public async Task DeleteInterviewWhenUserIsInternResponseNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var datetime = DateTime.Today.AddMinutes(700);

        var employee = await client.CreateEmployee();

        var interview = new CreateOrUpdateInterviewInput(
            UserId: employee!.Id,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: "test client",
            JiraTask: "MMS-1234",
            Type: Core.TeamBoard.UserInterviewType.Prescreen,
            Status: Core.TeamBoard.UserInterviewStatus.WaitingForInterview,
            InterviewDate: DateOnly.FromDateTime(datetime),
            InterviewTimeFromUtc: TimeOnly.FromDateTime(datetime),
            SaleManagerId: null,
            ReasonForRefusal: "reason for refusal",
            Notes: "notes");

        var createdInterview = await client.CreateInterviewAsync(interview, _cancellationToken);

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
            Location = employee.Location,
            DepartmentId = "LD",
            CountryId = 1,
            RegisteredLocationId = employee.RegisteredLocationId
        };

        await client.UpdateUserAsync(userToUpdate);

        // Act
        var response = await client.DeleteInterviewAsync(createdInterview!.Id, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteInterviewWhenRequestIsValidResponseOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        await client.AuthAsCto();

        var datetime = DateTime.Today.AddMinutes(700);

        var interview = new CreateOrUpdateInterviewInput(
            UserId: 1,
            Specialization: Guid.CreateVersion7().ToString(),
            Client: "test client",
            JiraTask: "MMS-1234",
            Type: Core.TeamBoard.UserInterviewType.Prescreen,
            Status: Core.TeamBoard.UserInterviewStatus.WaitingForInterview,
            InterviewDate: DateOnly.FromDateTime(datetime),
            InterviewTimeFromUtc: TimeOnly.FromDateTime(datetime),
            SaleManagerId: null,
            ReasonForRefusal: "reason for refusal",
            Notes: "notes");

        var createdInterview = await client.CreateInterviewAsync(interview, _cancellationToken);

        // Act
        var response = await client.DeleteInterviewAsync(createdInterview!.Id, _cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
