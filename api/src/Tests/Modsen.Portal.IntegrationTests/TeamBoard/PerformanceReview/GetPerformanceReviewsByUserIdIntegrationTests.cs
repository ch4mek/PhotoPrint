using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Reviews.PerformanceReviews.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.TeamBoard.PerformanceReview;

[Collection("GeneralCollection")]
public class GetPerformanceReviewsByUserIdIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetPerformanceReviewsByUserIdIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetPerformanceReviewsByUserIdWhenUserIsInternShouldReturnUsersPerformanceReviews()
    {
        // Arrange

        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var employee = await client.CreateEmployee();

        var createPerformanceReviewInput = new CreatePerformanceReviewDto(
            employee!.Id,
            DateTime.UtcNow.Date);

        await client.CreatePerfomanceReview(createPerformanceReviewInput);

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

        var response = await client
            .GetPerfomanceReview($"/user/?top=10&skip=0&orderby=Id&overdueDaysCount=7&filter=userId eq {employee!.Id}")
            .WithResponseType<PageableListOfItems<OdpPerformanceReview>>();

        // Assert
        var performanceReview = response.Items.ToList();
        performanceReview.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetPerformanceReviewsByUserIdWhenRequestIsValidShouldReturnUsersPerformanceReviews()
    {
        // Arrange

        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userEmail = Guid.CreateVersion7().ToAlphabetic();
        var userToCreate = new CreateUserInput
        {
            FirstName = "firstnameee",
            LastName = "TestUserLastsssNameq",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{userEmail}@modsen-software.com",
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var createdUser = await client.CreateUser(userToCreate);

        var createPerformanceReviewInput = new CreatePerformanceReviewDto(
            createdUser.Id,
            DateTime.UtcNow.Date);

        await client
            .CreatePerfomanceReview(createPerformanceReviewInput);

        // Act

        var response = await client
            .GetPerfomanceReview($"/user/?top=10&skip=0&orderby=Id&overdueDaysCount=7&filter=userId eq {createdUser.Id}")
            .WithResponseType<PageableListOfItems<OdpPerformanceReview>>();

        // Assert
        var performanceReview = response.Items.ToList().First();
        performanceReview.UserId.Should().Be(createdUser.Id);
        performanceReview.PerformanceReviewDate.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public async Task GetPerformanceReviewsByUserIdWhenRequestIsInvalidShouldReturnUsersPerformanceReviews()
    {
        // Arrange

        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userEmail = Guid.CreateVersion7().ToAlphabetic();
        var userToCreate = new CreateUserInput
        {
            FirstName = "firstnameee",
            LastName = "TestUserLastsssNameq",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{userEmail}@modsen-software.com",
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var createdUser = await client.CreateUser(userToCreate);

        var createPerformanceReviewInput = new CreatePerformanceReviewDto(
            createdUser.Id,
            DateTime.UtcNow.Date);

        await client
            .CreatePerfomanceReview(createPerformanceReviewInput)
            .WithResponseType<CreatePerformanceReviewOutput>();

        // Act

        var response = await client.GetPerfomanceReview(
            $"/user/?top=10&skip=0&overdueDaysCount=7&filter=userId eq {999999}")
            .WithResponseType<PageableListOfItems<OdpPerformanceReview>>();

        // Assert
        var performanceReview = response.Items.ToList();
        performanceReview.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetUpcommingReviewLabelWhenRequestIsValidShouldReturnUpcommingPerformanceReviewDate()
    {
        // Arrange

        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var userEmail = Guid.CreateVersion7().ToAlphabetic();
        var userToCreate = new CreateUserInput
        {
            FirstName = "firstnameee",
            LastName = "TestUserLastsssNameq",
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Color = "#ffffff",
            Email = $"{userEmail}@modsen-software.com",
            Position = PortalRoleNames.Employee,
            Location = LocationType.Minsk,
            DepartmentId = "Net",
            CountryId = 1,
            RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
        };

        var createdUser = await client.CreateUser(userToCreate);

        var createPerformanceReviewInput = new CreatePerformanceReviewDto(
            createdUser.Id,
            DateTime.UtcNow.Date.AddDays(3));

        await client
            .CreatePerfomanceReview(
                createPerformanceReviewInput);

        // Act
        var response = await client.GetPersonalInfo(createdUser.Id);

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var personalInfo = JsonSerializer.Deserialize<PersonalInfoOutput>(content, options);

        // Assert

        personalInfo.Should().NotBeNull();
        personalInfo.UpcomingPerfomanceReview.Should().Be(DateTime.UtcNow.Date.AddDays(3));
        personalInfo.PerformanceReviewDateStatus.Should().Be(PerformanceReviewDateType.Imminent.GetDescription());
    }
}
