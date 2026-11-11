using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Encryptor;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Loaders;

internal interface IUserSalaryLoader
{
    Task<UserSalaryCoreDto?> LoadAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<UserSalaryCoreDto>> LoadAsync(UsersSalaryByMonthLoaderRequest request, CancellationToken cancellationToken);
}

internal class UserSalaryLoader : IUserSalaryLoader
{
    private readonly ITeamBoardDbContext _dbContext;
    private readonly ISalaryEncryptor _salaryEncryptor;

    public UserSalaryLoader(ITeamBoardDbContext dbContext, ISalaryEncryptor salaryEncryptor)
    {
        _dbContext = dbContext;
        _salaryEncryptor = salaryEncryptor;
    }

    public async Task<UserSalaryCoreDto?> LoadAsync(int id, CancellationToken cancellationToken)
    {
        var userSalary = await _dbContext.UserSalaries
            .FirstOrDefaultAsync(salary => salary.Id == id, cancellationToken);

        return userSalary is null
            ? null
            : new UserSalaryCoreDto(
                userSalary.Id,
                userSalary.UserId,
                userSalary.SalaryFrom,
                SalaryUsd: _salaryEncryptor.Decrypt(userSalary.SalaryUsd),
                userSalary.CreatedById,
                userSalary.CreatedAtUtc);
    }

    public async Task<IReadOnlyCollection<UserSalaryCoreDto>> LoadAsync(UsersSalaryByMonthLoaderRequest request, CancellationToken cancellationToken)
    {
        if (request.UserIds.Length == 0)
        {
            return [];
        }

        var lastDay = DateTime.DaysInMonth(request.Year, request.Month);
        var month = new DateOnly(request.Year, request.Month, lastDay);

        var usersSalary = await _dbContext.UserSalaries
            .AsNoTracking()
            .Where(salary => request.UserIds.Contains(salary.UserId))
            .Where(salary => salary.SalaryFrom <= month)
            .GroupBy(salary => salary.UserId)
            .Select(x => x.OrderByDescending(s => s.SalaryFrom).First())
            .ToListAsync(cancellationToken);

        return usersSalary
            .Select(salary => new UserSalaryCoreDto(
                salary.Id,
                salary.UserId,
                salary.SalaryFrom,
                SalaryUsd: _salaryEncryptor.Decrypt(salary.SalaryUsd),
                salary.CreatedById,
                salary.CreatedAtUtc))
            .ToList();
    }
}

internal record UsersSalaryByMonthLoaderRequest(int[] UserIds, int Month, int Year);
