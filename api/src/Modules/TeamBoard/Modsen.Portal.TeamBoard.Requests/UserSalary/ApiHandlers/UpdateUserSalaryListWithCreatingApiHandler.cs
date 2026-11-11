using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Encryptor;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.ApiHandlers;

internal interface IUpdateUserSalaryListWithCreatingApiHandler
{
    Task<bool> UpdateAsync(int userId, IReadOnlyCollection<UserSalaryFullCoreDto> salaries, CancellationToken cancellationToken);
}

internal sealed class UpdateUserSalaryListWithCreatingApiHandler : IUpdateUserSalaryListWithCreatingApiHandler
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IUserSalaryComposer _userSalaryComposer;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardPortalSettingsAccessor _teamBoardPortalSettingsAccessor;
    private readonly ISalaryEncryptor _salaryEncryptor;
    private readonly IIsSalaryFromDateValidBusinessRule _isSalaryFromDateValidBusinessRule;

    public UpdateUserSalaryListWithCreatingApiHandler(
        ITeamBoardDbContext teamBoardDbContext,
        IUserSalaryComposer userSalaryComposer,
        IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardPortalSettingsAccessor teamBoardPortalSettingsAccessor,
        ISalaryEncryptor salaryEncryptor,
        IIsSalaryFromDateValidBusinessRule isSalaryFromDateValidBusinessRule)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _userSalaryComposer = userSalaryComposer;
        _authenticatedUserContext = authenticatedUserContext;
        _teamBoardPortalSettingsAccessor = teamBoardPortalSettingsAccessor;
        _salaryEncryptor = salaryEncryptor;
        _isSalaryFromDateValidBusinessRule = isSalaryFromDateValidBusinessRule;
    }

    public async Task<bool> UpdateAsync(int userId, IReadOnlyCollection<UserSalaryFullCoreDto> salaries, CancellationToken cancellationToken)
    {
        var userSalaries = await _teamBoardDbContext.UserSalaries
            .Where(userSalary => userSalary.UserId == userId)
            .ToListAsync(cancellationToken);

        var settings = await _teamBoardPortalSettingsAccessor.GetSettings(cancellationToken);

        ArgumentNullException.ThrowIfNull(settings.UsdCurrencyId);

        var createUserSalaryList = new List<CreateUserSalaryCoreDto>();

        if (salaries.Count != salaries.Select(salary => salary.SalaryFrom).ToHashSet().Count)
        {
            throw new EntityConflictException(PortalMessages.SalaryDateConflicts);
        }

        foreach (var salary in salaries)
        {
            var isDateInvalid = await _isSalaryFromDateValidBusinessRule.IsInvalid(
                salary.Id,
                salary.UserId,
                salary.SalaryFrom,
                cancellationToken);

            if (isDateInvalid)
            {
                throw new EntityConflictException(PortalMessages.SalaryDateConflicts);
            }

            var userSalary = userSalaries.FirstOrDefault(userSalary => userSalary.Id == salary.Id);

            if (userSalary is null)
            {
                createUserSalaryList.Add(new CreateUserSalaryCoreDto
                (
                    UserId: salary.UserId,
                    SalaryFrom: salary.SalaryFrom,
                    SalaryUsd: salary.SalaryUsd,
                    SalaryInCurrency: salary.SalaryUsd,
                    SalaryCurrencyId: settings.UsdCurrencyId,
                    CreatedById: _authenticatedUserContext.UserId,
                    CreatedAtUtc: DateTime.UtcNow,
                    Source: salary.Source ?? UserSalarySource.Unknown,
                    ConversationId: salary.ConversationId
                ));
            }
            else
            {
                var requestSalaryFrom = salary.SalaryFrom == DateOnly.MaxValue
                    ? salary.SalaryFrom.AddDays(-1)
                    : salary.SalaryFrom;

                userSalary.SalaryFrom = requestSalaryFrom;
                userSalary.SalaryUsd = _salaryEncryptor.Encrypt(salary.SalaryUsd) ?? userSalary.SalaryUsd;
                userSalary.SalaryInCurrency = _salaryEncryptor.Encrypt(salary.SalaryUsd) ?? userSalary.SalaryInCurrency;
            }
        }

        await _userSalaryComposer.ComposeAsync(createUserSalaryList, cancellationToken);

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
