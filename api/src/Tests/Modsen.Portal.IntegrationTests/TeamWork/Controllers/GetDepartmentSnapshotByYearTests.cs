using System.Globalization;
using System.Text.Json;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TimeTracker;
using Modsen.Portal.TeamWork.UseCases.Models;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.TeamWork.Controllers;

[Collection("TeamWorkCollection")]
public class GetDepartmentSnapshotByYearTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetDepartmentSnapshotByYearTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;
    }

    [Fact]
    public async Task GetDepartmentSnapshotByYearShouldReturnExpectedModel()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var employee = await client.CreateEmployee(departmentId: "Net");

        var firstProject = await client.CreateProject(
            new CreateProjectInput
            {
                ClientId = 2,
                Color = "#745632",
                IsBillable = true,
                IsPublic = false,
                Name = "Bdone"
            });

        var secondProject = await client.CreateProject(
            new CreateProjectInput
            {
                ClientId = _factory.ModsenClientId.Value,
                Color = "#745632",
                IsBillable = true,
                IsPublic = false,
                Name = "Bdtwo"
            });

        var thirdProject = await client.CreateProject(
            new CreateProjectInput
            {
                ClientId = 3,
                Color = "#745632",
                IsBillable = true,
                IsPublic = false,
                Name = "Bdthree"
            });

        var fourthProject = await client.CreateProject(
            new CreateProjectInput
            {
                ClientId = _factory.ModsenClientId.Value,
                Color = "#745632",
                IsBillable = false,
                IsPublic = false,
                Name = "Bdfour"
            });

        foreach (var project in new[] { firstProject, secondProject, thirdProject, fourthProject })
        {
            await client.CreateProjectAssignmentAsHttpMessageAsync(employee.Id, project.Id,
                DateOnly.FromDateTime(DateTime.UtcNow));
        }

        var testUser = await client.CreateEmployee(departmentId: "Java");

        foreach (var project in new[] { firstProject!, secondProject!, thirdProject!, fourthProject! })
        {
            await client.CreateProjectAssignmentAsHttpMessageAsync(testUser.Id, project.Id,
                DateOnly.FromDateTime(DateTime.UtcNow));
        }

        await client.TrackBillableAsync(
            new UpsertBillableDto
            {
                ProjectId = firstProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
                TaskName = "Task1",
                BillableTrackedInMinutes = 15,
                UserId = testUser!.Id
            });

        await client.TrackBillableAsync(
            new UpsertBillableDto
            {
                ProjectId = secondProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
                TaskName = "Task2",
                BillableTrackedInMinutes = 15,
                UserId = testUser.Id
            });

        await client.TrackBillableAsync(
            new UpsertBillableDto
            {
                ProjectId = thirdProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)),
                TaskName = "Task3",
                BillableTrackedInMinutes = 15,
                UserId = testUser.Id
            });

        await client.AuthAs(employee.Email);

        await client.TrackFactAsync(
            new UpsertFactDto
            {
                ProjectId = firstProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker1",
                FactTrackedInMinutes = 60
            });

        await client.TrackFactAsync(
            new UpsertFactDto
            {
                ProjectId = firstProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker2",
                FactTrackedInMinutes = 60
            });

        await client.TrackFactAsync(
            new UpsertFactDto
            {
                ProjectId = firstProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker3",
                FactTrackedInMinutes = 60
            });

        await client.TrackFactAsync(
            new UpsertFactDto
            {
                ProjectId = secondProject.Id,
                ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TaskName = "TimeTracker4",
                FactTrackedInMinutes = 60
            });

        await client.TrackFactAsync(new UpsertFactDto
        {
            ProjectId = secondProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker5",
            FactTrackedInMinutes = 60
        });

        await client.TrackFactAsync( new UpsertFactDto
        {
            ProjectId = secondProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker6",
            FactTrackedInMinutes = 60
        });

        await client.TrackFactAsync(new UpsertFactDto
        {
            ProjectId = thirdProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker7",
            FactTrackedInMinutes = 60
        });

        await client.TrackFactAsync(new UpsertFactDto
        {
            ProjectId = fourthProject.Id,
            ReportForDate = DateOnly.FromDateTime(DateTime.UtcNow),
            TaskName = "TimeTracker8",
            FactTrackedInMinutes = 60
        });

        await client.AuthAsCto();

        var input = new GetSnapshotByYearDto
        (
            DateTime.UtcNow.Year,
            DateTime.UtcNow.Year + 2,
            DepartmentId: null,
            [firstProject.Id, secondProject.Id, thirdProject.Id, fourthProject.Id],
            [1, 2]
        );

        var expectedResult = GetExpectedResult();

        //Act
        var response = await client.PostAsJsonAsync<GetSnapshotByYearDto, GetSnapshotByYearResponse<DepartmentChild>>("/api/TeamWork/department", input, _cancellationToken);

        //Assert
        response!.Should().BeEquivalentTo(expectedResult);
    }

    private GetSnapshotByYearResponse<DepartmentChild> GetExpectedResult()
    {
        var monthYearFormat = "MMM yyyy";
        var result = new GetSnapshotByYearResponse<DepartmentChild>
        {
            Totals =
            [
                new Total
                {
                    Amounts = [],
                    EntriesCount = 9,
                    Id = "",
                    TotalAmount = null,
                    TotalTime = 27000
                }
            ],
            GroupOne =
            [
                new Group<DepartmentChild>
                {
                    Amounts = [],
                    Duration = 25200,
                    Id = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
                        .ToString(monthYearFormat, CultureInfo.InvariantCulture),
                    Name = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)
                        .ToString(monthYearFormat, CultureInfo.InvariantCulture),
                    Children =
                    [
                        new DepartmentChild { Amounts = [], Duration = 25200, Ids = ["Net"], Name = "Net" }
                    ]
                },
                new Group<DepartmentChild>
                {
                    Amounts = [],
                    Duration = 1800,
                    Id = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(1).Month, 1)
                        .ToString(monthYearFormat, CultureInfo.InvariantCulture),
                    Name = new DateTime(DateTime.UtcNow.AddMonths(1).Year, DateTime.UtcNow.AddMonths(1).Month, 1)
                        .ToString(monthYearFormat, CultureInfo.InvariantCulture),
                    Children =
                    [
                        new DepartmentChild { Amounts = [], Duration = 1800, Ids = ["Java"], Name = "Java" }
                    ]
                }
            ]
        };

        return result;
    }
}
