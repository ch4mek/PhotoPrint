using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Input;

internal class UpdateDevelopmentPlanStatusInput
{
    public int DevelopmentPlanId { get; set; }
    public DevelopmentPlanStatusType DevelopmentPlanStatus { get; set; }
    public DateOnly? ConfirmationDate { get; set; }
}
