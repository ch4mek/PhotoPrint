using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Core;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.IntegrationTests;

public class FunctionalTestFactory : BaseTestFactory
{
    public int? CeoUserId { get; private set; }
    public int? CtoUserId { get; private set; }
    public int? EmployeeUserId { get; private set; }
    public int MinskOfficeId { get; private set; }
    public int WarsawOfficeId { get; private set; }
    public int? ModsenClientId { get; private set; }
    public int WorkplaceIdInMinskOffice { get; private set; }
    public int WorkplaceIdInWarsawOffice { get; private set; }

    protected override string GetEnvironmentName()
    {
        return PortalEnvironmentNames.FunctionalTest;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityModuleDbContext>();
        var bookingDbContext = scope.ServiceProvider.GetRequiredService<IBookingDbContext>();
        var timeTrackDbContext = scope.ServiceProvider.GetRequiredService<ITimeTrackDbContext>();

        CeoUserId = dbContext.Users.FirstOrDefault(x => x.Email == EmailConstants.CeoEmail)?.Id;
        CtoUserId = dbContext.Users.FirstOrDefault(x => x.Email == EmailConstants.CtoEmail)?.Id;
        EmployeeUserId = dbContext.Users.FirstOrDefault(x => x.Email == EmailConstants.EmployeeEmail)?.Id;
        MinskOfficeId = bookingDbContext.Offices.First(x => x.Name == "Minsk").Id;
        WarsawOfficeId = bookingDbContext.Offices.First(x => x.Name == "Warsaw").Id;
        WorkplaceIdInMinskOffice = bookingDbContext.Offices
            .Where(x => x.Name == "Minsk")
            .SelectMany(x => x.Workplaces.Select(f => f.Id))
            .First();
        WorkplaceIdInWarsawOffice = bookingDbContext.Offices
            .Where(x => x.Name == "Warsaw")
            .SelectMany(x => x.Workplaces.Select(f => f.Id))
            .First();

        var modsenClient = timeTrackDbContext.Clients.FirstOrDefault(x => x.Name == "Modsen");

        ModsenClientId = modsenClient?.Id;

        return host;
    }
}
