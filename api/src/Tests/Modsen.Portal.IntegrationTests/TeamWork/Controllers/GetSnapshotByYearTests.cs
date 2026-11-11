using System.Globalization;
using System.Text.Json;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.TeamWork.UseCases.Models;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamWork.Controllers;

[Collection("TeamWorkCollection")]
public class GetSnapshotByYearTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetSnapshotByYearTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetSnapshotByYearShouldReturnExpectedModel()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var firstProject = await client.PostAsJsonAsync<CreateProjectInput, ProjectOutput>(
            "api/projects",
            new CreateProjectInput
            {
                ClientId = 2,
                Color = "#745632",
                IsBillable = true,
                IsPublic = false,
                Name = Guid.CreateVersion7().ToAlphabetic()
            }, _jsonOptions,
            _cancellationToken);

        var secondProject = await client.PostAsJsonAsync<CreateProjectInput, ProjectOutput>(
            "api/projects",
            new CreateProjectInput
            {
                ClientId = _factory.ModsenClientId.Value,
                Color = "#745632",
                IsBillable = true,
                IsPublic = false,
                Name = Guid.CreateVersion7().ToAlphabetic()
            }, _jsonOptions,
            _cancellationToken);

        var thirdProject = await client.PostAsJsonAsync<CreateProjectInput, ProjectOutput>(
            "api/projects",
            new CreateProjectInput
            {
                ClientId = 3,
                Color = "#745632",
                IsBillable = true,
                IsPublic = false,
                Name = Guid.CreateVersion7().ToAlphabetic()
            },  _jsonOptions,
            _cancellationToken);

        var fourthProject = await client.PostAsJsonAsync<CreateProjectInput, ProjectOutput>(
            "api/projects",
            new CreateProjectInput
            {
                ClientId = _factory.ModsenClientId.Value,
                Color = "#745632",
                IsBillable = false,
                IsPublic = false,
                Name = Guid.CreateVersion7().ToAlphabetic()
            }, _jsonOptions,
            _cancellationToken);

        var testUser = await client.CreateUser(new CreateUserInput
            {
                FirstName = "TestUserFirstName",
                LastName = "TestUserLastName",
                FirstNameRu = "Имя",
                LastNameRu = "Фамилия",
                PatronymicRu = "Отчество",
                Color = "#ffffff",
                Email = $"{Guid.CreateVersion7().ToAlphabetic()}@modsen-software.com",
                Position = PortalRoleNames.Employee,
                Location = LocationType.Minsk,
                DepartmentId = "Java",
                CountryId = 1,
                RegisteredLocationId = PortalSharedConstants.DefaultRegisteredLocationId
            });

        foreach (var project in new[]{firstProject!, secondProject!, thirdProject!, fourthProject!})
        {
            foreach (var userId in new[] { testUser!.Id, _factory.EmployeeUserId!.Value })
            {
                await client.PostAsJsonAsync<CreateUserProjectAssignmentInput, UserProjectAssignmentOutput>(
                    $"api/users/{userId}/projects/{project.Id}",
                    new CreateUserProjectAssignmentInput(DateOnly.FromDateTime(DateTime.UtcNow), UserOnProjectTo: null,
                        project.IsBillable, Fte: 1, UserOnProjectSeniorityLevelId: "SeniorL3"),
                    _jsonOptions,
                    _cancellationToken);
            }
        }

        await client.PostAsJsonAsync<UpsertBillableDto, TimesheetCoreDto>(
            "/api/Timesheet/billable/upsert",
            new UpsertBillableDto
            {
                ProjectId = firstProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
                TaskName = "Task1",
                BillableTrackedInMinutes = 15,
                UserId = testUser!.Id
            }, _jsonOptions,
            _cancellationToken);

        await client.PostAsJsonAsync<UpsertBillableDto, TimesheetCoreDto>("/api/Timesheet/billable/upsert",
            new UpsertBillableDto
            {
                ProjectId = secondProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
                TaskName = "Task2",
                BillableTrackedInMinutes = 15,
                UserId = testUser.Id
            }, _jsonOptions,
            _cancellationToken);

        await client.PostAsJsonAsync<UpsertBillableDto, TimesheetCoreDto>("/api/Timesheet/billable/upsert",
            new UpsertBillableDto
            {
                ProjectId = thirdProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
                TaskName = "Task3",
                BillableTrackedInMinutes = 15,
                UserId = testUser.Id
            },  _jsonOptions,
            _cancellationToken);

        await client.PostAsJsonAsync<UpsertFactDto, TimesheetCoreDto>("/api/Timesheet/fact/upsert",
            new UpsertFactDto
            {
                ProjectId = fourthProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
                TaskName = "Task4",
                FactTrackedInMinutes = 15,
                UserId = testUser.Id
            },  _jsonOptions,
            _cancellationToken);

        await client.AuthAsEmployee();

        await client.PostAsJsonAsync<UpsertFactDto, TimesheetCoreDto>("/api/Timesheet/fact/upsert",
            new UpsertFactDto
            {
                ProjectId = firstProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker1",
                FactTrackedInMinutes = 60
            }, _jsonOptions,
            _cancellationToken);

        await client.PostAsJsonAsync<UpsertFactDto, TimesheetCoreDto>("/api/Timesheet/fact/upsert",
            new UpsertFactDto
            {
                ProjectId = firstProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker2",
                FactTrackedInMinutes = 60
            },  _jsonOptions,
            _cancellationToken);

        await client.PostAsJsonAsync<UpsertFactDto, TimesheetCoreDto>("/api/Timesheet/fact/upsert",
            new UpsertFactDto
            {
                ProjectId = firstProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker3",
                FactTrackedInMinutes = 60
            },  _jsonOptions,
            _cancellationToken);

        await client.PostAsJsonAsync<UpsertFactDto, TimesheetCoreDto>("/api/Timesheet/fact/upsert",
            new UpsertFactDto
            {
                ProjectId = secondProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker4",
                FactTrackedInMinutes = 60
            },  _jsonOptions,
            _cancellationToken);

        await client.PostAsJsonAsync<UpsertFactDto, TimesheetCoreDto>("/api/Timesheet/fact/upsert",
            new UpsertFactDto
            {
                ProjectId = secondProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker5",
                FactTrackedInMinutes = 60
            }, _jsonOptions,
            _cancellationToken);

        await client.PostAsJsonAsync<UpsertFactDto, TimesheetCoreDto>("/api/Timesheet/fact/upsert",
            new UpsertFactDto
            {
                ProjectId = secondProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker6",
                FactTrackedInMinutes = 60
            }, _jsonOptions,
            _cancellationToken);

        await client.PostAsJsonAsync<UpsertFactDto, TimesheetCoreDto>("/api/Timesheet/fact/upsert",
            new UpsertFactDto
            {
                ProjectId = thirdProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker7",
                FactTrackedInMinutes = 60
            }, _jsonOptions,
            _cancellationToken);

        await client.PostAsJsonAsync<UpsertFactDto, TimesheetCoreDto>("/api/Timesheet/fact/upsert",
            new UpsertFactDto
            {
                ProjectId = fourthProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker8",
                FactTrackedInMinutes = 60
            }, _jsonOptions,
            _cancellationToken);

        await client.AuthAsCto();

        var input = new GetSnapshotByYearDto(
            DateTime.UtcNow.Year,
            DateTime.UtcNow.Year + 2,
            DepartmentId: null,
            [firstProject.Id, secondProject.Id, thirdProject.Id, fourthProject.Id],
            [1, 2]);

        var expectedResult = GetExpectedResult(testUser);

        //Act
        var response = await client.PostAsJsonAsync<GetSnapshotByYearDto, GetSnapshotByYearResponse<UserChild>>(
            "/api/TeamWork/user",
            input,
            _jsonOptions,
            _cancellationToken);


        //Assert
        response!.Should().BeEquivalentTo(expectedResult);
    }

    private GetSnapshotByYearResponse<UserChild> GetExpectedResult(UserOutput createdUser)
    {
        var monthYearFormat = "MMM yyyy";
        var result = new GetSnapshotByYearResponse<UserChild>
        {
            Totals =
            [
                new Total
                {
                    Amounts = [],
                    EntriesCount = 8,
                    Id = "",
                    TotalAmount = null,
                    TotalTime = 23400
                }
            ],
            GroupOne =
            [
                new Group<UserChild>
                {
                    Amounts = [],
                    Duration = 21600,
                    Id = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
                        .ToString(monthYearFormat, CultureInfo.InvariantCulture),
                    Name = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
                        .ToString(monthYearFormat, CultureInfo.InvariantCulture),
                    Children =
                    [
                        new UserChild
                        {
                            Amounts = [],
                            Duration = 21600,
                            HasMembership = true,
                            Id = _factory.EmployeeUserId!.Value,
                            Name = "Test Employee",
                            NameLowerCase = "test employee"
                        }
                    ]
                },
                new Group<UserChild>
                {
                    Amounts = [],
                    Duration = 1800,
                    Id = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(1).Month, 1)
                        .ToString(monthYearFormat, CultureInfo.InvariantCulture),
                    Name = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(1).Month, 1)
                        .ToString(monthYearFormat, CultureInfo.InvariantCulture),
                    Children =
                    [
                        new UserChild
                        {
                            Amounts = [],
                            Duration = 1800,
                            HasMembership = true,
                            Id = createdUser.Id,
                            Name = $"{createdUser.FirstName} {createdUser.LastName}",
                            NameLowerCase = $"{createdUser.FirstName} {createdUser.LastName}".ToLower()
                        }
                    ]
                }
            ]
        };

        return result;
    }
}
