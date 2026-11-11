using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Services;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Seeds;

internal class UserCareerStartedAtSeed : ISeedEntitiesProvider<TeamBoardDbContext>
{
    private readonly IClockService _clockService;
    private readonly TeamBoardDbContext _dbContext;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;

    public UserCareerStartedAtSeed(IClockService clockService, TeamBoardDbContext dbContext,
        IHostEnvironmentContext hostEnvironmentContext)
    {
        _clockService = clockService;
        _hostEnvironmentContext = hostEnvironmentContext;
        _dbContext = dbContext;
    }

    public void Seed()
    {
        if (_hostEnvironmentContext.IsProductionEnvironment())
        {
            return;
        }

        if (_hostEnvironmentContext.IsFunctionalTestEnvironment() ||
            _hostEnvironmentContext.IsIntegrationTestEnvironment() ||
            _hostEnvironmentContext.IsDevelopmentEnvironment() ||
            _hostEnvironmentContext.IsQaEnvironment() ||
            _hostEnvironmentContext.IsStagingEnvironment())
        {
            var random = Random.Shared;
            var startDate = new DateTime(2020, 8, 1);
            var range = (_clockService.TodayUtc().ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc) - startDate).Days;

            var usersWithoutCareerStart = _dbContext.TeamBoardUsers
                .Where(user => user.CareerStartedAt == null)
                .ToList();

            foreach (var teamBoardUser in usersWithoutCareerStart)
            {
                teamBoardUser.CareerStartedAt = DateOnly.FromDateTime(startDate.AddDays(random.Next(range)));
            }

        }

        _dbContext.SaveChanges();
    }
}
