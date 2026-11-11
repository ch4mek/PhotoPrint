using Modsen.Portal.Equipment.Requests.Reports.Requests.RequestDtos;

namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

public class EquipmentReportOutput
{
    public int Id { get; set; }
    public required string SerialNumber { get; set; }
    public required string Data { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime AssembledAtUtc { get; set; }

    internal static EquipmentReportOutput Of(EquipmentReportDto source)
    {
        return new EquipmentReportOutput
        {
            Id = source.Id,
            Data = source.Data,
            SerialNumber = source.SerialNumber,
            AssembledAtUtc = source.AssembledAtUtc,
            CreatedAtUtc = source.CreatedAtUtc
        };
    }
}
