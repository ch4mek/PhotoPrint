using Modsen.Portal.Core.Finance;
using Modsen.Portal.Finance.Requests.Core.Converters;
using Modsen.Portal.Finance.Requests.Managements.Requests.RequestDtos;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Finance.Requests.Managements.Core.Calculators;

internal interface IManagementsCalculator
{
    Task<ManagementsCalculator.Response> Calculate(ManagementsCalculator.Request request, CancellationToken cancellationToken);
}

internal class ManagementsCalculator : IManagementsCalculator
{
    private readonly ICurrencyConverter _currencyConverter;
    private readonly ITeamBoardContract _teamBoardContract;

    public ManagementsCalculator(ICurrencyConverter currencyConverter, ITeamBoardContract teamBoardContract)
    {
        _currencyConverter = currencyConverter;
        _teamBoardContract = teamBoardContract;
    }

    public async Task<Response> Calculate(Request request, CancellationToken cancellationToken)
    {
        var response = new Response();

        if (request.ManagementUsers.Count == 0)
        {
            return response;
        }

        var kpiColors = await _teamBoardContract.GetKpiColors(cancellationToken);

        var projects = request.ManagementUsers
            .SelectMany(x => x.Projects)
            .Where(x => x.HourlyRate != null &&
                        x.BillableTrackedHours != null &&
                        x.HourlyRateCurrency != null)
            .ToList();

        CalculateIncome(projects);

        await ConvertIncomeToByn(projects, request.Year, request.Month, cancellationToken);

        foreach (var user in request.ManagementUsers)
        {
            var shouldCalculateAmount = user.Projects
                .Where(x => x.HourlyRateCurrency != null)
                .DistinctBy(x => x.HourlyRateCurrency!.Id).Count() == 1;

            foreach (var project in user.Projects)
            {
                user.BillableTrackedHoursAmount += project.BillableTrackedHours ?? decimal.Zero;

                if (shouldCalculateAmount)
                {
                    user.HourlyRateAmount += project.HourlyRate ?? decimal.Zero;
                    user.IncomeAmount += project.Income ?? decimal.Zero;
                }

                user.IncomeBynAmount += project.IncomeByn ?? decimal.Zero;
            }

            user.Kpi = CalculateKpi(user.IncomeBynAmount, user.Salary);

            user.KpiColor = _teamBoardContract.CalculateKpiColor(user.Kpi, kpiColors);

            if (request.ShouldCalculateColumnStatistics || request.ShouldCalculateProfit)
            {
                response.IncomeBynAmount = (response.IncomeBynAmount ?? decimal.Zero) + user.IncomeBynAmount;
                response.SalaryAmount = (response.SalaryAmount ?? decimal.Zero) + (user.Salary ?? decimal.Zero);
            }
        }

        if (request.ShouldCalculateColumnStatistics)
        {
            var kpi = CalculateKpi(response.IncomeBynAmount, response.SalaryAmount);

            response.AverageKpi = kpi;
            response.AverageKpiColor = _teamBoardContract.CalculateKpiColor(kpi, kpiColors);

        }
        response.AverageKpi = request.ShouldCalculateColumnStatistics
            ? CalculateKpi(response.IncomeBynAmount, response.SalaryAmount)
            : null;

        if (request.ShouldCalculateProfit)
        {
            CalculateProfit(request, response);
        }

        return response;
    }

    private async Task ConvertIncomeToByn(List<ManagementProjectDto> projects, int year, int month, CancellationToken cancellationToken)
    {
        var projectsToConvert = projects
            .Where(x => x.IncomeByn == null && x.Income != null)
            .ToList();

        var currencyConverterRequests = projectsToConvert
            .DistinctBy(x => new
            {
                Currency = x.HourlyRateCurrency!.CurrencyCode,
                x.Income,
                BynCurrency = x.BynCurrency.CurrencyCode
            })
            .Select(x => new CurrencyConverter.Request
            {
                FromCurrencyCode = x.HourlyRateCurrency!.CurrencyCode,
                FromCurrencyCount = x.Income!.Value,
                ToCurrencyCode = x.BynCurrency.CurrencyCode,
                Year = year,
                Month = month
            })
            .ToList();

        if (currencyConverterRequests.Count == 0)
        {
            return;
        }

        var response = await _currencyConverter.Convert(currencyConverterRequests, cancellationToken);

        foreach (var project in projectsToConvert)
        {
            project.IncomeByn = response
                .Where(x => x.FromCurrencyCode == project.HourlyRateCurrency!.CurrencyCode &&
                            x.FromCurrencyCount == project.Income &&
                            x.ToCurrencyCode == project.BynCurrency.CurrencyCode)
                .Select(x => x.Result)
                .FirstOrDefault();
        }
    }

    private static void CalculateIncome(List<ManagementProjectDto> projects)
    {
        if (projects.Count == 0)
        {
            return;
        }

        foreach (var project in projects)
        {
            project.Income = project.HourlyRate * project.BillableTrackedHours;

            if (project.HourlyRateCurrency!.CurrencyCode == CurrencyCodes.BelarusianRuble)
            {
                project.IncomeByn = project.Income;
            }
        }
    }

    private static void CalculateProfit(Request request, Response response)
    {
        var income = response.IncomeBynAmount ?? decimal.Zero;
        var salary = response.SalaryAmount ?? decimal.Zero;
        var expenses = request.Expenses ?? decimal.Zero;

        response.Profit = income - salary - expenses;
    }

    private static int CalculateKpi(decimal? income, decimal? salary)
    {
        if (salary == null || salary == decimal.Zero || income == null)
        {
            return 0;
        }

        return (int)Math.Round(income.Value / salary.Value * 100);
    }

    internal class Request
    {
        public int Year { get; init; }
        public int Month { get; init; }

        public List<ManagementUserDto> ManagementUsers { get; init; } = [];

        public bool ShouldCalculateProfit { get; init; }
        public bool ShouldCalculateColumnStatistics { get; init; }

        public decimal? Expenses { get; init; }
    }

    internal class Response
    {
        public decimal? Profit { get; set; }

        public decimal? IncomeBynAmount { get; set; }
        public decimal? SalaryAmount { get; set; }
        public int? AverageKpi { get; set; }
        public string? AverageKpiColor { get; set; }
    }
}
