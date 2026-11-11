using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Finance;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.Requests.Core.Builders;
using Modsen.Portal.Finance.Requests.Core.Converters;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Expenses.Core.Encryptor;
using Modsen.Portal.Finance.Requests.Managements.Core.Calculators;
using Modsen.Portal.Finance.Requests.Managements.Core.CoreDto;
using Modsen.Portal.Finance.Requests.Managements.Core.Extensions;
using Modsen.Portal.Finance.Requests.Managements.Requests.RequestDtos;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Encryptor;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Input;
using Modsen.Portal.TimeTrack.Contract;
using Modsen.Portal.TimeTrack.Contract.Contract.Input;

namespace Modsen.Portal.Finance.Requests.Managements.Requests;

internal sealed class GetManagementsRequest : IQueryHandler<GetManagementsRequest.Query, PageableManagementsDto>
{
    private readonly IFinanceDbContext _financeDbContext;
    private readonly IManagementsCalculator _managementsCalculator;
    private readonly ITimeTrackContract _timeTrackContract;
    private readonly ITeamBoardContract _teamBoardContract;
    private readonly ICurrencyConverter _currencyConverter;
    private readonly IIdentityContract _identityContract;
    private readonly IExpenseEncryptor _expenseEncryptor;
    private readonly IProjectRateEncryptor _numberEncryptor;
    private readonly IGetProjectRatesQueryBuilder _getRatesQueryBuilder;

    public GetManagementsRequest(
        IFinanceDbContext financeDbContext,
        IManagementsCalculator managementsCalculator,
        ITimeTrackContract timeTrackContract,
        ITeamBoardContract teamBoardContract,
        ICurrencyConverter currencyConverter,
        IIdentityContract identityContract,
        IExpenseEncryptor expenseEncryptor,
        IProjectRateEncryptor numberEncryptor,
        IGetProjectRatesQueryBuilder getRatesQueryBuilder)
    {
        _financeDbContext = financeDbContext;
        _managementsCalculator = managementsCalculator;
        _timeTrackContract = timeTrackContract;
        _teamBoardContract = teamBoardContract;
        _currencyConverter = currencyConverter;
        _identityContract = identityContract;
        _expenseEncryptor = expenseEncryptor;
        _numberEncryptor = numberEncryptor;
        _getRatesQueryBuilder = getRatesQueryBuilder;
    }

    public async Task<PageableManagementsDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var byn = await _financeDbContext.Currencies
            .Where(x => x.Id == CurrencyCodes.BelarusianRuble)
            .Select(x => CurrencyCoreDto.Of(x))
            .FirstOrDefaultAsync(cancellationToken);

        if (byn == null)
        {
            throw new AppException("Byn currency does not exist in the database.");
        }

        var response = await GetManagementUsers(request, byn, cancellationToken);

        var managementUsers = response.Users;

        await SetBillableTrackedHours(
            managementUsers.SelectMany(x => x.Projects).ToList(),
            request,
            cancellationToken);

        await SetUsersSalaries(managementUsers, request, cancellationToken);

        decimal? expenses = request.ShouldCalculateMainStatistics
            ? await GetExpensesAmount(request, cancellationToken)
            : null;

        var statistics = await _managementsCalculator.Calculate(new ManagementsCalculator.Request
        {
            ManagementUsers = managementUsers,
            Year = request.Year,
            Month = request.Month,
            ShouldCalculateProfit = request.ShouldCalculateMainStatistics,
            ShouldCalculateColumnStatistics = request.ShouldCalculateColumnStatistics,
            Expenses = expenses
        },
        cancellationToken);

        if (request.ShouldCalculateMainStatistics)
        {
            managementUsers.ApplyFiltering(request.Filter);
        }

        if (request.ShouldCalculateColumnStatistics || request.Ordering.ShouldOrderByStatistics || request.ShouldCalculateMainStatistics)
        {
            managementUsers = managementUsers
                    .ApplyOrdering(request.Ordering)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();
        }

