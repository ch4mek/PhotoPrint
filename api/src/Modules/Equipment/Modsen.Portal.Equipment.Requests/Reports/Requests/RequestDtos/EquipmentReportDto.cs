
namespace Modsen.Portal.Equipment.Requests.Reports.Requests.RequestDtos;

internal class EquipmentReportDto
{
    public int Id { get; set; }
    public required string SerialNumber { get; set; }
    public required string Data { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime AssembledAtUtc { get; set; }
}
