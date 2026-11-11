using Bogus;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.Seeds;

internal class EquipmentAssignSeed : ISeedEntitiesProvider<EquipmentDbContext>
{
    private readonly EquipmentDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;

    public EquipmentAssignSeed(
        EquipmentDbContext dbContext,
        IClockService clock,
        IHostEnvironmentContext hostEnvironmentContext)
    {
        _dbContext = dbContext;
        _clock = clock;
        _hostEnvironmentContext = hostEnvironmentContext;
    }

    private int FakeEquipmentAssignsCount => _hostEnvironmentContext.IsFunctionalTestEnvironment() || _hostEnvironmentContext.IsIntegrationTestEnvironment()
        ? 3
        : 40;

    public void Seed()
    {
        if (_hostEnvironmentContext.IsFunctionalTestEnvironment() ||
            _hostEnvironmentContext.IsDevelopmentEnvironment() ||
            _hostEnvironmentContext.IsQaEnvironment())
        {
            var equipmentAssigns = _dbContext.Set<EquipmentAssignEntity>();
            var approvers = _dbContext.EquipmentUsers
                .Where(x => x.Email == "modsen.portal.test.ceo@modsen-software.com" ||
                    x.Email == "modsen.portal.test.cto@modsen-software.com")
                .Select(x => x.UserId)
                .ToArray();

            if (equipmentAssigns.Count() < FakeEquipmentAssignsCount)
            {
                var equipments = _dbContext
                    .Set<EquipmentEntity>()
                    .OrderBy(equipment => equipment.Id)
                    .Take(FakeEquipmentAssignsCount)
                    .ToList();

                var userIds = _dbContext
                    .Set<EquipmentUser>()
                    .OrderBy(x => x.UserId)
                    .Select(x => x.UserId)
                    .Take(150)
                    .ToArray();

                var i = 0;
                var faker = new Faker<EquipmentAssignEntity>()
                    .RuleFor(x => x.ApproverId, x => x.Random.ArrayElement(approvers))
                    .RuleFor(
                        assign => assign.AssignedToUserId,
                        faker => faker.Random.ArrayElement(userIds))
                    .RuleFor(
                        assign => assign.IssueDate,
                        faker => faker.Date.Past(1, _clock.UtcNow.AddYears(-2)).Date.ToUniversalTime())
                    .RuleFor(
                        assign => assign.ReturnDate,
                        faker =>
                        {
                            i++;

                            return i % 3 == 0
                                ? faker.Date.Past(1, _clock.UtcNow).Date.ToUniversalTime()
                                : i % 5 == 0
                                    ? faker.Date.Future(1, _clock.UtcNow).Date.ToUniversalTime()
                                    : null;
                        })
                    .RuleFor(x => x.CreatedById, x => null);

                equipments.ForEach(e => e.Assigns.Add(faker.Generate()));

                _dbContext.SaveChanges();
            }
        }
    }
}
