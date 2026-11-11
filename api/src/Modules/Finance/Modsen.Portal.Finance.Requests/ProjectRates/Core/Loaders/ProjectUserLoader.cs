using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Finance.DataAccess.Npgsql;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.Loaders;

internal interface IProjectUserLoader
{
    Task<List<int>> GetBillableUserProjectOnSpecificMonthAsync(int userId, DateOnly date, CancellationToken cancellationToken);
}

internal class ProjectUserLoader : IProjectUserLoader
{
    private readonly IFinanceDbContext _context;

    public ProjectUserLoader(IFinanceDbContext context)
    {
        _context = context;
    }

    public async Task<List<int>> GetBillableUserProjectOnSpecificMonthAsync(int userId, DateOnly date, CancellationToken cancellationToken)
    {
        var firstDayInMonth = new DateOnly(date.Year, date.Month, 1);
        var lastDayInMonth = new DateOnly(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

        return await _context.FinanceProjectUsers
            .Where(pu => pu.UserId == userId
                && pu.IsTimeOnProjectBillable
                && pu.UserOnProjectFrom <= lastDayInMonth
                && (!pu.UserOnProjectTo.HasValue || pu.UserOnProjectTo >= firstDayInMonth))
            .Select(pu => pu.ProjectId)
            .ToListAsync(cancellationToken);
    }
}
