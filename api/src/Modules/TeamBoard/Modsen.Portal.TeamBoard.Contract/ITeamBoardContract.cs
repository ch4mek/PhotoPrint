using Modsen.Portal.Core;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.Contract.Contract.Input;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;

namespace Modsen.Portal.TeamBoard.Contract;

public interface ITeamBoardContract
{
    Task<List<ShortHardSkillGroupOutput>> GetHardSkillGroups(CancellationToken cancellationToken);

    Task<List<HardSkillOutput>> GetHardSkills(CancellationToken cancellationToken);

    Task<List<SoftSkillOutput>> GetSoftSkills(CancellationToken cancellationToken);

    Task<List<SeniorityLevelOutput>> GetSeniorityLevels(SeniorityLevelScopes seniorityLevelScope, CancellationToken cancellationToken);

    Task<List<EnglishLevelOutput>> GetEnglishLevels(CancellationToken cancellationToken);

    Task<List<CountryOutput>> GetCountries(CancellationToken cancellationToken);

    Task<List<UserHireDateOutput>> GetUsersHireDates(CancellationToken cancellationToken);

    Task<UserContractDatesOutput?> GetUserContractDates(int userId, CancellationToken cancellationToken);
    Task<List<UserContractDatesOutput>> GetUserContractsDates(int userId, CancellationToken cancellationToken, bool onlyActiveUsers = true);
    Task<List<UserContractDatesOutput>> GetUsersActiveContractDates(DateOnly? activeOnDate, CancellationToken cancellationToken);
    Task<List<UserContractDatesOutput>> GetUsersExpiredContractsDates(DateOnly lastDay, CancellationToken cancellationToken);

    Task<SeniorityLevelOutput> GetSeniorityLevelByUserProjectId(int projectUserId, CancellationToken cancellationToken);
    Task<List<UserWithRuNameOutput>> GetUsersRuNames(int[] userIds, CancellationToken cancellationToken);

    Task<bool> IsUserHaveAccessToPayslipFile(string payslipFileId, CancellationToken cancellationToken);
    Task<BusinessRuleResult> ArePersonalPhoneAndEmailsUniqueAsync(string? personalEmail,
        string? personalEmailSecond, string? personalPhone, CancellationToken cancellationToken);

    Task<Dictionary<int, bool>> CanUsersBeRetrievedByUser(int[] usersIds,
        AccessOptions accessOptions,
        CancellationToken cancellationToken);

    Task<PermanentUserInfoOutput> GetPermanentUserInfo(string email, CancellationToken cancellationToken);
    Task<ImpermanentUserInfoOutput> GetImpermanentUserInfo(string email, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<UserSalaryOutput>> GetUsersSalariesByMonth(UsersSalaryByMonthInput request, CancellationToken cancellationToken);

    Task<List<KpiColorOutput>> GetKpiColors(CancellationToken cancellationToken);
    string CalculateKpiColor(decimal kpi, IReadOnlyCollection<KpiColorOutput> colors);
}
