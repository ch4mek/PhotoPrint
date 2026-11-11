namespace Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;

internal record CreateProjectRateCoreDto(
    int UserId,
    int ProjectId,
    decimal HourlyRate,
    string CurrencyId,
    DateOnly RateFrom);
