using System.Net;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.TeamBoard.Controllers.Contracts.Output;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamBoard.UserSalary;

[Collection("GeneralCollection")]
public class GetUserSalaryDetailsByIdAsyncTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    public GetUserSalaryDetailsByIdAsyncTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetUserSalaryWhenIdIsValidWithOkAndSalaryDetailsResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();
        var userId = _factory.EmployeeUserId!.Value;

        var salaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(5));
        var salary = 100;

        var newSalaryFrom = DateOnly.FromDateTime(DateTime.Now.AddDays(6));

        var createUserSalary = new CreateSalaryDto(userId, salaryFrom, salary, null);

        var responseAfterCreating = await client.CreateUserSalary(createUserSalary)
            .WithResponseType<CreateUserSalaryOutput>();

        // Act
        var response = await client.GetUserSalaryDetailsById(responseAfterCreating.Id);
        var result = await response.WithResponseType<SalaryDetailsDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Id.Should().Be(responseAfterCreating.Id);
        result.SalaryUsd.Should().Be(salary);
    }
}
