using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;

namespace Modsen.Portal.IntegrationTests.Finance;

internal class FinanceTestDataGenerator
{
    private readonly IntegrationTestFactory _factory;

    public FinanceTestDataGenerator(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    public async Task<(Portal.Identity.Controllers.Contracts.Output.UserOutput? User,
        TimeTrack.Controllers.Contracts.Output.ProjectOutput? Project)> GetUserAndProjectAndCreateProjectAssignmentAsync(
        HttpClient client, DateOnly date, string? userLastName = null, int? clientId = null)
    {
        var user = await client.CreateUser(GetCreateUserInput(lastName: userLastName));
        var project = await CreateProjectOutputAsync(client, clientId: clientId);

        _ = await client.CreateProjectAssignment(user!.Id, project!.Id, date);

        return (user, project);
    }

    public CreateUserInput GetCreateUserInput(string departmentId = "Net", bool useResourceGroupId = true, string? lastName = null)
    {
        return new CreateUserInput
        {
            Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
            FirstName = Guid.CreateVersion7().ToAlphabetic(),
            LastName = lastName ?? Guid.CreateVersion7().ToAlphabetic(),
            FirstNameRu = "Имя",
            LastNameRu = "Фамилия",
            PatronymicRu = "Отчество",
            Location = LocationType.Minsk,
            Position = PortalRoleNames.Employee,
            DepartmentId = departmentId,
            ResourceGroupId = useResourceGroupId ? _factory.ResourceGroupId : null,
            Color = "#123412",
            CountryId = 1,
            RegisteredLocationId = 1,
            ContractDateFrom = DateOnly.FromDateTime(DateTime.UtcNow),
            ContractDateTo = DateOnly.FromDateTime(DateTime.UtcNow).AddYears(1),
        };
    }

    public async Task<TimeTrack.Controllers.Contracts.Output.ProjectOutput?> CreateProjectOutputAsync(
        HttpClient client, string? name = null, int? clientId = null)
    {
        return await client.CreateProject(
            clientId: clientId ?? _factory.ModsenClientId!.Value,
            isPublic: false,
            name: name ?? Guid.CreateVersion7().ToAlphabetic(),
            isBillable: true);
    }

    public static UpsertFactDto GetUpsertFactDto(string taskName, DateOnly date, int projectId, int userId, int factTracked = 480)
    {
        return new UpsertFactDto
        {
            TaskName = taskName,
            ReportForDate = date,
            ProjectId = projectId,
            UserId = userId,
            FactTrackedInMinutes = factTracked
        };
    }
}
