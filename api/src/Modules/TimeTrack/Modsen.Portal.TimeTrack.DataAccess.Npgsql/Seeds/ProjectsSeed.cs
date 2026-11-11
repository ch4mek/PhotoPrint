using System.Globalization;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Seeds;

internal class ProjectsSeed : ISeedEntitiesProvider<TimeTrackDbContext>
{
    private readonly TimeTrackDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly IHexColorGenerator _hexColorGenerator;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;
    private readonly IConfiguration _configuration;
    public ProjectsSeed(
        TimeTrackDbContext dbContext,
        IClockService clock,
        IHexColorGenerator hexColorGenerator,
        IHostEnvironmentContext hostEnvironmentContext,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _clock = clock;
        _hexColorGenerator = hexColorGenerator;
        _hostEnvironmentContext = hostEnvironmentContext;
        _configuration = configuration;
    }

    private int CountOfFakeProjects => _hostEnvironmentContext.IsFunctionalTestEnvironment() ? 3 : 100;

    public void Seed()
    {
        if (!_dbContext.Projects.Any())
        {
            var client = _dbContext.Clients.FirstOrDefault(x =>
                x.Name == _configuration.GetValue<string>("DefaultClientName"));
            var user = _dbContext.TimeTrackUsers.FirstOrDefault(x => x.Email == "support@modsen-software.com");

            _dbContext.Projects.Add(new Project
            {
                Name = PortalSharedConstants.IdleProjectName,
                ClientId = client!.Id,
                CreatedById = null,
                CreatedAtUtc = _clock.UtcNow,
                IsBillable = false,
                IsPublic = false,
                IsActive = true,
                Color = _hexColorGenerator.GenerateColor()
            });

            _dbContext.Projects.Add(new Project
            {
                Name = _configuration.GetValue<string>("DefaultProjectName") ?? "DefaultProject",
                ClientId = client.Id,
                CreatedById = null,
                CreatedAtUtc = _clock.UtcNow,
                IsBillable = false,
                IsPublic = false,
                IsActive = true,
                Color = _hexColorGenerator.GenerateColor(),
                Timesheets =
                [
                    new Timesheet
                    {
                        TaskName = "Idle",
                        CreatedById = null,
                        CreatedAtUtc = _clock.UtcNow,
                        FactTrackedInMinutes = 260,
                        BillableTrackedInMinutes = 0,
                        ReportDayOfYear = 1,
                        ReportMonthOfYear = 1,
                        ReportForDate = _clock.TodayUtc(),
                        UserId = user!.UserId
                    }
                ]
            });

            _dbContext.SaveChanges();
        }

        if (_hostEnvironmentContext.IsProductionEnvironment())
        {
            return;
        }

        if (_hostEnvironmentContext.IsDevelopmentEnvironment() ||
            _hostEnvironmentContext.IsQaEnvironment() ||
            _hostEnvironmentContext.IsFunctionalTestEnvironment())
        {
            if (_dbContext.Projects.Count() >= CountOfFakeProjects)
            {
                return;
            }

            var clientIds = _dbContext.Set<Client>().Select(x => x.Id).ToArray();
            var userIds = _dbContext.Set<TimeTrackUser>().Where(x => x.IsActive).Select(x => x.UserId).ToArray();

            var faker = new Faker();

            for (var i = 0; i < CountOfFakeProjects; i++)
            {
                var clientId = faker.Random.ArrayElement(clientIds);
                var projectName = faker.Commerce.ProductName();
                var countOfProjectUsers = 1;
                var projectsUserIds = Enumerable.Range(1, countOfProjectUsers)
                    .Select(_ => faker.Random.ArrayElement(userIds))
                    .Distinct()
                    .ToList();

                var project = new Project
                {
                    Name = projectName,
                    ClientId = clientId,
                    CreatedById = null,
                    CreatedAtUtc = _clock.UtcNow,
                    IsBillable = true,
                    IsPublic = false,
                    IsActive = true,
                    Color = _hexColorGenerator.GenerateColor(),
                    ProjectUsers = projectsUserIds.ConvertAll(userId => new ProjectUser
                    {
                        UserId = userId,
                        Comments ="auto-generated",
                        Fte = 1,
                        IsTimeOnProjectBillable = true
                    }),
                    Timesheets = []
                };

                foreach (var projectUser in project.ProjectUsers)
                {
                    var countOfTracks = 2;
                    for (var j = 0; j < countOfTracks; j++)
                    {
                        var date = _clock.UtcNow.AddDays(faker.Random.Int(-60, 0));
                        var weekOfYear = ISOWeek.GetWeekOfYear(date);

                        var trackedInMinutes = faker.Random.Int(15, 180);

                        project.Timesheets.Add(new Timesheet
                        {
                            TaskName = $"#{Guid.CreateVersion7().ToAlphabetic()}: {faker.Lorem.Sentence()}",
                            FactTrackedInMinutes = trackedInMinutes,
                            BillableTrackedInMinutes = trackedInMinutes,
                            ReportForDate = DateOnly.FromDateTime(date),
                            ReportYear = date.Year,
                            ReportMonthOfYear = date.Month,
                            ReportWeekOfYear = weekOfYear,
                            ReportDayOfYear = date.DayOfYear,
                            UserId = projectUser.UserId
                        });
                    }

                    projectUser.UserOnProjectFrom = project.Timesheets.Select(x => x.ReportForDate).Min();
                }

                _dbContext.Projects.Add(project);
                _dbContext.SaveChanges();
            }

            var employee = _dbContext.Set<TimeTrackUser>().AsNoTracking().First(x => x.Email == "modsen.portal.test.employee@modsen-software.com");

            _dbContext.Projects.Add(new Project
            {
                Name = "RequiredProject",
                ClientId = faker.Random.ArrayElement(clientIds),
                CreatedById = null,
                CreatedAtUtc = _clock.UtcNow,
                IsBillable = true,
                IsPublic = false,
                IsActive = true,
                Color = _hexColorGenerator.GenerateColor(),
                ProjectUsers = [
                    new ProjectUser
                    {
                        UserId = employee.UserId,
                        Comments ="auto-generated",
                        Fte = 1,
                        IsTimeOnProjectBillable = true,
                        UserOnProjectFrom = DateOnly.FromDateTime(_clock.UtcNow)
                    }
                ],
                Timesheets = []
            });

            _dbContext.SaveChanges();
        }
    }
}
