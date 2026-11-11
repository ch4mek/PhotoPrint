
namespace Modsen.Portal.Equipment.Requests.Reports.Requests.RequestDtos;

internal class EquipmentReportRelevancePeriodDto
{
    public DateTime FromUtc { get; set; }
    public DateTime ToUtc { get; set; }
    public int EquipmentReportId { get; set; }
}
