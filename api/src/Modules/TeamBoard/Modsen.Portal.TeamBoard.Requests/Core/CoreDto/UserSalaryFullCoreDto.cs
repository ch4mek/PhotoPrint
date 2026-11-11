using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

internal class UserSalaryFullCoreDto
{
    public int? Id { get; set; }
    public int UserId { get; set; }
    public DateOnly SalaryFrom { get; set; }
    public decimal SalaryUsd { get; set; }
    public int? ConversationId { get; set; }
    public UserSalarySource? Source { get; set; }
}
