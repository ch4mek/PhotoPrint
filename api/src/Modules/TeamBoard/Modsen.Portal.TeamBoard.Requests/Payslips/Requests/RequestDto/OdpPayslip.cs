using System.ComponentModel.DataAnnotations;

namespace Modsen.Portal.TeamBoard.Requests.Payslips.Requests.RequestDto;

public class OdpPayslip
{
    [Key]
    public required string FileId { get; set; }
    public required string FileName { get; set; }
    public DateOnly ReportDate { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
