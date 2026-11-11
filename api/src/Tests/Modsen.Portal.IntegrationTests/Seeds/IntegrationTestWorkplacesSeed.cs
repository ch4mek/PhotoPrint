using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Booking.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Booking.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Database;

namespace Modsen.Portal.IntegrationTests.Seeds;

internal class IntegrationTestWorkplacesSeed : ISeedEntitiesProvider<BookingDbContext>
{
    private readonly BookingDbContext _dbContext;

    public IntegrationTestWorkplacesSeed(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        CreateWorkplace(1, 6, 1205);
        CreateWorkplace(1, 6, 1206);

        // CollisionsIntegrationTests
        CreateWorkplace(1, 7, 1207);
        CreateWorkplace(1, 8, 1208);
        CreateWorkplace(1, 9, 1209);

        // GetBookingsIntegrationTests
        CreateWorkplace(1, 10, 1210);
        CreateWorkplace(1, 11, 1212);
        CreateWorkplace(2, 10, 1211);

        // CancelBookingIntegrationTests
        CreateWorkplace(1, 12, 1213);

        // CreateBookingIntegrationTests
        CreateWorkplace(1, 13, 1214);
        CreateWorkplace(2, 13, 1215);
        CreateWorkplace(3, 14, 1216);
        CreateWorkplace(4, 14, 1217);
        CreateWorkplace(5, 14, 1218);
    }

    private void CreateWorkplace(int workplaceNumber, int officeId, int id, int? workspaceId = null)
    {
        var office = _dbContext.Offices.FirstOrDefault(o => o.Id == officeId);

        if (office is null)
        {
            office = new OfficeEntity
            {
                Name = $"Test{officeId}",
                Id = officeId,
                LocationType = Core.LocationType.Tbilisi,
                CreatedAtUtc = DateTime.Now,
            };

            _dbContext.Offices.Add(office);
        }

        _dbContext.Workplaces.Add(new Workplace
        {
            Id = id,
            WorkplaceNumber = workplaceNumber,
            OfficeId = officeId,
            WorkspaceId = workspaceId,
            CreatedAtUtc = DateTime.Now,
        });

        _dbContext.SaveChanges();
    }
}
