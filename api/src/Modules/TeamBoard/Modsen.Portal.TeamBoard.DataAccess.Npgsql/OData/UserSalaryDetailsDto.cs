namespace Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

public class UserSalaryDetailsDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateOnly SalaryFrom { get; set; }
    public decimal SalaryUsd { get; set; }
    public string? Source { get; set; }
    public int? ConversationId { get; set; }
    public bool IsCurrent { get; set; }
}
