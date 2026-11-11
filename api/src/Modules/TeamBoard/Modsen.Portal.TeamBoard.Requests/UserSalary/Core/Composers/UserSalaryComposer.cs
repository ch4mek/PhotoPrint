using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Encryptor;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Composers;

internal interface IUserSalaryComposer
{
    Task<UserSalaryCoreDto> ComposeAsync(CreateUserSalaryCoreDto request, CancellationToken cancellationToken);

    Task<List<UserSalaryCoreDto>> ComposeAsync(List<CreateUserSalaryCoreDto> requests,
        CancellationToken cancellationToken);
}

internal class UserSalaryComposer : IUserSalaryComposer
{
    private readonly ITeamBoardDbContext _teamboardDbContext;
    private readonly ISalaryEncryptor _salaryEncryptor;

    public UserSalaryComposer(ITeamBoardDbContext teamboardDbContext, ISalaryEncryptor salaryEncryptor)
    {
        _teamboardDbContext = teamboardDbContext;
        _salaryEncryptor = salaryEncryptor;
    }

    public async Task<UserSalaryCoreDto> ComposeAsync(CreateUserSalaryCoreDto request, CancellationToken cancellationToken)
    {
        var userSalary = new DataAccess.Npgsql.DatabaseEntities.UserSalary
        {
            UserId = request.UserId,
            SalaryFrom = request.SalaryFrom == DateOnly.MaxValue ? request.SalaryFrom.AddDays(-1) : request.SalaryFrom,
            SalaryUsd = _salaryEncryptor.Encrypt(request.SalaryUsd)!,
            SalaryInCurrency = _salaryEncryptor.Encrypt(request.SalaryInCurrency)!,
            SalaryCurrencyId = request.SalaryCurrencyId,
            CreatedById = request.CreatedById,
            CreatedAtUtc = request.CreatedAtUtc,
            Source = request.Source ?? UserSalarySource.Unknown,
            ConversationId = request.ConversationId
        };

        var entity = await _teamboardDbContext.UserSalaries.AddAsync(userSalary, cancellationToken);

        await _teamboardDbContext.SaveChangesAsync(cancellationToken);

        return new UserSalaryCoreDto(
            Id: entity.Entity.Id,
            UserId: entity.Entity.UserId,
            SalaryFrom: entity.Entity.SalaryFrom,
            SalaryUsd: _salaryEncryptor.Decrypt(entity.Entity.SalaryUsd),
            CreatedAtUtc: entity.Entity.CreatedAtUtc,
            CreatedById: entity.Entity.CreatedById);
    }

    public async Task<List<UserSalaryCoreDto>> ComposeAsync(List<CreateUserSalaryCoreDto> requests,
        CancellationToken cancellationToken)
    {
        var userSalaries = requests.ConvertAll(salaryCoreDto => new DataAccess.Npgsql.DatabaseEntities.UserSalary
        {
            UserId = salaryCoreDto.UserId,
            SalaryFrom = salaryCoreDto.SalaryFrom == DateOnly.MaxValue
                ? salaryCoreDto.SalaryFrom.AddDays(-1)
                : salaryCoreDto.SalaryFrom,
            SalaryUsd = _salaryEncryptor.Encrypt(salaryCoreDto.SalaryUsd)!,
            SalaryInCurrency = _salaryEncryptor.Encrypt(salaryCoreDto.SalaryInCurrency)!,
            SalaryCurrencyId = salaryCoreDto.SalaryCurrencyId,
            CreatedById = salaryCoreDto.CreatedById,
            CreatedAtUtc = salaryCoreDto.CreatedAtUtc,
            Source = salaryCoreDto.Source ?? UserSalarySource.Unknown,
            ConversationId = salaryCoreDto.ConversationId
        });

        await _teamboardDbContext.UserSalaries.AddRangeAsync(userSalaries, cancellationToken);
        await _teamboardDbContext.SaveChangesAsync(cancellationToken);

        return await _teamboardDbContext.UserSalaries
            .Where(us =>
                userSalaries.Select(salary => salary.CreatedAtUtc).Contains(us.CreatedAtUtc) &&
                userSalaries.Select(salary => salary.UserId).Contains(us.UserId))
            .Select(us => new UserSalaryCoreDto(
                us.Id,
                us.UserId,
                us.SalaryFrom,
                _salaryEncryptor.Decrypt(us.SalaryUsd),
                us.CreatedById,
                us.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}
