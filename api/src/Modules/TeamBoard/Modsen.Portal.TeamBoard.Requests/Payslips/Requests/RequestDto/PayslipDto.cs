namespace Modsen.Portal.TeamBoard.Requests.Payslips.Requests.RequestDto;

public record PayslipDto
{
    public required string FileId { get; set; }
    public required string FileName { get; set; }
    public DateOnly ReportDate { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
