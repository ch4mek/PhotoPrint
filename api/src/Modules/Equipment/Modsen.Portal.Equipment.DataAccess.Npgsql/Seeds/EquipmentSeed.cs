using Bogus;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Equipment.DataAccess.Npgsql.Seeds;

internal class EquipmentSeed : ISeedEntitiesProvider<EquipmentDbContext>
{
    private readonly EquipmentDbContext _dbContext;
    private readonly IClockService _clock;
    private readonly IHostEnvironmentContext _hostEnvironmentContext;

    public EquipmentSeed(
        EquipmentDbContext dbContext,
        IClockService clock,
        IHostEnvironmentContext hostEnvironmentContext)
    {
        _dbContext = dbContext;
        _clock = clock;
        _hostEnvironmentContext = hostEnvironmentContext;
    }

    private int FakeEquipmentsCount =>
        _hostEnvironmentContext.IsFunctionalTestEnvironment() || _hostEnvironmentContext.IsIntegrationTestEnvironment()
        ? 3
        : 50;

    public void Seed()
    {
        if (_hostEnvironmentContext.IsDevelopmentEnvironment() ||
            _hostEnvironmentContext.IsQaEnvironment() ||
            _hostEnvironmentContext.IsIntegrationTestEnvironment() ||
            _hostEnvironmentContext.IsFunctionalTestEnvironment()
       )
        {
            var equipments = _dbContext.Set<EquipmentEntity>();
            var equipmentUsers = _dbContext.Set<EquipmentUser>();

            if (equipments.Count() < FakeEquipmentsCount)
            {
                var ceoId = GetUserIdByEmail(
                    equipmentUsers,
                    "modsen.portal.test.ceo@modsen-software.com");
                var ctoId = GetUserIdByEmail(
                    equipmentUsers,
                    "modsen.portal.test.cto@modsen-software.com");
                var headId = GetUserIdByEmail(
                    equipmentUsers,
                    "modsen.portal.test.headofdepartment@modsen-software.com");

                var approverIds = new[] { ceoId, ctoId, headId }
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .ToArray();

                var equipmentTypeIds = _dbContext
                    .EquipmentTypes
                    .Select(x => x.Id)
                    .ToArray();

                var faker = new Faker<EquipmentEntity>()
                    .RuleFor(x => x.Name, x => x.Commerce.ProductName())
                    .RuleFor(x => x.BrokenStatus, _ => EquipmentBrokenStatus.None)
                    .RuleFor(x => x.TypeId, x => x.Random.ArrayElement(equipmentTypeIds))
                    .RuleFor(x => x.Location, x => x.Random.Enum<EquipmentLocationType>())
                    .RuleFor(x => x.SerialNumber, x => x.Random.AlphaNumeric(9))
                    .RuleFor(x => x.PurchasePrice, x => decimal.Round(x.Random.Decimal(100, 1000), 2))
                    .RuleFor(x => x.PurchasePriceUsd, x => decimal.Round(x.Random.Decimal(10, 100), 2))
                    .RuleFor(x => x.PurchaseCurrency, x => x.Random.Enum<MoneyCurrencyType>())
                    .RuleFor(x => x.PurchaseDate, x => x.Date.Past(3, _clock.UtcNow).Date.ToUniversalTime())
                    .RuleFor(x => x.PurchasePlace, x => x.Address.City())
                    .RuleFor(x => x.GuaranteeDate, x => x.Date.Future(3, _clock.UtcNow).Date.ToUniversalTime())
                    .RuleFor(x => x.Characteristics, x => x.Lorem.Text())
                    .RuleFor(x => x.Comment, x => x.Lorem.Text())
                    .RuleFor(x => x.ApproverId, x =>
                        approverIds.Length == 0
                            ? null
                            : x.Random.ArrayElement(approverIds))
                    .RuleFor(x => x.CreatedById, _ => null)
                    .RuleFor(x => x.Storage, _ => LocationType.Minsk)
                    .RuleFor(x => x.JiraTask, _ => "https://modsen.atlassian.net/")
                    .RuleFor(x => x.InvoiceNumber, x => x.Finance.Iban());

                equipments.AddRange(faker.Generate(FakeEquipmentsCount));

                _dbContext.SaveChanges();
            }
        }
    }

    private static int? GetUserIdByEmail(DbSet<EquipmentUser> equipmentUsers, string emailAddress)
    {
        return equipmentUsers
            .Where(x => x.Email == emailAddress)
            .Select(x => new { x.UserId })
            .FirstOrDefault()
            ?.UserId;
    }
}
