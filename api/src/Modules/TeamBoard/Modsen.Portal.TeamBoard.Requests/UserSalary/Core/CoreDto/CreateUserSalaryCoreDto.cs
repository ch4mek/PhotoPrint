using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Core.CoreDto;

public record CreateUserSalaryCoreDto(
    int UserId,
    DateOnly SalaryFrom,
    decimal SalaryUsd,
    decimal SalaryInCurrency,
    string SalaryCurrencyId,
    int? CreatedById,
    DateTime CreatedAtUtc,
    UserSalarySource? Source,
    int? ConversationId);
