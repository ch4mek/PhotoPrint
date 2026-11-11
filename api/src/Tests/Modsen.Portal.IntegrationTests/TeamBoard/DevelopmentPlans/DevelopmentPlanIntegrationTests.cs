using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Input;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
namespace Modsen.Portal.IntegrationTests.TeamBoard.DevelopmentPlans;

[Collection("GeneralCollection")]
public class DevelopmentPlanIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public DevelopmentPlanIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetDevelopmentPlanByIdWhenDoesNotExistResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetDevelopmentPlans(int.MaxValue);

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
    [InlineData("?top=50&skip=50&filter=Id eq a")]
    public async Task GetPageableDevelopmentPlansWhenRequestIsInvalidResponseWithBadRequest(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetDevelopmentPlans(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("?top=20&skip=0")]
    [InlineData("?top=10&skip=50")]
    [InlineData("?top=50&skip=50&orderby=Id")]
    [InlineData("?top=20&skip=0&filter=userId eq 22")]
    [InlineData("?top=20&skip=0&filter=contains(Notes, tolower('note-part'))")]
    public async Task GetPageableDevelopmentPlansWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetDevelopmentPlans(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateDevelopmentPlanWhenDeadlineDateIsBeforeThanStartDateResponseWithBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };

        // Act
        var response = await client.CreateDevelopmentPlans(developmentPlanToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDevelopmentPlanWhenEndDateIsBeforeThanStartDateResponseWithBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };

        // Act
        var response = await client.CreateDevelopmentPlans(developmentPlanToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateDevelopmentPlanWhenUserDoesNotExistResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = int.MaxValue,
            ConversationId = null
        };

        // Act
        var response = await client.CreateDevelopmentPlans(developmentPlanToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateDevelopmentPlanWhenUserIsInternResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.InternUserId!.Value,
            ConversationId = null
        };

        // Act
        var response = await client.CreateDevelopmentPlans(developmentPlanToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateDevelopmentPlanWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };

        // Act
        var response = await client.CreateDevelopmentPlans(developmentPlanToCreate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<DevelopmentPlanOutput>(_cancellationToken);
        body.Should().NotBeNull();
        body.StartDate.Should().Be(developmentPlanToCreate.StartDate);
        body.DeadlineDate.Should().Be(developmentPlanToCreate.DeadlineDate);
        body.Notes.Should().Be(developmentPlanToCreate.Notes);
        body.Summary.Should().Be(developmentPlanToCreate.Summary);
        body.UserId.Should().Be(developmentPlanToCreate.UserId);

    }

    [Fact]
    public async Task UpdateDevelopmentPlanWhenDoesNotExistResponserWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToUpdate = new UpdateDevelopmentPlanInput
        {
            Id = int.MaxValue,
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic()
        };

        // Act
        var response = await client.UpdateDevelopmentPlans(developmentPlanToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDevelopmentPlanWhenDeadlineDateIsBeforeThanStartDateResponserWithBadRequest()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };


        var createdDevelopmentPlan = await client.CreateDevelopmentPlans(developmentPlanToCreate)
            .WithResponseType<DevelopmentPlanOutput>();

        var developmentPlanToUpdate = new UpdateDevelopmentPlanInput
        {
            Id = createdDevelopmentPlan.Id,
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic()
        };


        // Act
        var response = await client.UpdateDevelopmentPlans(developmentPlanToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateDevelopmentPlanWhenUserIsInternResponserWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = employee!.Id,
            ConversationId = null
        };


        var createdDevelopmentPlan = await client.CreateDevelopmentPlans(developmentPlanToCreate)
            .WithResponseType<DevelopmentPlanOutput>();

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

        var developmentPlanToUpdate = new UpdateDevelopmentPlanInput
        {
            Id = createdDevelopmentPlan.Id,
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic()
        };

        // Act
        var response = await client.UpdateDevelopmentPlans(developmentPlanToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateDevelopmentPlanWhenEndDateIsBeforeThanStartDateResponserWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };


        var createdDevelopmentPlan = await client.CreateDevelopmentPlans(developmentPlanToCreate)
            .WithResponseType<DevelopmentPlanOutput>();

        var developmentPlanToUpdate = new UpdateDevelopmentPlanInput
        {
            Id = createdDevelopmentPlan.Id,
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic()
        };

        // Act
        var response = await client.UpdateDevelopmentPlans(developmentPlanToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteDevelopmentPlanWhenUserIsInternResponseWithForbidden()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = employee!.Id,
            ConversationId = null
        };

        var createdDevelopmentPlan = await client.CreateDevelopmentPlans(developmentPlanToCreate)
            .WithResponseType<DevelopmentPlanOutput>();

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
        var response = await client.DeleteDevelopmentPlan(createdDevelopmentPlan.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteDevelopmentPlanWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };

        var createdDevelopmentPlan = await client.CreateDevelopmentPlans(developmentPlanToCreate)
            .WithResponseType<DevelopmentPlanOutput>();

        // Act
        var response = await client.DeleteDevelopmentPlan(createdDevelopmentPlan.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteDevelopmentPlanWhenPlanNotExistsResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.DeleteDevelopmentPlan(999);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDevelopmentPlansStatusWhenRequestIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };

        var createdDevelopmentPlan = await client.CreateDevelopmentPlans(developmentPlanToCreate)
            .WithResponseType<DevelopmentPlanOutput>();

        var developmentPlanToUpdate = new UpdateDevelopmentPlanStatusInput
        {
            DevelopmentPlanId = createdDevelopmentPlan.Id,
            ConfirmationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20)),
            DevelopmentPlanStatus = Core.TeamBoard.DevelopmentPlanStatusType.Approved
        };

        // Act
        var response = await client.UpdateDevelopmentPlanStatus(developmentPlanToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateDevelopmentPlansStatusWhenPlanNotFoundResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToUpdate = new UpdateDevelopmentPlanStatusInput
        {
            DevelopmentPlanId = 9999,
            ConfirmationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20)),
            DevelopmentPlanStatus = Core.TeamBoard.DevelopmentPlanStatusType.Approved
        };

        // Act
        var response = await client.UpdateDevelopmentPlanStatus(developmentPlanToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDevelopmentPlansStatusWhenUserIsNotAuthenticatedUserFoundResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };

        var createdDevelopmentPlan = await client.CreateDevelopmentPlans(developmentPlanToCreate)
            .WithResponseType<DevelopmentPlanOutput>();

        var developmentPlanToUpdate = new UpdateDevelopmentPlanStatusInput
        {
            DevelopmentPlanId = createdDevelopmentPlan.Id,
            DevelopmentPlanStatus = Core.TeamBoard.DevelopmentPlanStatusType.UnderReview
        };

        // Act
        var response = await client.UpdateDevelopmentPlanStatus(developmentPlanToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDevelopmentPlansStatusWhenStatusIsUnderReviewResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };

        var createdDevelopmentPlan = await client.CreateDevelopmentPlans(developmentPlanToCreate)
            .WithResponseType<DevelopmentPlanOutput>();

        var developmentPlanToUpdate = new UpdateDevelopmentPlanStatusInput
        {
            DevelopmentPlanId = createdDevelopmentPlan.Id,
            DevelopmentPlanStatus = Core.TeamBoard.DevelopmentPlanStatusType.UnderReview
        };

        await client.AuthAsEmployee();

        // Act
        var response = await client.UpdateDevelopmentPlanStatus(developmentPlanToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateDevelopmentPlansStatusWhenEmployeeApproveResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };

        var createdDevelopmentPlan = await client.CreateDevelopmentPlans(developmentPlanToCreate)
            .WithResponseType<DevelopmentPlanOutput>();

        var developmentPlanToUpdate = new UpdateDevelopmentPlanStatusInput
        {
            DevelopmentPlanId = createdDevelopmentPlan.Id,
            DevelopmentPlanStatus = Core.TeamBoard.DevelopmentPlanStatusType.Approved,
        };

        await client.AuthAsEmployee();

        // Act
        var response = await client.UpdateDevelopmentPlanStatus(developmentPlanToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDevelopmentPlansStatusWhenGmApproveUserNotFromHisGroupResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };

        var createdDevelopmentPlan = await client.CreateDevelopmentPlans(developmentPlanToCreate)
            .WithResponseType<DevelopmentPlanOutput>();

        var developmentPlanToUpdate = new UpdateDevelopmentPlanStatusInput
        {
            DevelopmentPlanId = createdDevelopmentPlan.Id,
            DevelopmentPlanStatus = Core.TeamBoard.DevelopmentPlanStatusType.Approved,
        };

        await client.AuthAsGroupManager();

        // Act
        var response = await client.UpdateDevelopmentPlanStatus(developmentPlanToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateDevelopmentPlansStatusWhenConfirmationDateLessThanStartDateResponseWithNotFound()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo(); 

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };

        var createdDevelopmentPlan = await client.CreateDevelopmentPlans(developmentPlanToCreate)
            .WithResponseType<DevelopmentPlanOutput>();

        var developmentPlanToUpdate = new UpdateDevelopmentPlanStatusInput
        {
            DevelopmentPlanId = createdDevelopmentPlan.Id,
            ConfirmationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            DevelopmentPlanStatus = Core.TeamBoard.DevelopmentPlanStatusType.Approved,
        };

        // Act
        var response = await client.UpdateDevelopmentPlanStatus(developmentPlanToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreateDevelopmentPlansWhenPlanIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };

        // Act
        var response = await client.CreateDevelopmentPlanList([developmentPlanToCreate]);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
    
    [Fact]
    public async Task UpdateDevelopmentPlansWhenPlanIsValidResponseWithOk()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var developmentPlanToCreate = new CreateDevelopmentPlanInput
        {
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(24)),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic(),
            UserId = _factory.EmployeeUserId!.Value,
            ConversationId = null
        };

        var createdDevelopmentPlan = await client.CreateDevelopmentPlans(developmentPlanToCreate)
            .WithResponseType<DevelopmentPlanOutput>();

        var developmentPlanToUpdate = new UpdateDevelopmentPlanInput
        {
            Id = createdDevelopmentPlan.Id,
            DeadlineDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Summary = Guid.CreateVersion7().ToAlphabetic(),
            Notes = Guid.CreateVersion7().ToAlphabetic()
        };

        // Act
        var response = await client.UpdateDevelopmentPlanList([developmentPlanToUpdate]);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
