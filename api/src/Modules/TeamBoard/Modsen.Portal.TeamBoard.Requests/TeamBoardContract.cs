using MediatR;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Services.BusinessRules;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Input;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Modsen.Portal.TeamBoard.Requests.Contracts.Requests;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.Calculators;
using Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests;
using Modsen.Portal.TeamBoard.Requests.Payload.KpiColors.Requests.RequestDto;
using Modsen.Portal.TeamBoard.Requests.Payslips.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Projects.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.EnglishLevels.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.SeniorityLevels.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;
using Modsen.Portal.TeamBoard.Requests.UserPersonalInfo.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Users.Requests;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Loaders;

namespace Modsen.Portal.TeamBoard.Requests;

internal class TeamBoardContract : ITeamBoardContract
{
    private readonly ISender _sender;
    private readonly IIsUserHaveAccessToPayslipFileBusinessRule _isUserHaveAccessToPayslipFileBusinessRule;
    private readonly IArePersonalPhoneAndEmailsUniqueBusinessRule _arePersonalPhoneAndEmailsUniqueBusinessRule;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly IUserSalaryLoader _userSalaryLoader;
    private readonly IKpiColorCalculator _kpiColorCalculator;

    public TeamBoardContract(
        ISender sender,
        IIsUserHaveAccessToPayslipFileBusinessRule isUserHaveAccessToPayslipFileBusinessRule,
        IArePersonalPhoneAndEmailsUniqueBusinessRule arePersonalPhoneAndEmailsUniqueBusinessRule,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        IUserSalaryLoader userSalaryLoader,
        IKpiColorCalculator kpiColorCalculator)
    {
        _sender = sender;
        _isUserHaveAccessToPayslipFileBusinessRule = isUserHaveAccessToPayslipFileBusinessRule;
        _arePersonalPhoneAndEmailsUniqueBusinessRule = arePersonalPhoneAndEmailsUniqueBusinessRule;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _userSalaryLoader = userSalaryLoader;
        _kpiColorCalculator = kpiColorCalculator;
    }

    public async Task<List<ShortHardSkillGroupOutput>> GetHardSkillGroups(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetHardSkillGroupsRequest.Query(), cancellationToken);

