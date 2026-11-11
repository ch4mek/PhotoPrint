namespace Modsen.Portal.Finance.Controllers.Contracts.Input;

public record CreateProjectRateInput(
    int UserId,
    int ProjectId,
    decimal HourlyRate,
    string CurrencyId,
    DateOnly RateFrom);
