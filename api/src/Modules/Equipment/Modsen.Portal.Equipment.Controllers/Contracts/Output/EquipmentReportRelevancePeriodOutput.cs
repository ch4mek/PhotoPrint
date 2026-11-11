using Modsen.Portal.Equipment.Requests.Reports.Requests.RequestDtos;

namespace Modsen.Portal.Equipment.Controllers.Contracts.Output;

public class EquipmentReportRelevancePeriodOutput
{
    public DateTime FromUtc { get; set; }
    public DateTime ToUtc { get; set; }
    public int EquipmentReportId { get; set; }

    internal static EquipmentReportRelevancePeriodOutput Of(EquipmentReportRelevancePeriodDto source)
    {
        return new EquipmentReportRelevancePeriodOutput
        {
            EquipmentReportId = source.EquipmentReportId,
            FromUtc = source.FromUtc,
            ToUtc = source.ToUtc
        };
    }
}