        return result.ConvertAll(group => new ShortHardSkillGroupOutput
        {
            Id = group.Id,
            Name = group.Name
        });
    }

    public async Task<List<HardSkillOutput>> GetHardSkills(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetHardSkillsLookupRequest.Query(), cancellationToken);

        return result.ConvertAll(skill => new HardSkillOutput
        {
            Id = skill.Id,
            Name = skill.Name
        });
    }

    public async Task<List<SoftSkillOutput>> GetSoftSkills(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetSoftSkillsLookupRequest.Query(), cancellationToken);

        return result.ConvertAll(skill => new SoftSkillOutput
        {
            Id = skill.Id,
            Name = skill.Name
        });
    }

    public async Task<List<SeniorityLevelOutput>> GetSeniorityLevels(SeniorityLevelScopes seniorityLevelScope,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetSeniorityLevelsRequest.Query(seniorityLevelScope),
            cancellationToken);

        return result.ConvertAll(seniority => new SeniorityLevelOutput
        {
            Id = seniority.Id,
            Name = seniority.Name
        });
    }

    public async Task<List<EnglishLevelOutput>> GetEnglishLevels(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetEnglishLevelsRequest.Query(), cancellationToken);

        return result.ConvertAll(englishLevel => new EnglishLevelOutput
        {
            Id = englishLevel.Id,
            Name = englishLevel.Name
        });
    }

    public async Task<List<CountryOutput>> GetCountries(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetCountriesRequest.Query(), cancellationToken);

        return result.ConvertAll(country => new CountryOutput
        {
            Id = country.Id,
            Name = country.Name
        });
    }

    public async Task<List<UserHireDateOutput>> GetUsersHireDates(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetUsersHireDatesRequest.Query(), cancellationToken);

        return result.ConvertAll(x => new UserHireDateOutput
        {
            UserId = x.UserId,
            HireDate = x.HireDate
        });
    }

    public async Task<UserContractDatesOutput?> GetUserContractDates(int userId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetUserContractDatesRequest.Query(userId), cancellationToken);

        return result == null ? null : new UserContractDatesOutput
        {
            UserId = result.UserId,
            HireDate = result.HireDate,
            ContractStartDate = result.ContractStartDate,
            ContractEndDate = result.ContractEndDate,
            FireDate = result.FireDate,
        };
    }

    public async Task<List<UserContractDatesOutput>> GetUserContractsDates(int userId, CancellationToken cancellationToken, bool onlyActiveUsers = true)
    {
        var result = await _sender.Send(
            new GetUserContractsDatesRequest.Query(userId, onlyActiveUsers), cancellationToken);

        return result.ConvertAll(dates => new UserContractDatesOutput
        {
            UserId = dates.UserId,
            HireDate = dates.HireDate,
            ContractStartDate = dates.ContractStartDate,
            ContractEndDate = dates.ContractEndDate,
            FireDate = dates.FireDate,
        });
    }

    public async Task<List<UserContractDatesOutput>> GetUsersActiveContractDates(DateOnly? activeOnDate, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetActiveUsersContractDatesRequest.Query(activeOnDate), cancellationToken);

        return result.ConvertAll(dates => new UserContractDatesOutput
        {
            UserId = dates.UserId,
            HireDate = dates.HireDate,
            ContractStartDate = dates.ContractStartDate,
            ContractEndDate = dates.ContractEndDate,
            FireDate = dates.FireDate,
        });
    }

    public async Task<List<UserContractDatesOutput>> GetUsersExpiredContractsDates(DateOnly lastDay, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetUsersExpiredContractsDatesRequest.Query(lastDay), cancellationToken);

        return result.ConvertAll(dates => new UserContractDatesOutput
        {
            UserId = dates.UserId,
            HireDate = dates.HireDate,
            ContractStartDate = dates.ContractStartDate,
            ContractEndDate = dates.ContractEndDate,
            FireDate = dates.FireDate,
        });
    }

    public Task<SeniorityLevelOutput> GetSeniorityLevelByUserProjectId(int projectUserId, CancellationToken cancellationToken)
    {
        return _sender.Send(new GetAssignmentSeniorityLevelIdRequest.Query(projectUserId), cancellationToken);
    }

    public Task<List<UserWithRuNameOutput>> GetUsersRuNames(int[] userIds, CancellationToken cancellationToken)
    {
        return _sender.Send(new GetUsersRuNamesRequest.Query(userIds), cancellationToken);
    }

    public Task<bool> IsUserHaveAccessToPayslipFile(string payslipFileId, CancellationToken cancellationToken)
    {
        return _isUserHaveAccessToPayslipFileBusinessRule.IsCorrespondsToBusiness(
            new IsUserHaveAccessToPayslipFileBusinessRuleModel(payslipFileId),
            cancellationToken);
    }

    public Task<BusinessRuleResult> ArePersonalPhoneAndEmailsUniqueAsync(string? personalEmail,
        string? personalEmailSecond, string? personalPhone, CancellationToken cancellationToken)
    {
        return _arePersonalPhoneAndEmailsUniqueBusinessRule.IsCorrespondsToBusiness(
            new ArePersonalPhoneAndEmailsUniqueBusinessModel
            {
                UserId = null,
                PersonalEmail = personalEmail,
                PersonalEmailSecond = personalEmailSecond,
                PersonalPhone = personalPhone,
            }, cancellationToken);
    }

    public Task<Dictionary<int, bool>> CanUsersBeRetrievedByUser(
        int[] usersIds,
        AccessOptions accessOptions,
        CancellationToken cancellationToken)
    {
        return _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusinessBatch(usersIds, accessOptions, cancellationToken);
    }

    public async Task<PermanentUserInfoOutput> GetPermanentUserInfo(string email, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetPermanentUserInfoRequest.Query(email), cancellationToken);

        return new PermanentUserInfoOutput
        {
            FirstName = result.FirstName,
            LastName = result.LastName,
            FirstNameRu = result.FirstNameRu,
            LastNameRu = result.LastNameRu,
            PatronymicRu = result.PatronymicRu,
            HireDate = result.HireDate,
            FirstProjectInCompanyStarted = result.FirstProjectInCompanyStarted,
            Contracts = result.Contracts?.Select(x => new ContractOutput
            {
                DateFrom = x.DateFrom,
                DateTo = x.DateTo,
            }).ToList(),
            Email = result.Email,
            Departament = result.DepartamentId,
            GroupName = result.ResourceGroupName,
            Managers = result.Managers?.Select(x => new ManagerOutput
            {
                Position = x.Position,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FirstNameRu = x.FirstNameRu,
                LastNameRu = x.LastNameRu,
            }).ToList(),
            DateOfBirth = result.DateOfBirth,
            Country = result.Country,
            CurrentLocation = result.CurrentLocation,
            EnglishLevel = result.EnglishLevel,
            IsGraduateJobPlacement = result.IsGraduateJobPlacement,
            LinkedIn = result.LinkedIn,
            Location = result.Location,
            MilitaryServiceStatus = result.MilitaryServiceStatus,
            PersonalEmail = result.PersonalEmail,
            PersonalEmailSecond = result.PersonalEmailSecond,
            PersonalPhone = result.PersonalPhone,
            Position = result.Position,
            SeniorityLevel = result.SeniorityLevel,
            Telegram = result.Telegram,
        };
    }

    public async Task<ImpermanentUserInfoOutput> GetImpermanentUserInfo(string email, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetImpermanentUserInfoRequest.Query(email), cancellationToken);

        return new ImpermanentUserInfoOutput
        {
            FuturePerformanceReviewDate = result.FuturePerformanceReviewDate,
            BalanceDays = result.BalanceDays?.Select(x => new UserBalanceDaysOutput
            {
                Balance = x.Balance,
                InquiryTemplateName = x.InquiryTemplateName,
                InquiryType = x.InquiryType,
            }).ToList(),
            OpenRequests = result.OpenRequests?.Select(x => new UserInquiryOutput
            {
                DateFrom = x.DateFrom,
                DateTo = x.DateTo,
                InquiryTemplateName = x.InquiryTemplateName,
                InquiryType = x.InquiryType,
                Status = x.Status,
            }).ToList(),
        };
    }

    public async Task<IReadOnlyCollection<UserSalaryOutput>> GetUsersSalariesByMonth(UsersSalaryByMonthInput request, CancellationToken cancellationToken)
    {
        var usersSalaryByMonthLoaderRequest = new UsersSalaryByMonthLoaderRequest(request.UserIds.ToArray(), request.Month, request.Year);

        var response = await _userSalaryLoader.LoadAsync(usersSalaryByMonthLoaderRequest, cancellationToken);

        return response
            .Select(x => new UserSalaryOutput
            {
                UserId = x.UserId,
                SalaryUsd = x.SalaryUsd,
            })
            .ToList();
    }

    public async Task<List<KpiColorOutput>> GetKpiColors(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetKpiColorsRequest.Query(), cancellationToken);

        return result
            .Select(x => new KpiColorOutput
            {
                Id = x.Id,
                Name = x.Name,
                RightIntervalBorder = x.RightIntervalBorder
            })
            .ToList();
    }

    public string CalculateKpiColor(decimal kpi, IReadOnlyCollection<KpiColorOutput> colors)
    {
        var kpiColorDtos = colors
            .Select(x => new KpiColorDto
            {
                Id = x.Id,
                Name = x.Name,
                RightIntervalBorder = x.RightIntervalBorder
            })
            .ToList();

        return _kpiColorCalculator.GetKpiColor(kpi, kpiColorDtos);
    }
}
