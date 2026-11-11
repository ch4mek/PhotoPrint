using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.Seeds;

internal class UserContractSeed : ISeedEntitiesProvider<TeamBoardDbContext>
{
    private readonly IClockService _clockService;
    private readonly TeamBoardDbContext _dbContext;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;

    public UserContractSeed(IClockService clockService, TeamBoardDbContext dbContext,
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
            var usersWithoutContracts = _dbContext.TeamBoardUsers
                .Where(user => !user.Contracts.Any())
                .Select(user => new { user.UserId, user.CareerStartedAt, user.HireDate })
                .ToList();

            var random = Random.Shared;
            var contractsToAdd = new List<UserContract>();

            foreach (var user in usersWithoutContracts)
            {
                var date = user.HireDate ?? user.CareerStartedAt;
                var contractFrom = GenerateContractStartDate(date!.Value, random);

                var contract = new UserContract
                {
                    UserId = user.UserId,
                    ContractFrom = contractFrom,
                    IsProbationPeriod = false,
                    IsActive = true,
                    CreatedAtUtc = _clockService.UtcNow,
                };

                contractsToAdd.Add(contract);
            }

            _dbContext.UserContracts.AddRange(contractsToAdd);
            _dbContext.SaveChanges();
        }
    }

    private DateOnly GenerateContractStartDate(DateOnly minDate, Random random)
    {
        var maxDate = DateOnly.FromDateTime(_clockService.UtcNow);

        if (minDate > maxDate)
        {
            return minDate;
        }

        var range = maxDate.DayNumber - minDate.DayNumber;

        if (range <= 0)
        {
            return minDate;
        }

        return minDate.AddDays(random.Next(range + 1));
    }
}
