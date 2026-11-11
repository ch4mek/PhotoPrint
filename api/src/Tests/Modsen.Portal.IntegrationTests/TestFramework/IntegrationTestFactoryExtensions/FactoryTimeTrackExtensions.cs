using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.IntegrationTests.TestFramework.IntegrationTestFactoryExtensions;

internal static class FactoryTimeTrackExtensions
{
    public static void CleanTimesheets(this IntegrationTestFactory factory)
    {
        using var scope = factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TimeTrackDbContext>();

        dbContext.Timesheets.ExecuteDelete();

    }
}
