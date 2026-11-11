namespace Modsen.Portal.Finance.Controllers.Contracts.Input;

public record EditProjectRateInput(    
    decimal HourlyRate,
    string CurrencyId,
    DateOnly RateFrom);
