using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modsen.Portal.Booking.DataAccess.Npgsql;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.IntegrationTests;

public class IntegrationTestFactory : BaseTestFactory
{
    public CancellationToken CancellationToken { get; private set; }
    public int? CeoUserId { get; private set; }
    public int? CtoUserId { get; private set; }
    public int? HeadOfDepartmentUserId { get; private set; }
    public int? PmoHeadOfDepartmentUserId { get; private set; }
    public string? PmoHeadOfDepartmentEmail { get; private set; }
    public int? SysAdminHeadId { get; private set; }
    public int? EmployeeUserId { get; private set; }
    public int? InternUserId { get; private set; }
    public int? AdminUserId { get; private set; }
    public int? SystemUserId { get; private set; }
    public int? GroupManagerId { get; private set; }

    public int? EmployeeRoleId { get; private set; }

    public int MinksOfficeId { get; private set; }
    public int WarsawOfficeId { get; private set; }

    public int WorkplaceIdInMinksOffice { get; private set; }

    public int WorkplaceIdInWarsawOffice { get; private set; }

    public int? ModsenClientId { get; private set; }

    public int? IdleProjectId { get; private set; }
    public int? ModsenProjectId { get; private set; }

    public int ResourceGroupId { get; private set; }

    protected override string GetEnvironmentName()
    {
        return PortalEnvironmentNames.IntegrationTest;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();

        var identityDbContext = scope.ServiceProvider.GetRequiredService<IdentityModuleDbContext>();
        var bookingDbContext = scope.ServiceProvider.GetRequiredService<IBookingDbContext>();
        var timeTrackDbContext = scope.ServiceProvider.GetRequiredService<ITimeTrackDbContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        CancellationToken = CancellationToken.None;

        CeoUserId = identityDbContext.Users.FirstOrDefault(x => x.Email == EmailConstants.CeoEmail)?.Id;
        CtoUserId = identityDbContext.Users.FirstOrDefault(x => x.Email == EmailConstants.CtoEmail)?.Id;
        HeadOfDepartmentUserId = identityDbContext.Users.FirstOrDefault(x => x.Email == EmailConstants.HeadEmail)?.Id;
        SysAdminHeadId = identityDbContext.Users.FirstOrDefault(x => x.Email == EmailConstants.SysAdminHead)?.Id;
        EmployeeUserId = identityDbContext.Users.FirstOrDefault(x => x.Email == EmailConstants.EmployeeEmail)?.Id;
        InternUserId = identityDbContext.Users.FirstOrDefault(x => x.Email == EmailConstants.InternEmail)?.Id;
        AdminUserId = identityDbContext.Users.FirstOrDefault(x => x.Email == EmailConstants.AdminEmail)?.Id;
        SystemUserId = identityDbContext.Users.FirstOrDefault(x => x.Email == EmailConstants.SystemUserEmail)?.Id;
        GroupManagerId = identityDbContext.Users.FirstOrDefault(x => x.Email == EmailConstants.GroupManagerEmail)?.Id;

        var pmoHead = identityDbContext.Users.FirstOrDefault(x =>
                x.DepartmentId == "PMO" &&
                x.UserRoles.FirstOrDefault()!.Role!.Name == PortalRoleNames.HeadOfDepartment);

        PmoHeadOfDepartmentUserId = pmoHead?.Id;
        PmoHeadOfDepartmentEmail = pmoHead?.Email;

        ModsenClientId = timeTrackDbContext.Clients.FirstOrDefault(x =>
            x.Name == configuration.GetValue<string>("DefaultClientName"))?.Id;

        IdleProjectId = timeTrackDbContext.Projects.FirstOrDefault(x => x.Name == PortalSharedConstants.IdleProjectName)?.Id;
        ModsenProjectId = timeTrackDbContext.Projects.FirstOrDefault(x =>
            x.Name == configuration.GetValue<string>("DefaultProjectName"))?.Id;

        MinksOfficeId = bookingDbContext.Offices.First(x => x.Name == "Minsk").Id;
        WarsawOfficeId = bookingDbContext.Offices.First(x => x.Name == "Warsaw").Id;

        ResourceGroupId = identityDbContext.ResourceGroups.FirstOrDefault(x => x.Name == ".NET Minsk - Group 1")!.Id;

        WorkplaceIdInMinksOffice = bookingDbContext.Offices
            .Where(x => x.Name == "Minsk")
            .SelectMany(x => x.Workplaces.Select(f => f.Id))
            .First();

        WorkplaceIdInWarsawOffice = bookingDbContext.Offices
            .Where(x => x.Name == "Warsaw")
            .SelectMany(x => x.Workplaces.Select(f => f.Id))
            .First();

        EmployeeRoleId = identityDbContext.Roles.FirstOrDefault(x => x.Name == PortalRoleNames.Employee).Id;

        return host;
    }
}