        var pagebaleManagements = new PageableManagementsDto
        {
            Items = managementUsers,
            Year = request.Year,
            Month = request.Month,
            Page = request.PageNumber,
            PageSize = request.PageSize,
            Total = response.TotalCount,
            Expenses = expenses,
            Profit = request.ShouldCalculateMainStatistics ? statistics.Profit : null,
            IncomeBynAmount = request.ShouldCalculateColumnStatistics ? statistics.IncomeBynAmount : null,
            SalaryAmount = request.ShouldCalculateColumnStatistics ? statistics.SalaryAmount : null,
            AverageKpi = request.ShouldCalculateColumnStatistics ? statistics.AverageKpi : null,
            AverageKpiColor = statistics.AverageKpiColor,
        };

        return pagebaleManagements;
    }

    private async Task<(List<ManagementUserDto> Users, int TotalCount)> GetManagementUsers(Query request, CurrencyCoreDto byn, CancellationToken cancellationToken)
    {
        _getRatesQueryBuilder.ForYearAndMonth(request.Year, request.Month);

        if (!request.ShouldCalculateMainStatistics)
        {
            _getRatesQueryBuilder.WithFilter(request.Filter);

            if (!request.Ordering.ShouldOrderByStatistics && !request.ShouldCalculateColumnStatistics)
            {
                _getRatesQueryBuilder
                    .WithOrdering(request.Ordering.Map())
                    .WithPagination((request.PageNumber - 1) * request.PageSize, request.PageSize);
            }
        }

        var getManagementsQueryBuilderDto = await _getRatesQueryBuilder.BuildRates(cancellationToken, isForManagement: true);

        var userHistoricalDeps = await _identityContract.UsersHistoricalDepartment(
            request.Year,
            request.Month,
            getManagementsQueryBuilderDto.UsersIds,
            cancellationToken);

        var accessOptions = AccessOptions.Manager | AccessOptions.Self;

        var visibility = await _teamBoardContract.CanUsersBeRetrievedByUser(
            getManagementsQueryBuilderDto.UsersIds.ToArray(),
            accessOptions,
            cancellationToken);

        var result = new List<ManagementUserDto>();

        foreach (var userId in getManagementsQueryBuilderDto.UsersIds)
        {
            var userAssigns = getManagementsQueryBuilderDto.ProjectUsers.FirstOrDefault(x => x.Key == userId)?.Select(g => g).ToList();
            var userRates = getManagementsQueryBuilderDto.Rates.FirstOrDefault(x => x.Key == userId)?.Select(g => g);

            var userProjects = new List<ManagementProjectDto>();

            if (userAssigns!.Count == 0)
            {
                continue;
            }

            foreach (var assign in userAssigns)
            {
                var rate = userRates?.FirstOrDefault(r => r.ProjectId == assign.ProjectId);

                var project = new ManagementProjectDto
                {
                    UserId = userId,
                    ProjectId = assign.ProjectId,
                    ProjectName = assign.Project!.ProjectName,
                    ProjectColor = assign.Project!.Color,
                    ClientId = assign.Project!.ClientId,
                    ClientName = assign.Project!.Client!.Name,
                    ClientLegalEntity = assign.Project!.Client.LegalEntity,
                    HourlyRate = rate != null
                        ? _numberEncryptor.Decrypt(rate.HourlyRate)
                        : null,
                    HourlyRateCurrency = rate?.Currency != null
                        ? CurrencyCoreDto.Of(rate.Currency)
                        : null,
                    BynCurrency = byn
                };

                userProjects.Add(project);
            }

            var userEntity = userAssigns[0].User!;

            var managementUser = new ManagementUserDto
            {
                Id = userEntity.Id,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                FirstNameRu = userEntity.FirstNameRu,
                LastNameRu = userEntity.LastNameRu,
                IsActive = userEntity.IsActive,
                DepartmentId = userHistoricalDeps.GetValueOrDefault(userEntity.Id) ?? userEntity.DepartmentId,
                Projects = userProjects,
                CanUserInfoBeRetrievedByCurrentUser = visibility.GetValueOrDefault(userEntity.Id)
            };

            result.Add(managementUser);
        }

        return (result, getManagementsQueryBuilderDto.TotalUsersCount);
    }

    private async Task SetBillableTrackedHours(List<ManagementProjectDto> projects, Query request, CancellationToken cancellationToken)
    {
        var input = projects
            .Select(x => new TimesheetByUserInput
            {
                UserId = x.UserId,
                ProjectId = x.ProjectId,
                Year = request.Year,
                Month = request.Month
            })
            .ToList();

        var timesheets = await _timeTrackContract.GetTimesheetByUsers(input, cancellationToken);

        if (timesheets.Count == 0)
        {
            return;
        }

        foreach (var project in projects)
        {
            var billableTrackedMinutes = timesheets
                .Where(x => x.UserId == project.UserId && x.ProjectId == project.ProjectId)
                .Select(x => x.BillableTrackedSum)
                .DefaultIfEmpty()
                .Sum();

            project.BillableTrackedHours = (decimal)billableTrackedMinutes / 60m;
        }
    }

    private async Task SetUsersSalaries(List<ManagementUserDto> users, Query request, CancellationToken cancellationToken)
    {
        var userIds = users.Select(x => x.Id).ToList();

        var input = new UsersSalaryByMonthInput
        {
            UserIds = userIds,
            Year = request.Year,
            Month = request.Month
        };

        var salaries = await _teamBoardContract.GetUsersSalariesByMonth(input, cancellationToken);

        var currencyConverterRequests = salaries
            .DistinctBy(x => x.SalaryUsd)
            .Select(x => new CurrencyConverter.Request
            {
                FromCurrencyCode = CurrencyCodes.USDollar,
                FromCurrencyCount = x.SalaryUsd,
                ToCurrencyCode = CurrencyCodes.BelarusianRuble,
                Year = request.Year,
                Month = request.Month
            })
            .ToList();

        var converterResults = await _currencyConverter.Convert(currencyConverterRequests, cancellationToken);

        foreach (var user in users)
        {
            var userSalaryUsd = salaries
                .Where(x => x.UserId == user.Id)
                .Select(x => x.SalaryUsd)
                .FirstOrDefault();

            if (userSalaryUsd == default)
            {
                continue;
            }

            user.Salary = converterResults
                .Where(x => x.FromCurrencyCount == userSalaryUsd)
                .Select(x => x.Result)
                .FirstOrDefault();
        }
    }

    private async Task<decimal> GetExpensesAmount(Query request, CancellationToken cancellationToken)
    {
        var expenses = await _financeDbContext.Expenses
            .Where(x => x.Year == request.Year && x.MonthOfYear == request.Month)
            .Where(x => !x.IsDeleted)
            .Select(x => x.ExpenseAmount)
            .ToListAsync(cancellationToken);

        var expensesAmount = decimal.Zero;

        foreach (var expense in expenses)
        {
            expensesAmount += _expenseEncryptor.Decrypt(expense);
        }

        return expensesAmount;
    }

    internal sealed class Query : IQuery<PageableManagementsDto>
    {
        public int Year { get; init; }
        public int Month { get; init; }

        public int PageSize { get; init; }
        public int PageNumber { get; init; }

        public ManagementsFilterCoreDto Filter { get; init; } = new();
        public ManagementsOrderingCoreDto Ordering { get; init; } = new();

        public bool ShouldCalculateMainStatistics { get; init; }
        public bool ShouldCalculateColumnStatistics { get; init; }
    }

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Year).GreaterThanOrEqualTo(2020);

            RuleFor(x => x.Month).InclusiveBetween(1, 12);
        }
    }
}
