using System.Net;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.PerformanceReview;

[Collection("GeneralCollection")]
public class CreatePerformanceReviewIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public CreatePerformanceReviewIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task AddPerformanceReviewWhenAuthAsCeoOrCtoResponseWithOkAndPerformanceReviewResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.EmployeeUserId ?? 1;
        var date = DateTime.UtcNow;

        var createPerformanceReviewInput = new CreatePerformanceReviewDto(userId, date.ToUniversalTime());

        // Act
        var response = await client.CreatePerfomanceReview(createPerformanceReviewInput)
            .WithResponseType<CreatePerformanceReviewOutput>();

        // Assert
        response.UserId.Should().Be(userId);
        response.Date.Should().Be(date);
    }

    [Fact]
    public async Task AddPerformanceReviewWhenUserIsInternResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userId = _factory.InternUserId!.Value;
        var date = DateTime.UtcNow;

        var createPerformanceReviewInput = new CreatePerformanceReviewDto(userId, date.ToUniversalTime());

        // Act
        var response = await client.CreatePerfomanceReview(createPerformanceReviewInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddPerformanceReviewWhenAuthAsEmployeeOrSysAdminResponseWithForbiddenResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsEmployee();

        var userId = _factory.EmployeeUserId!.Value;
        var date = new DateTime(2024, 8, 20);

        var createPerformanceReviewInput = new CreatePerformanceReviewDto(userId, date);

        // Act
        var response = await client.CreatePerfomanceReview(createPerformanceReviewInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddUserSalaryWhenAuthAsHeadWhenPerformanceReviewExistsOnThisDateResponseWithConflictStatusCode()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsHeadOfDepartment();

        var userId = _factory.EmployeeUserId!.Value;
        var date = new DateTime(2024, 8, 20);

        var createPerformanceReviewInput = new CreatePerformanceReviewDto(userId, date.ToUniversalTime());

        // Act
        await client.CreatePerfomanceReview(createPerformanceReviewInput);
        var response = await client.CreatePerfomanceReview(createPerformanceReviewInput);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AddPerformanceReviewWhenAuthAsGroupManagerResponseWithOkAndPerformanceReviewResult()
    {
        // Arrange
        using var client = _factory.CreateClient();

        await client.AuthAsCeo();

        var user = await client.CreateUserAsHttpMessage(new Portal.Identity.Controllers.Contracts.Input.CreateUserInput
        {
           DepartmentId = "Net",
           ResourceGroupId = 1,
           Email = Guid.CreateVersion7().ToAlphabetic() + "@modsen-software.com",
           FirstName = Guid.CreateVersion7().ToAlphabetic(),
           LastName = Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
           Position = PortalRoleNames.Employee,
           RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId,
           CountryId = PortalSharedConstants.DefaultCountryId
        }).WithResponseType<UserOutput>();

        await client.AuthAsGroupManager();

        var date = DateTime.UtcNow.AddDays(1);

        var createPerformanceReviewInput = new CreatePerformanceReviewDto(user.Id, date.ToUniversalTime());

        // Act
        var response = await client.CreatePerfomanceReview(createPerformanceReviewInput)
            .WithResponseType<CreatePerformanceReviewOutput>();

        // Assert
        response.Date.Should().Be(date);
    }
}
