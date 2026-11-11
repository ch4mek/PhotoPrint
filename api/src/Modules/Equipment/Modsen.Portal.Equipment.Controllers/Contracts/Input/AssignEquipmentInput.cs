namespace Modsen.Portal.Equipment.Controllers.Contracts.Input;

public class AssignEquipmentInput
{
    public int UserId { get; set; }
    public required int ApproverId { get; set; }
    public DateTime IssueDate { get; set; }
}
